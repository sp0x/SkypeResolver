Imports System.Net
Imports System.Text

''' <summary>
''' Ip tree. Could be used to explain NAT translated addresses.
''' </summary>
''' <remarks></remarks>
Public Class IpEndpointTree
    Inherits IPEndPoint
    Public Property Children As New List(Of IpEndpointTree)
    Public Property Parent As IpEndpointTree
    Public Sub New(endpoint As IPEndPoint)
        MyBase.New(endpoint.Address, endpoint.Port)
    End Sub
    Public Sub New(addr As Long, port As Int32, par As IPEndPoint, Optional child As IPEndPoint = Nothing)
        MyBase.New(addr, port)
        If child IsNot Nothing Then
            Me.Children.Add(New IpEndpointTree(child) With {.Parent = Me})
        End If
        If par IsNot Nothing Then
            Dim parex As New IpEndpointTree(par)
            parex.Children.Add(Me)
            Me.Parent = parex
        End If
    End Sub

    Sub AddChild(p1 As IPEndPoint)
        If Children Is Nothing Then Children = New List(Of IpEndpointTree)
        Children.Add(New IpEndpointTree(p1) With {.Parent = Me})
    End Sub

    Public Overrides Function ToString() As String
        Dim output As New StringBuilder
        output.Append(String.Format("{0}{1}", MyBase.ToString, vbNewLine))
        If Children Is Nothing Then Children = New List(Of IpEndpointTree)
        For Each child As IpEndpointTree In Children
            output.Append(String.Format("   {0}", child.ToString))
        Next
        Return output.ToString
    End Function
End Class

