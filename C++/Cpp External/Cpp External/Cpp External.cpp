#include <iostream>
#include <vector>
#include <Windows.h>

#include "proc.h"

int main()
{
    DWORD procId = GetProcId(L"ac_client.exe");

    uintptr_t moduleBase = GetModuleBaseAddress(procId, L"ac_client.exe");

    HANDLE hProcess = 0;
    hProcess = OpenProcess(PROCESS_ALL_ACCESS, NULL, procId);

    uintptr_t dynamicPtrBaseAddr = moduleBase + 0x00042CC4;
    std::cout << "Dynamic Pointer Base Addres :" << "0x" << std::hex << dynamicPtrBaseAddr << std::endl;

    std::vector<unsigned int> ammoOffset = { 0x80, 0xE80};
    uintptr_t ammoAddr = FindDMAAddy(hProcess, dynamicPtrBaseAddr, ammoOffset);
    std::cout << "Ammo Address :" << "0x" << std::hex << ammoAddr << std::endl;

    int ammovalue = 0;
    ReadProcessMemory(hProcess, (BYTE*)ammoAddr, &ammovalue, sizeof(ammovalue), nullptr);
    std::cout << "Current Ammo :" << "0x" << std::dec << ammovalue << std::endl;

    int newAmmo = 1337;
    WriteProcessMemory(hProcess, (BYTE*)ammoAddr, &newAmmo, sizeof(newAmmo), nullptr);
    ReadProcessMemory(hProcess, (BYTE*)ammoAddr, &ammovalue, sizeof(ammovalue), nullptr);
    std::cout << "New Ammo = " << std::dec << ammovalue << std::endl;


    getchar();
    return 0;
}