module PELib.I386

open System
open System.Collections.Generic

type Reg32(r:byte) =
    member x.Value = r
    override x.GetHashCode () = int r
    override x.Equals (obj:obj) =
        obj :? Reg32 && x.Value = (obj :?> Reg32).Value

let eax = Reg32 0uy
let ecx = Reg32 1uy
let edx = Reg32 2uy
let ebx = Reg32 3uy
let esp = Reg32 4uy
let ebp = Reg32 5uy
let esi = Reg32 6uy
let edi = Reg32 7uy

type Assembler(list:List<byte>) =
    member x.mov (r:Reg32, v:int32) =
        list.Add (0xB8uy + r.Value)
        list.AddRange(BitConverter.GetBytes v)

    member x.mov (a:int32 list, r:Reg32) =
        if r = eax then
            list.Add 0xA3uy
        else
            list.AddRange [ 0x89uy; 5uy + (r.Value <<< 5) ]
        list.AddRange(BitConverter.GetBytes a.[0])

    member x.mov (r:Reg32, a:int32 list) =
        if r = eax then
            list.Add 0xA1uy
        else
            list.AddRange [ 0x8Buy; 5uy + (r.Value <<< 5) ]
        list.AddRange(BitConverter.GetBytes a.[0])

    member x.add (a:int32 list, r:Reg32) =
        list.AddRange [ 1uy; 5uy + (r.Value <<< 5) ]
        list.AddRange(BitConverter.GetBytes a.[0])

    member x.push (r:Reg32) =
        list.Add (0x50uy + r.Value)

    member x.pop (r:Reg32) =
        list.Add (0x58uy + r.Value)

    member x.push (v:int) =
        list.Add 0x68uy
        list.AddRange(BitConverter.GetBytes v)

    member x.push (a:int list) = 
        list.AddRange [ 0xFFuy; 0x35uy ]
        list.AddRange(BitConverter.GetBytes a.[0])

    member x.call (a:int list) =
        list.AddRange [ 0xFFuy; 0x15uy ]
        list.AddRange(BitConverter.GetBytes a.[0])

    member x.ret () =
        list.Add 0xC3uy
