Public Class IMAGE_DOS_HEADER
    Public e_magic, e_cblp, e_cp, e_crlc, e_cparhdr, e_minalloc, e_maxalloc,
           e_ss, e_sp, e_csum, e_ip, e_cs, e_lfarlc, e_ovno,
           e_res(3), e_oemid, e_oeminfo, e_res2(9) As UShort
    Public e_lfanew%
End Class

Public Class IMAGE_NT_HEADERS32
    Public Signature%
    Public FileHeader As New IMAGE_FILE_HEADER
    Public OptionalHeader As New IMAGE_OPTIONAL_HEADER32
End Class

Public Class IMAGE_FILE_HEADER
    Public Machine, NumberOfSections As UShort
    Public TimeDateStamp, PointerToSymbolTable, NumberOfSymbols As Integer
    Public SizeOfOptionalHeader, Characteristics As UShort
End Class

Public Class IMAGE_OPTIONAL_HEADER32
    Public Magic As UShort
    Public MajorLinkerVersion, MinorLinkerVersion As Byte
    Public SizeOfCode, SizeOfInitializedData, SizeOfUninitializedData,
           AddressOfEntryPoint, BaseOfCode, BaseOfData, ImageBase,
           SectionAlignment, FileAlignment As Integer
    Public MajorOperatingSystemVersion, MinorOperatingSystemVersion,
           MajorImageVersion, MinorImageVersion,
           MajorSubsystemVersion, MinorSubsystemVersion As UShort
    Public Win32VersionValue, SizeOfImage, SizeOfHeaders, CheckSum As Integer
    Public Subsystem, DllCharacteristics As UShort
    Public SizeOfStackReserve, SizeOfStackCommit,
           SizeOfHeapReserve, SizeOfHeapCommit,
           LoaderFlags, NumberOfRvaAndSizes As Integer
    Public DataDirectory(15) As IMAGE_DATA_DIRECTORY

    Public Sub New()
        For i = 0 To DataDirectory.Length - 1
            DataDirectory(i) = New IMAGE_DATA_DIRECTORY
        Next
    End Sub
End Class

Public Class IMAGE_DATA_DIRECTORY
    Public VirtualAddress, Size As Integer
End Class

Public Class IMAGE_SECTION_HEADER
    Public Name(7) As Byte
    Public VirtualSize, VirtualAddress, SizeOfRawData,
           PointerToRawData, PointerToRelocations, PointerToLinenumbers As Integer
    Public NumberOfRelocations, NumberOfLinenumbers As UShort
    Public Characteristics%
End Class

Public Class IMAGE_IMPORT_DESCRIPTOR
    Public OriginalFirstThunk, TimeDateStamp, ForwarderChain,
           Name, FirstThunk As Integer
End Class
