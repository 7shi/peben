module I386

open System
open System.Collections.Generic

type Reg32 =
    | eax = 0
    | ecx = 1
    | edx = 2
    | ebx = 3
    | esp = 4
    | ebp = 5
    | esi = 6
    | edi = 7

let movr (list:List<byte>) (r:Reg32) (v:int32) =
    list.Add (0xB8uy + (byte r))
    list.AddRange(BitConverter.GetBytes v)

let movar (list:List<byte>) (a:int32) (r:Reg32) =
    if r = Reg32.eax then
        list.Add 0xA3uy
    else
        list.AddRange [ 0x89uy; 5uy + ((byte r) <<< 5) ]
    list.AddRange(BitConverter.GetBytes a)

let movra (list:List<byte>) (r:Reg32) (a:int32) =
    if r = Reg32.eax then
        list.Add 0xA1uy
    else
        list.AddRange [ 0x8Buy; 5uy + ((byte r) <<< 5) ]
    list.AddRange(BitConverter.GetBytes a)

let addar (list:List<byte>) (a:int32) (r:Reg32) =
    list.AddRange [ 1uy; 5uy + ((byte r) <<< 5) ]
    list.AddRange(BitConverter.GetBytes a)

let push (list:List<byte>) (r:Reg32) =
    list.Add (0x50uy + (byte r))

let pop (list:List<byte>) (r:Reg32) =
    list.Add (0x58uy + (byte r))

let pushd (list:List<byte>) (v:int) =
    list.Add 0x68uy
    list.AddRange(BitConverter.GetBytes v)

let pusha (list:List<byte>) (a:int) = 
    list.AddRange [ 0xFFuy; 0x35uy ]
    list.AddRange(BitConverter.GetBytes a)

let calla (list:List<byte>) (a:int) =
    list.AddRange [ 0xFFuy; 0x15uy ]
    list.AddRange(BitConverter.GetBytes a)
