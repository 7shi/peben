module I386

open System
open System.Collections.Generic
open PELib.I386

let Compile (lines:string list) (getaddr:string -> int) =
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
            x.call [getaddr "wsprintfA"]
            x.pop eax
            x.pop eax
            x.pop eax
            x.push 0
            x.push 0
            x.push 0x402000
            x.push 0
            x.call [getaddr "MessageBoxA"]
        | _ ->
            raise <| new Exception("error: " + tokens.[0])
    x.push 0
    x.call [getaddr "ExitProcess"]
    //x.ret()
    ret.ToArray()
