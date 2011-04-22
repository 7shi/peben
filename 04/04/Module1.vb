Imports System.IO

Module Module1
    Sub Main()
        Dim image(&H3FF) As Byte

        Dim dosh = New IMAGE_DOS_HEADER With {
            .e_magic = conv16("MZ"),
            .e_cblp = &H90,
            .e_cp = 3,
            .e_cparhdr = 4,
            .e_maxalloc = &HFFFF,
            .e_sp = &HB8,
            .e_lfarlc = &H40,
            .e_lfanew = &H80}
        writeFields(image, 0, dosh)
        write8s(image, &H40, &HB8, 1, &H4C, &HCD, &H21)

        ' PE header
        writestr(image, &H80, "PE")
        Dim fh = New IMAGE_FILE_HEADER With {
            .Machine = &H14C,
            .NumberOfSections = 1,
            .SizeOfOptionalHeader = &HE0,
            .Characteristics = &H102}
        writeFields(image, &H84, fh)
        write16(image, &H98, &H10B)
        image(&H9A) = 10
        write32(image, &H9C, &H200)
        write32(image, &HA8, &H1000)
        write32(image, &HAC, &H1000)
        write32(image, &HB0, &H2000)
        write32(image, &HB4, &H400000)
        write32(image, &HB8, &H1000)
        write32(image, &HBC, &H200)
        write16(image, &HC0, 5)
        write16(image, &HC2, 1)
        write16(image, &HC8, 5)
        write16(image, &HCA, 1)
        write32(image, &HD0, &H2000)
        write32(image, &HD4, &H200)
        write16(image, &HDC, 2)
        write16(image, &HDE, &H8540)
        write32(image, &HE0, &H100000)
        write32(image, &HE4, &H1000)
        write32(image, &HE8, &H100000)
        write32(image, &HEC, &H1000)
        write32(image, &HF5, &H10)

        ' section header
        writestr(image, &H178, ".text")
        write32(image, &H180, 1)
        write32(image, &H184, &H1000)
        write32(image, &H188, &H200)
        write32(image, &H18C, &H200)
        write32(image, &H19C, &H60000020)

        ' .text
        image(&H200) = &HC3

        Using fs = New FileStream("output.exe", FileMode.Create)
            fs.Write(image, 0, image.Length)
        End Using
    End Sub
End Module
