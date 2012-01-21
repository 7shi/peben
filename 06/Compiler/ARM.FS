module ARM

open System
open System.Collections.Generic

type Compiler() =
    member x.Machine      = 0x1C0us
    member x.ImageBase    = 0x10000
    member x.Subsystem    = 9us
    member x.MajorVersion = 4us
    member x.MinorVersion = 0us

    member x.Compile (lines:string list) (imports:Dictionary<string, int>) =
        let ret = new List<byte>()
        let add (x:int) = ret.AddRange(BitConverter.GetBytes x)
        let call name =
            let offset = imports.[name] - 0x3000
            add 0xe1a0e00f // mov lr, pc
            add (0xe595f000 + offset) // mov pc, [r5, #offset]
        add 0xe3a04a12 // mov r4, #0x12000
        add 0xe3a05a13 // mov r5, #0x13000
        for line in lines do
            let tokens = line.Split(' ')
            let getarg n =
                let v = tokens.[n]
                if v.Length <> 1 || v < "A" || v > "Z" then
                    raise <| new Exception("invalid variable: " + tokens.[n])
                else
                    0x30 + ((int v.[0]) - (int 'A')) * 4
            match tokens.[0] with
            | "LET" ->
                add 0xe59f0000 // ldr r0, [pc]
                add 0xe1a0f00f // mov pc, pc
                add (Convert.ToInt32 tokens.[2])
                add (0xe5840000 + (getarg 1)) // str r0, [r4, #arg1]
            | "ADD" ->
                add (0xe5940000 + (getarg 1)) // ldr r0, [r4, #arg1]
                add (0xe5941000 + (getarg 2)) // ldr r1, [r4, #arg2]
                add 0xe0800001 // add r0, r0, r1
                add (0xe5840000 + (getarg 1)) // str r0, [r4, #arg1]
            | "DISP" ->
                add 0xe1a00004 // mov r0, r4
                add 0xe2841020 // add r1, r4, #0x20
                add (0xe5942000 + (getarg 1)) // ldr r2, [r4, #arg1]
                call "wsprintfW"
                add 0xe3a00000 // mov r0, #0
                add 0xe1a01004 // mov r1, r4
                add 0xe3a02000 // mov r2, #0
                add 0xe3a03000 // mov r3, #0
                call "MessageBoxW"
            | _ ->
                raise <| new Exception("error: " + tokens.[0])
        add 0xe3a00042 // mov r0, #CurrentProcess
        add 0xe3a01000 // mov r1, #0
        call "TerminateProcess"
        add 0xeafffffe // 0: b 0b
        ret.ToArray()
