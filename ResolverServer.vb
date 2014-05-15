Imports System.IO

Public Module ResolverServer
    Public SkypekitPath As String = "skypekit.bat"
    Public IpServerPath As String = "ipserver.bat"
    Public Sub Start()
        Environment.CurrentDirectory = "res"
        If File.Exists(SkypekitPath) Then
            Shell(SkypekitPath)
        Else
            MsgBox("Could not find skypekit!", MsgBoxStyle.Critical)
        End If
        Threading.Thread.Sleep(1000 * 5)
        If File.Exists(IpServerPath) Then
            Shell(IpServerPath)
        Else
            MsgBox("Could not find skype IpServer!", MsgBoxStyle.Critical)
        End If
        Threading.Thread.Sleep(1000 * 5)
    End Sub
End Module
