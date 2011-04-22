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

    Function write8s%(image As Byte(), pos%, ParamArray bin As Byte())
        Array.Copy(bin, 0, image, pos, bin.Length)
        Return pos + bin.Length
    End Function

    Function writestr%(image As Byte(), pos%, s$)
        Return write8s(image, pos, Encoding.UTF8.GetBytes(s))
    End Function

    Function conv16(s$) As UShort
        Dim bin = Encoding.UTF8.GetBytes(s)
        ReDim Preserve bin(1)
        Return BitConverter.ToUInt16(bin, 0)
    End Function

    Function writeAny%(image As Byte(), pos%, v As Object)
        If TypeOf v Is Byte Then
            image(pos) = CByte(v)
            Return pos + 1
        ElseIf TypeOf v Is UShort Then
            Return write16(image, pos, CUShort(v))
        ElseIf TypeOf v Is Integer Then
            Return write32(image, pos, CInt(v))
        ElseIf TypeOf v Is Byte() Then
            Return write8s(image, pos, CType(v, Byte()))
        ElseIf TypeOf v Is UShort() Then
            Return write16s(image, pos, CType(v, UShort()))
        ElseIf TypeOf v Is Integer() Then
            Return write32s(image, pos, CType(v, Integer()))
        Else
            Dim t = v.GetType
            If t.IsArray Then
                For Each obj In CType(v, Array)
                    pos = writeAny(image, pos, obj)
                Next
                Return pos
            Else
                Dim fs = t.GetFields
                If fs.Length > 0 Then
                    Return writeFields(image, pos, v)
                Else
                    Throw New ArgumentException("不明な型: ", t.FullName)
                End If
            End If
        End If
    End Function

    Function writeFields%(image As Byte(), pos%, obj As Object)
        For Each f In obj.GetType.GetFields()
            pos = writeAny(image, pos, f.GetValue(obj))
        Next
        Return pos
    End Function
End Module
