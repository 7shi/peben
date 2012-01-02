module MIPS32el

open System
open System.Collections.Generic
open PELib.MIPS32

type Compiler() =
    member x.Machine = 0x166us
    member x.Compile (lines:string list) (getaddr:string -> int) =
        let ret = new List<byte>()
        let asm = new Assembler(ret, true)
        let call name =
            asm.La at (getaddr name)
            asm.Lw at 0(at)
            asm.Nop // load delay slot
            asm.Jalr ra at
            asm.Nop // branch delay slot
        for line in lines do
            let tokens = line.Split(' ')
            let getarg n = 0x402018 + ((int tokens.[n].[0]) - (int 'A')) * 4
            match tokens.[0] with
            | "LET" ->
                asm.La at (getarg 1)
                asm.Li v0 (Convert.ToInt32 tokens.[2])
                asm.Sw v0 0(at)
            | "ADD" ->
                asm.La at (getarg 2)
                asm.Lw v1 0(at)
                asm.La at (getarg 1)
                asm.Lw v0 0(at)
                asm.Nop // load delay slot
                asm.Add v0 v0 v1
                asm.Sw v0 0(at)
            | "DISP" ->
                asm.La at (getarg 1)
                asm.Lw a2 0(at)
                asm.La a1 0x402010
                asm.La a0 0x402000
                call "wsprintfA"
                asm.Li a3 0
                asm.Li a2 0
                asm.La a1 0x402000
                asm.Li a0 0
                call "MessageBoxA"
            | _ ->
                raise <| new Exception("error: " + tokens.[0])
        asm.Li a0 0
        call "ExitProcess"
        ret.ToArray()
