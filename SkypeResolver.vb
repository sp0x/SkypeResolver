Imports System.Net.Sockets
Imports System.Net
Imports System.IO

Public Class SkypeResolver

#Region "Vars"
    Public Sock As Socket
    Public Endpoint As IPEndPoint
#End Region

#Region "Props"
    Public ReadOnly Property IsConnected As Boolean
        Get
            Return If(Sock IsNot Nothing, Sock.Connected, False)
        End Get
    End Property
#End Region

#Region "Events"
    Public Event Connected As EventHandler(Of SocketAsyncEventArgs)
#End Region

#Region "Construction"
    Public Sub New(endpoint As IPEndPoint)
            Me.Endpoint = endpoint
    End Sub
#End Region

#Region "Connecting"
    Public Sub ConnectAsync(endpoint As IPEndPoint, Optional onConnect As EventHandler(Of SocketAsyncEventArgs) = Nothing)
        Sock = New Socket(AddressFamily.InterNetwork, SocketType.Stream)
        Sock.ConnectAsync(makeSockArgs(Sock, endpoint, _
            Sub(sender As Object, e As SocketAsyncEventArgs)
                RaiseEvent Connected(sender, e)
                If onConnect IsNot Nothing Then onConnect(sender, e)
            End Sub))
    End Sub

    Public Sub Connect(endpoint As IPEndPoint)
        Sock = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Unspecified)
        Do While Not Sock.Connected
            Try
                Console.WriteLine("Skyperesolver retrying..")
                Sock.Connect(endpoint)
            Catch ex As Exception
                System.Threading.Thread.Sleep(5 * 1000)
            End Try
        Loop
        Console.WriteLine("Skyperesolver connected..")
        RaiseEvent Connected(Me, Nothing)
    End Sub
#End Region

    Public Function GetRecordsForName(skypename As String) As List(Of IpEndpointTree)
        If Not IsConnected Then
            Connect(Me.Endpoint)
        End If
        Dim btReq As Byte() = System.Text.Encoding.UTF8.GetBytes(skypename)
        Dim btRes As Byte() = Nothing
        Sock.Send(btReq)
        btRes = netStreamReceiveAll(Sock)
        If btRes Is Nothing Then Return Nothing
        If btRes.Length = 0 Then Return Nothing
        Dim strReply As String = System.Text.Encoding.UTF8.GetString(btRes)
        Dim records As String() = Split(strReply, vbCrLf)
        Dim tmpEndpoint As IpEndpointTree = Nothing
        Dim output As New List(Of IpEndpointTree)
        For Each record As String In records
            Dim extensions As String() = Split(record, "#")
            For Each ext As String In extensions
                If tmpEndpoint Is Nothing Then
                    tmpEndpoint = New IpEndpointTree(makeIpEnd(ext))
                Else
                    tmpEndpoint.AddChild(makeIpEnd(ext))
                End If
            Next
            output.Add(tmpEndpoint) : tmpEndpoint = Nothing
        Next
        Return output
    End Function

    Public Shared Function makeIpEnd(str As String) As IPEndPoint
        Return New IPEndPoint(IPAddress.Parse(str.Substring(0, str.IndexOf(":"))), CInt(str.IndexOf(":") + 1))
    End Function

#Region "Tools"
    Public Shared Function netStreamReceiveAll(ByRef sock As Socket) As Byte()
        Dim bread As Int32 = 0
       Dim tmpBuff As Byte() = New Byte(4 * 1024) {}
        bread = sock.Receive(tmpBuff)
        ReDim Preserve tmpBuff(bread)
        Return tmpBuff
    End Function
    Public Shared Function makeSockArgs(socket As Socket, endpoint As IPEndPoint, onCompleted As EventHandler(Of SocketAsyncEventArgs)) As SocketAsyncEventArgs
        Dim e As SocketAsyncEventArgs = New SocketAsyncEventArgs()
        e.RemoteEndPoint = endpoint
        e.UserToken = socket
        AddHandler e.Completed, onCompleted
        Return e
    End Function
#End Region

End Class
Public Class NotConnectedException
    Inherits Exception
End Class