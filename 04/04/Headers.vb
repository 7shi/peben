Public Class IMAGE_DOS_HEADER
    Public e_magic, e_cblp, e_cp, e_crlc, e_cparhdr, e_minalloc, e_maxalloc,
        e_ss, e_sp, e_csum, e_ip, e_cs, e_lfarlc, e_ovno,
        e_res(3), e_oemid, e_oeminfo, e_res2(9) As UShort
    Public e_lfanew%

    Sub write(image As Byte(), pos%)
        pos = write16s(image, pos, e_magic, e_cblp, e_cp, e_crlc, e_cparhdr, e_minalloc,
                        e_maxalloc, e_ss, e_sp, e_csum, e_ip, e_cs, e_lfarlc, e_ovno)
        pos = write16s(image, pos, e_res)
        pos = write16s(image, pos, e_oemid, e_oeminfo)
        pos = write16s(image, pos, e_res2)
        write32(image, pos, e_lfanew)
    End Sub
End Class
