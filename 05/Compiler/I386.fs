module I386

open System
open System.Collections.Generic
open PELib.Headers
open PELib.I386

let Compile (lines:string list) (peh:IMAGE_NT_HEADERS32) (imports:Dictionary<string, int>) =
    let ExitProcess = peh.OptionalHeader.ImageBase + imports.["ExitProcess"]
    let MessageBoxA = peh.OptionalHeader.ImageBase + imports.["MessageBoxA"]
    let wsprintfA   = peh.OptionalHeader.ImageBase + imports.["wsprintfA"]
    let ret = new List<byte>()
    let x = new Assembler(ret)
    for line in lines do
        let tokens = line.Split(' ')
        let getarg n = 0x402018 + ((int tokens.[n].[0]) - (int 'A')) * 4
        match tokens.[0] with
        | "LET" ->
            x.mov(eax, Convert.ToInt32 tokens.[2])
            x.mov([getarg 1], eax)
        | "ADD" ->
            x.mov(eax, [getarg 2])
            x.add([getarg 1], eax)
        | "DISP" ->
            x.push [getarg 1]
            x.push 0x402010
            x.push 0x402000
            x.call [wsprintfA]
            x.pop eax
            x.pop eax
            x.pop eax
            x.push 0
            x.push 0
            x.push 0x402000
            x.push 0
            x.call [MessageBoxA]
        | _ ->
            raise <| new Exception("error: " + tokens.[0])
    x.push 0
    x.call [ExitProcess]
    //x.ret()
    ret.ToArray()
