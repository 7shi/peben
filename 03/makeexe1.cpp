#include <cstdio>
#include <cstring>
#include <windows.h>

BYTE image[0x400];

int main()
{
    auto dosh = reinterpret_cast<IMAGE_DOS_HEADER*>(&image[0]);
    dosh->e_magic = *(WORD*)"MZ";
    dosh->e_cblp = 0x90;
    dosh->e_cp = 3;
    dosh->e_cparhdr = 4;
    dosh->e_maxalloc = 0xffff;
    dosh->e_sp = 0xb8;
    dosh->e_lfarlc = 0x40;
    dosh->e_lfanew = 0x80;
    BYTE stub[] = { 0xb8, 0x01, 0x4c, 0xcd, 0x21 }; // メッセージを省略
    memcpy(&image[0x40], stub, sizeof(stub));
    
    auto peh = reinterpret_cast<IMAGE_NT_HEADERS32*>(&image[0x80]);
    peh->Signature = *(DWORD*)"PE\0\0";
    
    auto fh = &peh->FileHeader;
    fh->Machine = 0x14c;
    fh->NumberOfSections = 1;
    fh->SizeOfOptionalHeader = 0xe0;
    fh->Characteristics = 0x102;
    
    auto oph = &peh->OptionalHeader;
    oph->Magic = 0x10b;
    oph->MajorLinkerVersion = 10;
    oph->SizeOfCode = 0x200;
    oph->AddressOfEntryPoint = 0x1000;
    oph->BaseOfCode = 0x1000;
    oph->BaseOfData = 0x2000;
    oph->ImageBase  = 0x400000;
    oph->SectionAlignment = 0x1000;
    oph->FileAlignment = 0x200;
    oph->MajorOperatingSystemVersion = 5;
    oph->MinorOperatingSystemVersion = 1;
    oph->MajorSubsystemVersion = 5;
    oph->MinorSubsystemVersion = 1;
    oph->SizeOfImage = 0x2000;
    oph->SizeOfHeaders = 0x200;
    oph->Subsystem = 2;
    oph->DllCharacteristics = 0x8540;
    oph->SizeOfStackReserve = 0x100000;
    oph->SizeOfStackCommit = 0x1000;
    oph->SizeOfHeapReserve = 0x100000;
    oph->SizeOfHeapCommit = 0x1000;
    oph->NumberOfRvaAndSizes = 16;
    
    auto text = reinterpret_cast<IMAGE_SECTION_HEADER*>(&image[0x178]);
    memcpy(text->Name, ".text", 5);
    text->Misc.VirtualSize = 1;
    text->VirtualAddress = 0x1000;
    text->SizeOfRawData = 0x200;
    text->PointerToRawData = 0x200;
    text->Characteristics = 0x60000020;
    
    image[0x200] = 0xc3;
    
    auto f = fopen("output.exe", "wb");
    if (!f) return 1;
    fwrite(image, 1, sizeof(image), f);
    fclose(f);
}
