Module Module1
    Const ImageBase = &H400000
    Const SectionAlignment = 4096, FileAlignment = 512

    Sub Main()
        Dim Sections = {New With {.Name = "Header", .Size = 500},
                        New With {.Name = ".text", .Size = 5000},
                        New With {.Name = ".data", .Size = 10000},
                        New With {.Name = ".idata", .Size = 2000}}

        Dim rva = 0, off = 0
        For Each sect In Sections
            Console.WriteLine("{0,-8} Size={1:x8}, Addr={2:x8}, RVA={3:x8}, File Offset={4:x8}",
                              sect.Name, sect.Size, ImageBase + rva, rva, off)
            rva += Align(sect.Size, SectionAlignment)
            off += Align(sect.Size, FileAlignment)
        Next

        Console.ReadLine()
    End Sub

    Function Align%(v%, a%)
        Return Int((v + a - 1) / a) * a
    End Function
End Module
