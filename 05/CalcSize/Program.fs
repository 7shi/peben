open System

let ImageBase = 0x400000
let SectionAlignment = 4096
let FileAlignment = 512

let Align v a = ((v + a - 1) / a) * a

type Section = { Name:string; Size:int }

let Sections = [ { Name = "Header"; Size =   500}
                 { Name = ".text" ; Size =  5000}
                 { Name = ".data" ; Size = 10000}
                 { Name = ".idata"; Size =  2000} ]

ignore <| List.fold
    (fun tp sect ->
        let rva, off = tp
        Console.WriteLine("{0,-8} Size={1:x8}, Addr={2:x8}, RVA={3:x8}, File Offset={4:x8}",
                          sect.Name, sect.Size, ImageBase + rva, rva, off)
        Align sect.Size SectionAlignment, Align sect.Size FileAlignment)
    (0, 0) Sections

ignore <| Console.ReadLine()
