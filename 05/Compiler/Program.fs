open System
open System.Collections.Generic
open System.IO
open System.Text
open PELib.Headers
open PELib.Utils
open PELib.PE

let source = [
    "LET A 1"
    "LET B 2"
    "ADD A B"
    "DISP A" ]

let compiler = new I386.Compiler()
let image = Array.zeroCreate<byte> 0x200

ignore <| writeFields image 0 {
    e_magic     = conv16 "MZ"
    e_cblp      = 0x90us
    e_cp        = 3us
    e_crlc      = 0us
    e_cparhdr   = 4us
    e_minalloc  = 0us
    e_maxalloc  = 0xFFFFus
    e_ss        = 0us
    e_sp        = 0xB8us
    e_csum      = 0us
    e_ip        = 0us
    e_cs        = 0us
    e_lfarlc    = 0x40us
    e_ovno      = 0us
    e_res       = [| 0us; 0us; 0us; 0us |]
    e_oemid     = 0us
    e_oeminfo   = 0us
    e_res2      = [| 0us; 0us; 0us; 0us; 0us; 0us; 0us; 0us; 0us; 0us |]
    e_lfanew    = 0x80 }
ignore << write8 image 0x40 << PELib.I8086.write <| fun asm ->
    asm.mov(PELib.I8086.ax, 0x4C01us)
    asm.int 0x21uy

let peh = {
    Signature = conv32 "PE"
    FileHeader =
      { Machine              = compiler.Machine
        NumberOfSections     = 3us
        TimeDateStamp        = 0
        PointerToSymbolTable = 0
        NumberOfSymbols      = 0
        SizeOfOptionalHeader = 0xE0us
        Characteristics      = 0x102us }
    OptionalHeader =
      { Magic                       = 0x10Bus
        MajorLinkerVersion          = 10uy
        MinorLinkerVersion          = 0uy
        SizeOfCode                  = 0x200
        SizeOfInitializedData       = 0
        SizeOfUninitializedData     = 0
        AddressOfEntryPoint         = 0x1000
        BaseOfCode                  = 0x1000
        BaseOfData                  = 0x2000
        ImageBase                   = 0x400000
        SectionAlignment            = 0x1000
        FileAlignment               = 0x200
        MajorOperatingSystemVersion = 4us
        MinorOperatingSystemVersion = 0us
        MajorImageVersion           = 0us
        MinorImageVersion           = 0us
        MajorSubsystemVersion       = 4us
        MinorSubsystemVersion       = 0us
        Win32VersionValue           = 0
        SizeOfImage                 = 0x4000
        SizeOfHeaders               = 0x200
        CheckSum                    = 0
        Subsystem                   = 2us
        DllCharacteristics          = 0us
        SizeOfStackReserve          = 0x100000
        SizeOfStackCommit           = 0x1000
        SizeOfHeapReserve           = 0x100000
        SizeOfHeapCommit            = 0x1000
        LoaderFlags                 = 0
        NumberOfRvaAndSizes         = 16
        DataDirectory               =
          [| { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 }
             { VirtualAddress = 0; Size = 0 } |] } }

let idata_rva = 0x3000
let dlls = new Dictionary<string, string list>()
dlls.["kernel32.dll"] <- [ "ExitProcess" ]
dlls.["user32.dll"  ] <- [ "MessageBoxA"; "wsprintfA" ]
let imports = new Dictionary<string, int>()
let mutable idata = createIData idata_rva dlls imports

let mutable text = compiler.Compile source <| fun s ->
    peh.OptionalHeader.ImageBase + imports.[s]
let textlen = text.Length
text <- resizeArray text (align text.Length peh.OptionalHeader.FileAlignment)

let mutable psct = 0x178
psct <- writeFields image psct {
    Name                 = getBytes ".text" 8
    VirtualSize          = textlen
    VirtualAddress       = 0x1000
    SizeOfRawData        = text.Length
    PointerToRawData     = 0x200
    PointerToRelocations = 0
    PointerToLinenumbers = 0
    NumberOfRelocations  = 0us
    NumberOfLinenumbers  = 0us
    Characteristics      = 0x60000020 }

let mutable data = Array.zeroCreate<byte>(16 + 8 + 4 * 26)
ignore <| writestr data 16 "%d"
let datalen = data.Length
data <- resizeArray data (align data.Length peh.OptionalHeader.FileAlignment)

psct <- writeFields image psct {
    Name                 = getBytes ".data" 8
    VirtualSize          = datalen
    VirtualAddress       = 0x2000
    SizeOfRawData        = data.Length
    PointerToRawData     = 0x400
    PointerToRelocations = 0
    PointerToLinenumbers = 0
    NumberOfRelocations  = 0us
    NumberOfLinenumbers  = 0us
    Characteristics      = 0xC0000040 }

let idatalen = idata.Length
idata <- resizeArray idata (align idata.Length peh.OptionalHeader.FileAlignment)
peh.OptionalHeader.DataDirectory.[1] <- {
    VirtualAddress = idata_rva
    Size = align idata.Length peh.OptionalHeader.SectionAlignment }

psct <- writeFields image psct {
    Name                 = getBytes ".idata" 8
    VirtualSize          = idatalen
    VirtualAddress       = idata_rva
    SizeOfRawData        = idata.Length
    PointerToRawData     = 0x600
    PointerToRelocations = 0
    PointerToLinenumbers = 0
    NumberOfRelocations  = 0us
    NumberOfLinenumbers  = 0us
    Characteristics      = 0xC0300040 }

ignore <| writeFields image 0x80 peh

using (new FileStream("output.exe", FileMode.Create)) <| fun fs ->
    fs.Write(image, 0, image.Length)
    fs.Write(text, 0, text.Length)
    fs.Write(data, 0, data.Length)
    fs.Write(idata, 0, idata.Length)
