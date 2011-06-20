Imports PELib

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
            .NumberOfSections = 2
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
            .SizeOfImage = &H3000
            .SizeOfHeaders = &H200
            .Subsystem = 2
            .DllCharacteristics = &H8500
            .SizeOfStackReserve = &H100000
            .SizeOfStackCommit = &H1000
            .SizeOfHeapReserve = &H100000
            .SizeOfHeapCommit = &H1000
            .NumberOfRvaAndSizes = 16
        End With

        Dim text = New IMAGE_SECTION_HEADER With {
            .Name = getBytes(".text", 8),
            .VirtualSize = 1,
            .VirtualAddress = &H1000,
            .SizeOfRawData = &H200,
            .PointerToRawData = &H200,
            .Characteristics = &H60000020}
        writeFields(image, &H178, text)

        write8(image, &H200, &HC3)

        Dim idata_rva = &H2000
        Dim dlls = New Dictionary(Of String, String())
        dlls("test1.dll") = {"func1a", "func1b"}
        dlls("test2.dll") = {"func2a", "func2b"}
        Dim idata = CreateIData(idata_rva, dlls, Nothing)

        Dim idatalen = idata.Length
        ReDim Preserve idata(Align(idata.Length, peh.OptionalHeader.FileAlignment) - 1)
        With peh.OptionalHeader.DataDirectory(1)
            .VirtualAddress = idata_rva
            .Size = Align(idata.Length, peh.OptionalHeader.SectionAlignment)
        End With
        writeFields(image, &H1A0, New IMAGE_SECTION_HEADER With {
            .Name = getBytes(".idata", 8),
            .VirtualSize = idatalen,
            .VirtualAddress = idata_rva,
            .SizeOfRawData = idata.Length,
            .PointerToRawData = &H400,
            .Characteristics = &HC0300040})

        writeFields(image, &H80, peh)

        Using fs = New IO.FileStream("output.exe", IO.FileMode.Create)
            fs.Write(image, 0, image.Length)
            fs.Write(idata, 0, idata.Length)
        End Using
    End Sub
End Module
