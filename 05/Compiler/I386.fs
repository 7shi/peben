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
                asm.mov(eax, Convert.ToInt32 tokens.[2])
                asm.mov([getarg 1], eax)
            | "ADD" ->
                asm.mov(eax, [getarg 2])
                asm.add([getarg 1], eax)
            | "DISP" ->
                asm.push [getarg 1]
                asm.push 0x402010
                asm.push 0x402000
                asm.call [getaddr "wsprintfA"]
                asm.pop eax
                asm.pop eax
                asm.pop eax
                asm.push 0
                asm.push 0
                asm.push 0x402000
                asm.push 0
                asm.call [getaddr "MessageBoxA"]
            | _ ->
                raise <| new Exception("error: " + tokens.[0])
        asm.push 0
        asm.call [getaddr "ExitProcess"]
        ret.ToArray()
