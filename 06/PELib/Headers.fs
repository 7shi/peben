namespace PELib.Headers

type IMAGE_DOS_HEADER = {
    mutable e_magic     : uint16
    mutable e_cblp      : uint16
    mutable e_cp        : uint16
    mutable e_crlc      : uint16
    mutable e_cparhdr   : uint16
    mutable e_minalloc  : uint16
    mutable e_maxalloc  : uint16
    mutable e_ss        : uint16
    mutable e_sp        : uint16
    mutable e_csum      : uint16
    mutable e_ip        : uint16
    mutable e_cs        : uint16
    mutable e_lfarlc    : uint16
    mutable e_ovno      : uint16
    mutable e_res       : uint16[(*4*)]
    mutable e_oemid     : uint16
    mutable e_oeminfo   : uint16
    mutable e_res2      : uint16[(*10*)]
    mutable e_lfanew    : int }


type IMAGE_NT_HEADERS32 = {
    mutable Signature       : int
    mutable FileHeader      : IMAGE_FILE_HEADER
    mutable OptionalHeader  : IMAGE_OPTIONAL_HEADER32 }

and IMAGE_FILE_HEADER = {
    mutable Machine                 : uint16
    mutable NumberOfSections        : uint16
    mutable TimeDateStamp           : int
    mutable PointerToSymbolTable    : int
    mutable NumberOfSymbols         : int
    mutable SizeOfOptionalHeader    : uint16
    mutable Characteristics         : uint16 }

and IMAGE_OPTIONAL_HEADER32 = {
    mutable Magic                       : uint16
    mutable MajorLinkerVersion          : byte
    mutable MinorLinkerVersion          : byte
    mutable SizeOfCode                  : int
    mutable SizeOfInitializedData       : int
    mutable SizeOfUninitializedData     : int
    mutable AddressOfEntryPoint         : int
    mutable BaseOfCode                  : int
    mutable BaseOfData                  : int
    mutable ImageBase                   : int
    mutable SectionAlignment            : int
    mutable FileAlignment               : int
    mutable MajorOperatingSystemVersion : uint16
    mutable MinorOperatingSystemVersion : uint16
    mutable MajorImageVersion           : uint16
    mutable MinorImageVersion           : uint16
    mutable MajorSubsystemVersion       : uint16
    mutable MinorSubsystemVersion       : uint16
    mutable Win32VersionValue           : int
    mutable SizeOfImage                 : int
    mutable SizeOfHeaders               : int
    mutable CheckSum                    : int
    mutable Subsystem                   : uint16
    mutable DllCharacteristics          : uint16
    mutable SizeOfStackReserve          : int
    mutable SizeOfStackCommit           : int
    mutable SizeOfHeapReserve           : int
    mutable SizeOfHeapCommit            : int
    mutable LoaderFlags                 : int
    mutable NumberOfRvaAndSizes         : int
    mutable DataDirectory               : IMAGE_DATA_DIRECTORY[(*16*)] }

and IMAGE_DATA_DIRECTORY = {
    mutable VirtualAddress  : int
    mutable Size            : int }

type IMAGE_SECTION_HEADER = {
    mutable Name                    : byte[(*8*)]
    mutable VirtualSize             : int
    mutable VirtualAddress          : int
    mutable SizeOfRawData           : int
    mutable PointerToRawData        : int
    mutable PointerToRelocations    : int
    mutable PointerToLinenumbers    : int
    mutable NumberOfRelocations     : uint16
    mutable NumberOfLinenumbers     : uint16
    mutable Characteristics         : int }

type IMAGE_IMPORT_DESCRIPTOR = {
    mutable OriginalFirstThunk  : int
    mutable TimeDateStamp       : int
    mutable ForwarderChain      : int
    mutable Name                : int
    mutable FirstThunk          : int }
