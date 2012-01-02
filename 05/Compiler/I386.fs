module I386

open System
open System.Collections.Generic
open PELib.I386

type Compiler() =
    member x.Machine = 0x14Cus
    member x.Compile (lines:string list) (getaddr:string -> int) =
        let ret = new List<byte>()
        let asm = new Assembler(ret)
        for line in lines do
            let tokens = line.Split(' ')
            let getarg n = 0x402018 + ((int tokens.[n].[0]) - (int 'A')) * 4
            match tokens.[0] with
            | "LET" ->
                asm.Mov(eax, Convert.ToInt32 tokens.[2])
                asm.Mov([getarg 1], eax)
            | "ADD" ->
                asm.Mov(eax, [getarg 2])
                asm.Add([getarg 1], eax)
            | "DISP" ->
                asm.Push [getarg 1]
                asm.Push 0x402010
                asm.Push 0x402000
                asm.Call [getaddr "wsprintfA"]
                asm.Pop eax
                asm.Pop eax
                asm.Pop eax
                asm.Push 0
                asm.Push 0
                asm.Push 0x402000
                asm.Push 0
                asm.Call [getaddr "MessageBoxA"]
            | _ ->
                raise <| new Exception("error: " + tokens.[0])
        asm.Push 0
        asm.Call [getaddr "ExitProcess"]
        ret.ToArray()
