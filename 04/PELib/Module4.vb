Public Module Module4
    Function conv16(s$) As UShort
        Dim bin = Text.Encoding.ASCII.GetBytes(s)
        ReDim Preserve bin(1)
        Return BitConverter.ToUInt16(bin, 0)
    End Function

    Function conv32%(s$)
        Dim bin = Text.Encoding.ASCII.GetBytes(s)
        ReDim Preserve bin(3)
        Return BitConverter.ToInt32(bin, 0)
    End Function

    Function getBytes(s$, len%)
        Dim bin = Text.Encoding.ASCII.GetBytes(s)
        ReDim Preserve bin(len - 1)
        Return bin
    End Function
End Module
