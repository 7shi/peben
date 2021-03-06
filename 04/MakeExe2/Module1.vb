﻿Imports PELib

Module Module1
    Sub Main()
        Dim image(&H3FF) As Byte

        writeFields(image, 0, New IMAGE_DOS_HEADER With {
            .e_magic = conv16("MZ"),
            .e_cblp = &H90,
            .e_cp = 3,
            .e_cparhdr = 4,
            .e_maxalloc = &HFFFF,
            .e_sp = &HB8,
            .e_lfarlc = &H40,
            .e_lfanew = &H80})
        write8(image, &H40, &HB8, 1, &H4C, &HCD, &H21)

        Dim peh = New IMAGE_NT_HEADERS32 With {
            .Signature = conv32("PE")}
        With peh.FileHeader
            .Machine = &H14C
            .NumberOfSections = 1
            .SizeOfOptionalHeader = &HE0
            .Characteristics = &H102
        End With
        With peh.OptionalHeader
            .Magic = &H10B
            .MajorLinkerVersion = 10
            .SizeOfCode = &H200
            .AddressOfEntryPoint = &H1000
            .BaseOfCode = &H1000
            .BaseOfData = &H2000
            .ImageBase = &H400000
            .SectionAlignment = &H1000
            .FileAlignment = &H200
            .MajorOperatingSystemVersion = 5
            .MinorOperatingSystemVersion = 1
            .MajorSubsystemVersion = 5
            .MinorSubsystemVersion = 1
            .SizeOfImage = &H2000
            .SizeOfHeaders = &H200
            .Subsystem = 2
            .DllCharacteristics = &H8500
            .SizeOfStackReserve = &H100000
            .SizeOfStackCommit = &H1000
            .SizeOfHeapReserve = &H100000
            .SizeOfHeapCommit = &H1000
            .NumberOfRvaAndSizes = 16
        End With
        writeFields(image, &H80, peh)

        Dim text = New IMAGE_SECTION_HEADER With {
            .Name = getBytes(".text", 8),
            .VirtualSize = 1,
            .VirtualAddress = &H1000,
            .SizeOfRawData = &H200,
            .PointerToRawData = &H200,
            .Characteristics = &H60000020}
        writeFields(image, &H178, text)

        write8(image, &H200, &HC3)

        Using fs = New IO.FileStream("output.exe", IO.FileMode.Create)
            fs.Write(image, 0, image.Length)
        End Using
    End Sub
End Module
