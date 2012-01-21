module MIPSel

open System
open System.Collections.Generic
open PELib.MIPS

type Compiler(forCE:bool) =
    member x.Machine      = 0x166us
    member x.ImageBase    = if forCE then 0x10000 else 0x400000
    member x.Subsystem    = if forCE then  9us else 2us
    member x.MajorVersion = if forCE then  2us else 4us
    member x.MinorVersion = if forCE then 11us else 0us

    member x.Compile (lines:string list) (imports:Dictionary<string, int>) =
        let ret = new List<byte>()
        let asm = new Assembler(ret, true)
        let call name =
            asm.La at (x.ImageBase + imports.[name])
            asm.Lw at 0(at)
            asm.Nop // load delay slot
            asm.Jalr ra at
            asm.Nop // branch delay slot
        for line in lines do
            let tokens = line.Split(' ')
            let getarg n =
                let v = tokens.[n]
                if v.Length <> 1 || v < "A" || v > "Z" then
                    raise <| new Exception("invalid variable: " + tokens.[n])
                else
                    x.ImageBase + 0x2030 + ((int v.[0]) - (int 'A')) * 4
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
                asm.La a1 (x.ImageBase + 0x2020)
                asm.La a0 (x.ImageBase + 0x2000)
                call "wsprintfW"
                asm.Li a3 0
                asm.Li a2 0
                asm.La a1 (x.ImageBase + 0x2000)
                asm.Li a0 0
                call "MessageBoxW"
            | _ ->
                raise <| new Exception("error: " + tokens.[0])
        if not forCE then
            asm.Li a0 0
            call "ExitProcess"
        else
            asm.Li a0 66 // CurrentProcess
            asm.Move a1 zero
            call "TerminateProcess"
            asm.B -1
            asm.Nop // branch delay slot
        ret.ToArray()
