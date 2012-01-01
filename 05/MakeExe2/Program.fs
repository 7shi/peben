open System
open System.IO
open System.Text
open PELib.Headers
open PELib.Utils
open PELib.PE

let image = Array.zeroCreate<byte> 0x400

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
ignore <| write8 image 0x40 [| 0xB8uy; 1uy; 0x4Cuy; 0xCDuy; 0x21uy |]

ignore <| writeFields image 0x80 {
    Signature = conv32 "PE"
    FileHeader =
      { Machine              = 0x14Cus
        NumberOfSections     = 1us
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
        SizeOfImage                 = 0x2000
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

ignore <| writeFields image 0x178 {
    Name                 = getBytes ".text" 8
    VirtualSize          = 1
    VirtualAddress       = 0x1000
    SizeOfRawData        = 0x200
    PointerToRawData     = 0x200
    PointerToRelocations = 0
    PointerToLinenumbers = 0
    NumberOfRelocations  = 0us
    NumberOfLinenumbers  = 0us
    Characteristics      = 0x60000020 }

ignore <| write8 image 0x200 [| 0xC3uy |]

using (new FileStream("output.exe", FileMode.Create)) <| fun fs ->
    fs.Write(image, 0, image.Length)
