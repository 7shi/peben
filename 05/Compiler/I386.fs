module I386

open System
open System.Collections.Generic
open PELib.I386

type Compiler(forCE:bool) =
    member x.Machine      = 0x14Cus
    member x.ImageBase    = if forCE then 0x10000 else 0x400000
    member x.Subsystem    = if forCE then  9us else 2us
    member x.MajorVersion = if forCE then  2us else 4us
    member x.MinorVersion = if forCE then 11us else 0us

    member x.Compile (lines:string list) (getaddr:string -> int) =
        let ret = new List<byte>()
        let asm = new Assembler(ret)
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
                asm.Mov(eax, Convert.ToInt32 tokens.[2])
                asm.Mov([getarg 1], eax)
            | "ADD" ->
                asm.Mov(eax, [getarg 2])
                asm.Add([getarg 1], eax)
            | "DISP" ->
                asm.Push [getarg 1]
                asm.Push (x.ImageBase + 0x2020)
                asm.Push (x.ImageBase + 0x2000)
                asm.Call [getaddr "wsprintfW"]
                asm.Pop eax
                asm.Pop eax
                asm.Pop eax
                asm.Push 0
                asm.Push 0
                asm.Push (x.ImageBase + 0x2000)
                asm.Push 0
                asm.Call [getaddr "MessageBoxW"]
            | _ ->
                raise <| new Exception("error: " + tokens.[0])
        if not forCE then
            asm.Push 0
            asm.Call [getaddr "ExitProcess"]
        else
            asm.Call [getaddr "GetCurrentProcess"]
            asm.Push 0
            asm.Push eax
            asm.Call [getaddr "TerminateProcess"]
            asm.Jmp -2y
        ret.ToArray()
