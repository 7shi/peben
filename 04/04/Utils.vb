Imports System.Text

Module Utils
    Function write16%(image As Byte(), pos%, v As UShort)
        image(pos) = v And &HFF
        image(pos + 1) = v >> 8
        Return pos + 2
    End Function

    Function write16s%(image As Byte(), pos%, ParamArray vs As UShort())
        For Each v In vs
            write16(image, pos, v)
            pos += 2
        Next
        Return pos
    End Function

    Function write32%(image As Byte(), pos%, v%)
        image(pos) = v And &HFF
        image(pos + 1) = (v >> 8) And &HFF
        image(pos + 2) = (v >> 16) And &HFF
        image(pos + 3) = (v >> 24) And &HFF
        Return pos + 4
    End Function

    Function write32s%(image As Byte(), pos%, ParamArray vs%())
        For Each v In vs
            pos = write32(image, pos, v)
        Next
        Return pos
    End Function

    Function writebin%(image As Byte(), pos%, bin As Byte())
        Array.Copy(bin, 0, image, pos, bin.Length)
        Return pos + bin.Length
    End Function

    Function writestr%(image As Byte(), pos%, s$)
        Return writebin(image, pos, Encoding.UTF8.GetBytes(s))
    End Function

    Function conv16(s$) As UShort
        Dim bin = Encoding.UTF8.GetBytes(s)
        ReDim Preserve bin(1)
        Return BitConverter.ToUInt16(bin, 0)
    End Function
End Module
