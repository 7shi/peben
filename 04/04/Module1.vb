Imports System.IO

Module Module1

    Sub Main()
        Dim image(&H3FF) As Byte
        Using fs = New FileStream("output.exe", FileMode.Create)
            fs.Write(image, 0, image.Length)
        End Using
    End Sub

End Module
