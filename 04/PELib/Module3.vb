Public Module Module3
    Function writeAny%(image As Byte(), pos%, v As Object)
        If TypeOf v Is Byte Then
            image(pos) = CByte(v)
            Return pos + 1
        ElseIf TypeOf v Is UShort Then
            Return write16(image, pos, CUShort(v))
        ElseIf TypeOf v Is Integer Then
            Return write32(image, pos, CInt(v))
        ElseIf TypeOf v Is Byte() Then
            Return write8(image, pos, CType(v, Byte()))
        ElseIf TypeOf v Is UShort() Then
            Return write16(image, pos, CType(v, UShort()))
        ElseIf TypeOf v Is Integer() Then
            Return write32(image, pos, CType(v, Integer()))
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
        For Each f In obj.GetType.GetFields
            pos = writeAny(image, pos, f.GetValue(obj))
        Next
        Return pos
    End Function
End Module
