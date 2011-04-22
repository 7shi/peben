Public Class IMAGE_DOS_HEADER
    Public e_magic, e_cblp, e_cp, e_crlc, e_cparhdr, e_minalloc, e_maxalloc,
        e_ss, e_sp, e_csum, e_ip, e_cs, e_lfarlc, e_ovno,
        e_res(3), e_oemid, e_oeminfo, e_res2(9) As UShort
    Public e_lfanew%
End Class

Public Class IMAGE_FILE_HEADER
    Public Machine, NumberOfSections As UShort
    Public TimeDateStamp, PointerToSymbolTable, NumberOfSymbols As Integer
    Public SizeOfOptionalHeader, Characteristics As UShort
End Class
