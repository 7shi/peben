Public Module Module5
    Function CreateIData(rva%, dlls As Dictionary(Of String, String()), result As Dictionary(Of String, Integer)) As Byte()
        Dim nsym = (From syms In dlls.Values Select syms.Length).Sum()
        Dim thunklen = 4 * (nsym + dlls.Count)
        Dim namelen = (From dll In dlls.Keys Select Align(dll.Length + 1, 4)).Sum()
        Dim symlen = (From syms In dlls.Values Select
                      (From sym In syms Select Align(sym.Length + 3, 2)).Sum()).Sum()
        Dim pthunk = 20 * (dlls.Count + 1), pname = pthunk + thunklen * 2,
            psym = pname + namelen, p = 0
        Dim ret(psym + symlen - 1) As Byte
        For Each dll In dlls
            writeFields(ret, p, New IMAGE_IMPORT_DESCRIPTOR With {
                .Name = rva + pname,
                .OriginalFirstThunk = rva + pthunk,
                .FirstThunk = rva + pthunk + thunklen})
            p += 20
            writestr(ret, pname, dll.Key)
            pname += Align(dll.Key.Length + 1, 4)
            For Each sym In dll.Value
                If result IsNot Nothing Then result(sym) = rva + pthunk + thunklen
                write32(ret, pthunk, rva + psym)
                write32(ret, pthunk + thunklen, rva + psym)
                pthunk += 4
                writestr(ret, psym + 2, sym)
                psym += Align(sym.Length + 3, 2)
            Next
            pthunk += 4
        Next
        Return ret
    End Function

    Function Align%(v%, a%)
        Return Int((v + a - 1) / a) * a
    End Function
End Module
