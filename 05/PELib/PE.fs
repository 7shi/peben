module PELib.PE

open System
open System.Collections.Generic
open PELib.Headers
open PELib.Utils

let createIData rva (dlls:Dictionary<string, string list>) (result:Dictionary<string, int>) =
    let nsym = Seq.sum (seq { for syms in dlls.Values -> syms.Length })
    let thunklen = 4 * (nsym + dlls.Count)
    let namelen = Seq.sum (seq { for dll in dlls.Keys -> align (dll.Length + 1) 4 })
    let symlen = Seq.sum (seq {
        for syms in dlls.Values -> Seq.sum (seq {
            for sym in syms -> align (sym.Length + 3) 2 }) })
    let mutable pthunk = 20 * (dlls.Count + 1)
    let mutable pname = pthunk + thunklen * 2
    let mutable psym = pname + namelen
    let mutable p = 0
    let ret = Array.zeroCreate<byte>(psym + symlen)
    for dll in dlls do
        p <- writeFields ret p {
            OriginalFirstThunk = rva + pthunk
            TimeDateStamp      = 0
            ForwarderChain     = 0
            Name               = rva + pname
            FirstThunk         = rva + pthunk + thunklen }
        ignore <| writestr ret pname dll.Key
        pname <- pname + align (dll.Key.Length + 1) 4
        for sym in dll.Value do
            if result <> null then result.[sym] <- rva + pthunk + thunklen
            ignore <| write32 ret pthunk [| (rva + psym) |]
            ignore <| write32 ret (pthunk + thunklen) [| (rva + psym) |]
            pthunk <- pthunk + 4
            ignore <| writestr ret (psym + 2) sym
            psym <- psym + align (sym.Length + 3) 2
        pthunk <- pthunk + 4
    ret
