open System
open System.IO
open PELib.Utils
open PELib.PE

let image = Array.zeroCreate<byte> 0x400

// DOS header
ignore <| writestr image 0 "MZ"
ignore <| write16 image 2 [| 0x90us; 3us; 0us; 4us; 0us; 0xFFFFus |]
ignore <| write16 image 0x10 [| 0xB8us |]
ignore <| write16 image 0x18 [| 0x40us |]
ignore <| write32 image 0x3C [| 0x80 |]

// DOS binary
ignore <| write8 image 0x40 [| 0xB8uy; 1uy; 0x4Cuy; 0xCDuy; 0x21uy |]

// PE header
ignore <| writestr image 0x80 "PE"
ignore <| write16 image 0x84 [| 0x14Cus; 1us |]
ignore <| write16 image 0x94 [| 0xE0us; 0x102us; 0x10Bus |]
ignore <| write8  image 0x9A [| 10uy; 0uy |]
ignore <| write32 image 0x9C [| 0x200 |]
ignore <| write32 image 0xA8 [| 0x1000; 0x1000 |]
ignore <| write32 image 0xB0 [| 0x2000; 0x400000 |]
ignore <| write32 image 0xB8 [| 0x1000; 0x200 |]
ignore <| write16 image 0xC0 [| 4us; 0us |]
ignore <| write16 image 0xC8 [| 4us; 0us |]
ignore <| write32 image 0xD0 [| 0x2000; 0x200 |]
ignore <| write16 image 0xDC [| 2us; 0us |]
ignore <| write32 image 0xE0 [| 0x100000; 0x1000 |]
ignore <| write32 image 0xE8 [| 0x100000; 0x1000 |]
ignore <| write32 image 0xF4 [| 0x10 |]

// section header
ignore <| writestr image 0x178 ".text"
ignore <| write32 image 0x180 [| 1; 0x1000 |]
ignore <| write32 image 0x188 [| 0x200; 0x200 |]
ignore <| write32 image 0x19C [| 0x60000020 |]

// .text
ignore <| write8 image 0x200 [| 0xC3uy |]

// ファイル出力
using (new FileStream("output.exe", FileMode.Create)) <| fun fs ->
    fs.Write(image, 0, image.Length)
