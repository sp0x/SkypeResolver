Imports System.Net

Public Module Main
    Public res As SkypeResolver
    Public Sub main()
        ResolverServer.Start()
        res = New SkypeResolver(New IPEndPoint(IPAddress.Parse("127.0.0.1"), 133))
        Dim name As String = ""
        Do
            Console.Write("Name to lookup: ")
            name = Console.ReadLine()
            If String.IsNullOrEmpty(name) Then Continue Do
            Dim records As List(Of IpEndpointTree) = res.GetRecordsForName(name)
            If records Is Nothing Then Continue Do
            For Each record In records
                Console.WriteLine(record.ToString)
            Next
        Loop
    End Sub
End Module
