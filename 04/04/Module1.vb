Imports System.IO
Imports System.Text

Module Module1
    Dim image(&H3FF) As Byte

    Sub write16(pos%, v As UShort)
        image(pos) = v And &HFF
        image(pos + 1) = v >> 8
    End Sub

    Function write16s%(pos%, ParamArray vs As UShort())
        For Each v In vs
            write16(pos, v)
            pos += 2
        Next
        Return pos
    End Function

    Sub write32(pos%, v%)
        image(pos) = v And &HFF
        image(pos + 1) = (v >> 8) And &HFF
        image(pos + 2) = (v >> 16) And &HFF
        image(pos + 3) = (v >> 24) And &HFF
    End Sub

    Sub writebin(pos%, bin As Byte())
        Array.Copy(bin, 0, image, pos, bin.Length)
    End Sub

    Sub writestr(pos%, s$)
        writebin(pos, Encoding.UTF8.GetBytes(s))
    End Sub

    Function conv16(s$) As UShort
        Dim bin = Encoding.UTF8.GetBytes(s)
        ReDim Preserve bin(1)
        Return BitConverter.ToUInt16(bin, 0)
    End Function

    Sub Main()
        Dim dosh = New IMAGE_DOS_HEADER With {
            .e_magic = conv16("MZ"),
            .e_cblp = &H90,
            .e_cp = 3,
            .e_cparhdr = 4,
            .e_maxalloc = &HFFFF,
            .e_sp = &HB8,
            .e_lfarlc = &H40,
            .e_lfanew = &H80}
        dosh.write(0)
        writebin(&H40, {&HB8, 1, &H4C, &HCD, &H21})

        ' PE header
        writestr(&H80, "PE")
        write16(&H84, &H14C)
        write16(&H86, 1)
        write16(&H94, &HE0)
        write16(&H96, &H102)
        write16(&H98, &H10B)
        image(&H9A) = 10
        write32(&H9C, &H200)
        write32(&HA8, &H1000)
        write32(&HAC, &H1000)
        write32(&HB0, &H2000)
        write32(&HB4, &H400000)
        write32(&HB8, &H1000)
        write32(&HBC, &H200)
        write16(&HC0, 5)
        write16(&HC2, 1)
        write16(&HC8, 5)
        write16(&HCA, 1)
        write32(&HD0, &H2000)
        write32(&HD4, &H200)
        write16(&HDC, 2)
        write16(&HDE, &H8540)
        write32(&HE0, &H100000)
        write32(&HE4, &H1000)
        write32(&HE8, &H100000)
        write32(&HEC, &H1000)
        write32(&HF5, &H10)

        ' section header
        writestr(&H178, ".text")
        write32(&H180, 1)
        write32(&H184, &H1000)
        write32(&H188, &H200)
        write32(&H18C, &H200)
        write32(&H19C, &H60000020)

        ' .text
        image(&H200) = &HC3

        Using fs = New FileStream("output.exe", FileMode.Create)
            fs.Write(image, 0, image.Length)
        End Using
    End Sub
End Module
