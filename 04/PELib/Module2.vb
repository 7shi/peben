Public Module Module2
    Function write8%(image As Byte(), pos%, ParamArray bin As Byte())
        Array.Copy(bin, 0, image, pos, bin.Length)
        Return pos + bin.Length
    End Function

    Function write16%(image As Byte(), pos%, ParamArray values As UShort())
        For Each v In values
            pos = write8(image, pos, BitConverter.GetBytes(v))
        Next
        Return pos
    End Function

    Function write32%(image As Byte(), pos%, ParamArray values%())
        For Each v In values
            pos = write8(image, pos, BitConverter.GetBytes(v))
        Next
        Return pos
    End Function

    Function writestr%(image As Byte(), pos%, s$)
        Return write8(image, pos, Text.Encoding.UTF8.GetBytes(s))
    End Function
End Module
