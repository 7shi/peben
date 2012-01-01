module PELib.Utils

open System
open System.Reflection
open System.Text

let align v a = (v + a - 1) / a * a

let write8 (image:byte[]) pos (bin:byte[]) =
    array.Copy(bin, 0, image, pos, bin.Length)
    pos + bin.Length

let write16 (image:byte[]) pos (values:uint16[]) =
    Array.fold (fun pos (v:uint16) -> write8 image pos (BitConverter.GetBytes v)) pos values

let write32 (image:byte[]) pos (values:int[]) =
    Array.fold (fun pos (v:int) -> write8 image pos (BitConverter.GetBytes v)) pos values

let writestr (image:byte[]) pos (s:string) =
    write8 image pos (Encoding.UTF8.GetBytes s)

let rec writeAny (image:byte[]) pos (v:obj) =
    if v :? byte then
        image.[pos] <- unbox v
        pos + 1
    elif v :? uint16 then
        write16 image pos [| (unbox v) |]
    elif v :? int then
        write32 image pos [| (unbox v) |]
    elif v :? byte[] then
        write8 image pos (v :?> byte[])
    elif v :? uint16[] then
        write16 image pos (v :?> uint16[])
    elif v :? int[] then
        write32 image pos (v :?> int[])
    else
        let t = v.GetType()
        if t.IsArray then
            let mutable pos = pos
            for o in (v :?> Array) do
                pos <- writeAny image pos o
            pos
        else
            let fs = t.GetFields()
            if fs.Length > 0 then
                writeFields image pos v
            else
                raise <| new ArgumentException("不明な型: ", t.FullName)

and writeFields (image:byte[]) pos (o:obj) =
    Array.fold (fun pos (f:FieldInfo) ->
        writeAny image pos (f.GetValue o)) pos (o.GetType().GetFields())

let conv16 (s:string) =
    let bin = ref (Encoding.ASCII.GetBytes s)
    Array.Resize(bin, 2)
    BitConverter.ToUInt16(!bin, 0)

let conv32 (s:string) =
    let bin = ref (Encoding.ASCII.GetBytes s)
    Array.Resize(bin, 4)
    BitConverter.ToInt32(!bin, 0)

let getBytes (s:string) len =
    let bin = ref (Encoding.ASCII.GetBytes s)
    Array.Resize<byte>(bin, len)
    !bin

let resizeArray arr size =
    let orig = ref arr
    Array.Resize(orig, size)
    !orig
