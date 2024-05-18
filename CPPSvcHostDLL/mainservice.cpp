﻿//
// C++ 
// svchost.exe Service DLL Example
// v 0.1, 18.05.2024
// https://github.com/dkxce
// en,ru,1251,utf-8
//

#include "pch.h"
#include "mainservice.h"
#include <iostream>
#include <fstream>  

wchar_t* convertCharArrayToLPCWSTR(const char* charArray)
{
    wchar_t* wString = new wchar_t[4096];
    MultiByteToWideChar(CP_ACP, 0, charArray, -1, wString, 4096);
    return wString;
}

VOID WriteToLog(const char* charArray)
{
    std::ofstream outfile("C:\\SVCHOST_SVC_DLL.log.txt", std::ios_base::app);
    outfile << charArray << std::endl;
    outfile.close();
}

VOID MainServiceThread()
{
    WriteToLog("MainServiceThread");
}