Imports NLog
Public Class AutoCompleteBehavior(Of T)

    Private ReadOnly comboBox As ComboBox
    Private ReadOnly itemProvider As Func(Of String, List(Of T))
    Private ReadOnly displayMember As String
    Private ReadOnly valueMember As String

    Private lastText As String
    Private skipKeyPress As Boolean
    Private logger As Logger = LogManager.GetCurrentClassLogger()
    Private listDroppedOnPreviewKeyDown As Boolean = False
    Private leaveHandled As Boolean

    Public Sub New(comboBox As ComboBox, itemProvider As Func(Of String, List(Of T)), displayMember As String, valueMember As String)

        Me.comboBox = comboBox
        Me.displayMember = displayMember
        Me.valueMember = valueMember

        ' crucial otherwise exceptions occur when the user types in text which Is Not found in the autocompletion list
        Me.comboBox.AutoCompleteMode = AutoCompleteMode.Suggest

        Me.itemProvider = itemProvider

        AddHandler Me.comboBox.KeyPress, New KeyPressEventHandler(AddressOf Me.OnKeyPress)
        AddHandler Me.comboBox.Leave, New EventHandler(AddressOf Me.OnLeave)
        AddHandler Me.comboBox.SelectionChangeCommitted, New EventHandler(AddressOf Me.OnSelectionChangeCommitted)
        AddHandler Me.comboBox.KeyDown, New KeyEventHandler(AddressOf Me.OnKeyDown)
        AddHandler Me.comboBox.PreviewKeyDown, New PreviewKeyDownEventHandler(AddressOf Me.OnPreviewKeyDown)
        AddHandler Me.comboBox.DropDownClosed, New EventHandler(AddressOf Me.DropDownClosed)
        AddHandler Me.comboBox.Enter, New EventHandler(AddressOf Me.OnEnter)

    End Sub

    Private Sub OnLeave(sender As Object, e As EventArgs)
        HandleLeave()
    End Sub

    Private Sub OnEnter(sender As Object, e As EventArgs)
        logger.Trace("OnEnter")
        leaveHandled = False
        lastText = comboBox.Text
    End Sub

    Private Sub DropDownClosed(sender As Object, e As EventArgs)
        logger.Trace("DropDownClosed")
        HandleLeave()
    End Sub


    Private Sub HandleLeave()
        'we have to handle the case, when the user didn't selected from the list (first item would be selected)
        If Not leaveHandled Then
            'But only once (ClearDataSource() )
            leaveHandled = True

            logger.Trace("HandleLeaveOrEnter: idx={0}, text={1}, lastText={2}", comboBox.SelectedIndex, comboBox.Text, Me.lastText)

            'we have to clear the DataSource to be able to set the Text of the ComboBox 
            ClearDataSource()

            comboBox.Text = Me.lastText

            'SelectAll is the expected behavior (ComboBox.AutoCompleteMode)
            comboBox.SelectAll()
        Else
            logger.Trace("HandleLeave called twice")
        End If
    End Sub

    Private Sub OnSelectionChangeCommitted(sender As Object, e As EventArgs)
        logger.Trace("OnSelectionChangeCommitted: text={0}", comboBox.Text)
        Me.lastText = comboBox.Text
        'the user has selected form the list, so we don't have to handle the first item automatically selected behavior
        Me.leaveHandled = True
    End Sub

    Private Sub OnPreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs)
        logger.Trace("OnPreviewKeyDown: keyCode={0}, droppedDown={1}", e.KeyCode, Me.comboBox.DroppedDown)
        If e.KeyCode = Keys.Enter Then
            If Me.comboBox.DroppedDown Then
                'Workaround: OnPreviewKeyDown called twice, and DroppedDown is False on the second call
                'so we store the original value for the OnKeyDown method and for our second call (ElseIf branch)
                Me.listDroppedOnPreviewKeyDown = True
                e.IsInputKey = True
            ElseIf Me.listDroppedOnPreviewKeyDown Then
                e.IsInputKey = True
            End If
        End If
    End Sub

    Private Sub OnKeyDown(sender As Object, e As KeyEventArgs)
        logger.Trace("OnKeyDown: keyCode={0}, listDroppedOnPreviewKeyDown={1}", e.KeyCode, Me.listDroppedOnPreviewKeyDown)
        If e.KeyCode = Keys.Enter AndAlso Me.listDroppedOnPreviewKeyDown Then
            logger.Trace("OnKeyDown: ENTER HANDLED")
            Me.listDroppedOnPreviewKeyDown = False
            e.Handled = True
            Return
        End If
        If e.KeyCode = Keys.Delete Then
            comboBox.BeginInvoke(New Action(AddressOf Me.ReevaluateCompletionList))
        End If
        If e.KeyCode = Keys.C AndAlso e.Modifiers = Keys.Control Then
            'Avoid drop down the list on Copy operation
            Me.skipKeyPress = True
            Return
        End If
    End Sub

    Private Sub OnKeyPress(sender As Object, e As KeyPressEventArgs)
        logger.Trace("OnKeyPress: keyChar={0}", e.KeyChar)
        If Me.skipKeyPress Then
            Me.skipKeyPress = False
            Return
        End If

        'Its crucial that we use begininvoke because we need the changes to sink into the textfield  
        'Omitting begininvoke would cause the searchterm to lag behind by one character
        'That Is the last character that got typed in
        comboBox.BeginInvoke(New Action(AddressOf Me.ReevaluateCompletionList))
    End Sub

    Private Sub ReevaluateCompletionList()
        Dim currentSearchterm = comboBox.Text
        If currentSearchterm Is Nothing Then
            currentSearchterm = ""
        End If

        'we want to call the itemProvider with empty String (for example for last used values), so the Me.lastText = Nothing default value is expected
        If currentSearchterm = Me.lastText Then
            Return
        End If

        Me.lastText = currentSearchterm
        Dim selectionStart = comboBox.SelectionStart
        Dim selectionLength = comboBox.SelectionLength

        Try
            comboBox.SuspendLayout()

            'Nothing is accepted here
            Dim items = Me.itemProvider(currentSearchterm)

            comboBox.DataSource = items
            comboBox.DisplayMember = Me.displayMember
            comboBox.ValueMember = Me.valueMember

            If (currentSearchterm.Length >= 1 AndAlso comboBox.Items.Count > 0 AndAlso Not comboBox.DroppedDown) Then
                'If the current searchterm Is empty we leave the dropdown list to whatever state it already had
                comboBox.DroppedDown = True
                comboBox.SelectedIndex = -1

                'workaround for the fact the cursor disappears due To droppeddown=True, this is a known bug plaguing combobox which microsoft denies to fix for years now
                Cursor.Current = Cursors.Default
            ElseIf (comboBox.Items.Count = 0 AndAlso comboBox.DroppedDown) Then
                CloseDropDown()
            End If
        Finally
            'Another workaround for a glitch which causes all text to be selected when there is a matching entry which starts with the exact text being typed In
            comboBox.Text = currentSearchterm
            comboBox.Select(Math.Min(selectionStart, currentSearchterm.Length), Math.Min(selectionLength, currentSearchterm.Length - selectionStart))
            comboBox.ResumeLayout(True)
            logger.Trace("ReevaluateCompletionList finished")
        End Try

    End Sub

    Private Sub CloseDropDown()
        If Not comboBox.DroppedDown Then
            Return
        End If

        'workaround for the System.ArgumentOutOfRangeException with empty Item list
        If comboBox.Items.Count = 0 Then
            'we have to clear the DataSource to be able to add the dummy item
            '(yes, yes... it's a workaround to be able to do the next workaround)
            ClearDataSource()

            'workaround for the System.ArgumentOutOfRangeException with empty Item list
            comboBox.Items.Add(String.Empty)

            'After the list has been closed we have to clear the dummy element
            comboBox.BeginInvoke(New Action(AddressOf Me.ClearDummyItems))
        End If

        comboBox.SelectedIndex = -1
        comboBox.DroppedDown = False

    End Sub

    Private Sub ClearDataSource()
        logger.Trace("ClearDataSource: text={0}, dropped={1}", comboBox.Text, comboBox.DroppedDown)

        'setting DataSource to Nothing also set Text to Empty, so we save and restore the Text
        Dim textBefore = Me.comboBox.Text
        Try
            'we have to clear the items to be able to set the Text of the ComboBox 
            comboBox.DataSource = Nothing
            comboBox.DisplayMember = String.Empty
            comboBox.ValueMember = String.Empty
        Finally
            comboBox.Text = textBefore
        End Try
    End Sub

    Private Sub ClearDummyItems()
        'Dim currentSearchterm = comboBox.Text
        'If currentSearchterm Is Nothing Then
        '    currentSearchterm = ""
        'End If
        'Dim selectionStart = comboBox.SelectionStart
        'Dim selectionLength = comboBox.SelectionLength

        If comboBox.DataSource Is Nothing Then
            'it means the Items contains only the dummy element 
            'clear list by loop through it otherwise the cursor would move to the beginning of the textbox
            While comboBox.Items.Count > 0
                comboBox.Items.RemoveAt(0)
            End While
        End If

        'comboBox.Text = currentSearchterm
        'comboBox.Select(Math.Min(selectionStart, currentSearchterm.Length), Math.Min(selectionLength, currentSearchterm.Length - selectionStart))
    End Sub

End Class
