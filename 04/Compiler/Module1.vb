Imports PELib

Module Module1
    Dim MessageBoxA%, wsprintfA%

    Sub Main()
        Dim source = {"LET A 1",
                      "LET B 2",
                      "ADD A B",
                      "DISP A"}

        Dim image(&H1FF) As Byte

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
            .NumberOfSections = 3
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
            .SizeOfImage = &H4000
            .SizeOfHeaders = &H200
            .Subsystem = 2
            .DllCharacteristics = &H8500
            .SizeOfStackReserve = &H100000
            .SizeOfStackCommit = &H1000
            .SizeOfHeapReserve = &H100000
            .SizeOfHeapCommit = &H1000
            .NumberOfRvaAndSizes = 16
        End With

        Dim idata_rva = &H3000
        Dim dlls = New Dictionary(Of String, String())
        dlls("user32.dll") = {"MessageBoxA", "wsprintfA"}
        Dim result = New Dictionary(Of String, Integer)
        Dim idata = CreateIData(idata_rva, dlls, result)
        MessageBoxA = peh.OptionalHeader.ImageBase + result("MessageBoxA")
        wsprintfA = peh.OptionalHeader.ImageBase + result("wsprintfA")

        Dim text = Compile(source)
        Dim textlen = text.Length
        ReDim Preserve text(Align(text.Length, peh.OptionalHeader.FileAlignment) - 1)

        Dim psct = &H178
        psct = writeFields(image, psct, New IMAGE_SECTION_HEADER With {
            .Name = getBytes(".text", 8),
            .VirtualSize = textlen,
            .VirtualAddress = &H1000,
            .SizeOfRawData = text.Length,
            .PointerToRawData = &H200,
            .Characteristics = &H60000020})

        Dim data(16 + 8 + 4 * 26) As Byte
        writestr(data, 16, "%d")
        Dim datalen = data.Length
        ReDim Preserve data(Align(data.Length, peh.OptionalHeader.FileAlignment) - 1)

        psct = writeFields(image, psct, New IMAGE_SECTION_HEADER With {
            .Name = getBytes(".data", 8),
            .VirtualSize = datalen,
            .VirtualAddress = &H2000,
            .SizeOfRawData = data.Length,
            .PointerToRawData = &H400,
            .Characteristics = &HC0000040})
        ReDim Preserve data(Align(data.Length, peh.OptionalHeader.FileAlignment) - 1)

        Dim idatalen = idata.Length
        ReDim Preserve idata(Align(idata.Length, peh.OptionalHeader.FileAlignment) - 1)
        With peh.OptionalHeader.DataDirectory(1)
            .VirtualAddress = idata_rva
            .Size = Align(idata.Length, peh.OptionalHeader.SectionAlignment)
        End With
        psct = writeFields(image, psct, New IMAGE_SECTION_HEADER With {
            .Name = getBytes(".idata", 8),
            .VirtualSize = idatalen,
            .VirtualAddress = idata_rva,
            .SizeOfRawData = idata.Length,
            .PointerToRawData = &H600,
            .Characteristics = &HC0300040})

        writeFields(image, &H80, peh)

        Using fs = New IO.FileStream("output.exe", IO.FileMode.Create)
            fs.Write(image, 0, image.Length)
            fs.Write(text, 0, text.Length)
            fs.Write(data, 0, data.Length)
            fs.Write(idata, 0, idata.Length)
        End Using
    End Sub

    Function Compile(lines As String()) As Byte()
        Dim ret = New List(Of Byte)
        For Each line In lines
            Dim tokens = line.Split(" ")
            Select Case tokens(0)
                Case "LET"
                    ret.Add(&HB8)
                    ret.AddRange(BitConverter.GetBytes(Convert.ToInt32(tokens(2))))
                    ret.Add(&HA3)
                    ret.AddRange(BitConverter.GetBytes(&H402018 + (Asc(tokens(1)) - Asc("A")) * 4))
                Case "ADD"
                    ret.Add(&HA1)
                    ret.AddRange(BitConverter.GetBytes(&H402018 + (Asc(tokens(2)) - Asc("A")) * 4))
                    ret.AddRange({1, 5})
                    ret.AddRange(BitConverter.GetBytes(&H402018 + (Asc(tokens(1)) - Asc("A")) * 4))
                Case "DISP"
                    ret.AddRange({&HFF, &H35})
                    ret.AddRange(BitConverter.GetBytes(&H402018 + (Asc(tokens(1)) - Asc("A")) * 4))
                    ret.Add(&H68)
                    ret.AddRange(BitConverter.GetBytes(&H402010))
                    ret.Add(&H68)
                    ret.AddRange(BitConverter.GetBytes(&H402000))
                    ret.AddRange({&HFF, &H15})
                    ret.AddRange(BitConverter.GetBytes(wsprintfA))
                    ret.AddRange({&H58, &H58, &H58})
                    ret.Add(&H68)
                    ret.AddRange(BitConverter.GetBytes(0))
                    ret.Add(&H68)
                    ret.AddRange(BitConverter.GetBytes(0))
                    ret.Add(&H68)
                    ret.AddRange(BitConverter.GetBytes(&H402000))
                    ret.Add(&H68)
                    ret.AddRange(BitConverter.GetBytes(0))
                    ret.AddRange({&HFF, &H15})
                    ret.AddRange(BitConverter.GetBytes(MessageBoxA))
                Case Else
                    Throw New Exception("error: " + tokens(0))
            End Select
        Next
        ret.Add(&HC3)
        Return ret.ToArray()
    End Function
End Module
