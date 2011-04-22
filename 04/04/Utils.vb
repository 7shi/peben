Imports System.Text

Module Utils
    Sub write16(image As Byte(), pos%, v As UShort)
        image(pos) = v And &HFF
        image(pos + 1) = v >> 8
    End Sub

    Function write16s%(image As Byte(), pos%, ParamArray vs As UShort())
        For Each v In vs
            write16(image, pos, v)
            pos += 2
        Next
        Return pos
    End Function

    Sub write32(image As Byte(), pos%, v%)
        image(pos) = v And &HFF
        image(pos + 1) = (v >> 8) And &HFF
        image(pos + 2) = (v >> 16) And &HFF
        image(pos + 3) = (v >> 24) And &HFF
    End Sub

    Function write32s%(image As Byte(), pos%, ParamArray vs%())
        For Each v In vs
            write32(image, pos, v)
            pos += 4
        Next
        Return pos
    End Function

    Sub writebin(image As Byte(), pos%, bin As Byte())
        Array.Copy(bin, 0, image, pos, bin.Length)
    End Sub

    Sub writestr(image As Byte(), pos%, s$)
        writebin(image, pos, Encoding.UTF8.GetBytes(s))
    End Sub

    Function conv16(s$) As UShort
        Dim bin = Encoding.UTF8.GetBytes(s)
        ReDim Preserve bin(1)
        Return BitConverter.ToUInt16(bin, 0)
    End Function
End Module
