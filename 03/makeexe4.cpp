#include <cstdio>
#include <cstdlib>
#include <cstring>
#include <windows.h>

char image[0x600];

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
    fh->NumberOfSections = 2;
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
    oph->SizeOfImage = 0x3000;
    oph->SizeOfHeaders = 0x200;
    oph->Subsystem = 3;
    oph->DllCharacteristics = 0x8540;
    oph->SizeOfStackReserve = 0x100000;
    oph->SizeOfStackCommit = 0x1000;
    oph->SizeOfHeapReserve = 0x100000;
    oph->SizeOfHeapCommit = 0x1000;
    oph->NumberOfRvaAndSizes = 16;
    oph->DataDirectory[1].VirtualAddress = 0x00002000;
    oph->DataDirectory[1].Size = 0x00001000;
    
    auto scth = reinterpret_cast<IMAGE_SECTION_HEADER*>(&image[0x178]);
    
    memcpy(scth[0].Name, ".text", 5);
    scth[0].Misc.VirtualSize = 14;
    scth[0].VirtualAddress = 0x1000;
    scth[0].SizeOfRawData = 0x200;
    scth[0].PointerToRawData = 0x200;
    scth[0].Characteristics = 0x60000020;
    
    memcpy(scth[1].Name, ".idata", 6);
    scth[1].Misc.VirtualSize = 0x54;
    scth[1].VirtualAddress = 0x2000;
    scth[1].SizeOfRawData = 0x200;
    scth[1].PointerToRawData = 0x400;
    scth[1].Characteristics = 0xc0300040;
    
    image[0x200] = 0x68;
    *(DWORD*)&image[0x201] = 123;
    image[0x205] = 0xa1;
    *(DWORD*)&image[0x206] = 0x00402030;
    *(WORD*)&image[0x20a] = 0xd0ff;
    image[0x20c] = 0x58;
    image[0x20d] = 0xc3;
    
    auto ilt = reinterpret_cast<IMAGE_IMPORT_DESCRIPTOR*>(&image[0x400]);
    ilt[0].OriginalFirstThunk = 0x2028;
    ilt[0].Name = 0x2048;
    ilt[0].FirstThunk = 0x2030;
    *(DWORD*)&image[0x428] = 0x2038;
    *(DWORD*)&image[0x430] = 0x2038;
    strcpy(&image[0x43a], "prtnum");
    strcpy(&image[0x448], "prtnum.dll");
    
    auto f = fopen("output.exe", "wb");
    if (!f) return 1;
    fwrite(image, 1, sizeof(image), f);
    fclose(f);
}
