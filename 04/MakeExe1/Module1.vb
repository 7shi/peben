Imports PELib

Module Module1
    Sub Main()
        Dim image(&H3FF) As Byte

        ' dos header
        writestr(image, 0, "MZ")
        write16(image, 2, &H90, 3, 0, 4, 0, &HFFFF)
        write16(image, &H10, &HB8)
        write16(image, &H18, &H40)
        write32(image, &H3C, &H80)

        ' dos binary
        write8(image, &H40, &HB8, 1, &H4C, &HCD, &H21)

        ' PE header
        writestr(image, &H80, "PE")
        write16(image, &H84, &H14C, 1)
        write16(image, &H94, &HE0, &H102, &H10B)
        write8(image, &H9A, 10, 0)
        write32(image, &H9C, &H200)
        write32(image, &HA8, &H1000, &H1000)
        write32(image, &HB0, &H2000, &H400000)
        write32(image, &HB8, &H1000, &H200)
        write16(image, &HC0, 5, 1)
        write16(image, &HC8, 5, 1)
        write32(image, &HD0, &H2000, &H200)
        write16(image, &HDC, 2, &H8500)
        write32(image, &HE0, &H100000, &H1000)
        write32(image, &HE8, &H100000, &H1000)
        write32(image, &HF4, &H10)

        ' section header
        writestr(image, &H178, ".text")
        write32(image, &H180, 1, &H1000)
        write32(image, &H188, &H200, &H200)
        write32(image, &H19C, &H60000020)

        ' .text
        write8(image, &H200, &HC3)

        ' ファイル出力
        Using fs = New IO.FileStream("output.exe", IO.FileMode.Create)
            fs.Write(image, 0, image.Length)
        End Using
    End Sub
End Module
