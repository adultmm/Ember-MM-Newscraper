Imports System

''' <summary>
''' Marks a test as a pure unit test (no external dependencies)
''' </summary>
<AttributeUsage(AttributeTargets.Method, AllowMultiple:=False)>
Public Class UnitTestAttribute
    Inherits Attribute
End Class

''' <summary>
''' Marks a test as an integration test (requires network, filesystem, etc.)
''' </summary>
<AttributeUsage(AttributeTargets.Method, AllowMultiple:=False)>
Public Class IntegrationTestAttribute
    Inherits Attribute
End Class

''' <summary>
''' Marks a test as interactive (requires user interaction to verify)
''' </summary>
<AttributeUsage(AttributeTargets.Method, AllowMultiple:=False)>
Public Class InteractiveTestAttribute
    Inherits Attribute
End Class
