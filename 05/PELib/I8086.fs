module PELib.I8086

open System
open System.Collections.Generic

type Reg16(r:byte) =
    member x.Value = r
    override x.GetHashCode () = int r
    override x.Equals (obj:obj) =
        obj :? Reg16 && x.Value = (obj :?> Reg16).Value

let ax = Reg16 0uy
let cx = Reg16 1uy
let dx = Reg16 2uy
let bx = Reg16 3uy
let sp = Reg16 4uy
let bp = Reg16 5uy
let si = Reg16 6uy
let di = Reg16 7uy

type Assembler(list:List<byte>) =
    member x.mov (r:Reg16, v:uint16) =
        list.Add (0xB8uy + r.Value)
        list.AddRange(BitConverter.GetBytes v)

    member x.int (v:byte) =
        list.AddRange [ 0xCDuy; v ]

let write (f:Assembler -> unit) =
    let list = new List<byte>()
    f(new Assembler(list))
    list.ToArray()
