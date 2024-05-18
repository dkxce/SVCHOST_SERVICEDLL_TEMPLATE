﻿//
// C++ 
// svchost.exe Service DLL Example
// v 0.1, 18.05.2024
// https://github.com/dkxce
// en,ru,1251,utf-8
//

//
// Create Service: 
//   sc.exe create {SERVICE_NAME} binPath= "c:\windows\System32\svchost.exe -k DcomLaunch" type=share start=auto error=ignore displayname="{SERVICE_NAME}"
//
// Specify ServiceDLL Path: 
//   reg add HKLM\SYSTEM\CurrentControlSet\services\{SERVICE_NAME}\Parameters /v ServiceDll /t REG_EXPAND_SZ /d C:\Windows\system32\{SERVICE_NAME}.dll /f
//   HKLM\SYSTEM\CurrentControlSet\services\{SERVICE_NAME}\@ImagePath = "c:\windows\System32\svchost.exe -k DcomLaunch
//   HKLM\SYSTEM\CurrentControlSet\services\{SERVICE_NAME}\Parameters\@ServiceDll=C:\Windows\system32\{SERVICE_NAME}.dll
// 
// Group {SERVICE_NAME} with DcomLaunch:
//   HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Svchost\@DcomLaunch += {SERVICE_NAME}
// 
// Start:
//   sc.exe start {SERVICE_NAME}
// 
// Detection (Power Shell):
//   Get-ItemProperty hklm:\SYSTEM\ControlSet001\Services\*\Parameters | ? { $_.servicedll } | select psparentpath, servicedll
//

#include "pch.h"
#include <windows.h>
#include "mainservice.h"

#define SVCNAME L"SVCHOST_SVC_DLL"

SERVICE_STATUS_HANDLE serviceStatusHandle = nullptr;
SERVICE_STATUS serviceStatus = { SERVICE_WIN32_SHARE_PROCESS, SERVICE_START_PENDING, SERVICE_ACCEPT_STOP | SERVICE_ACCEPT_SHUTDOWN | SERVICE_ACCEPT_PAUSE_CONTINUE };

BOOL APIENTRY DllMain(HMODULE module, DWORD reason, LPVOID reserved)
{
    switch (reason)
    {
        case DLL_PROCESS_ATTACH:
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
        case DLL_PROCESS_DETACH:
            break;
    };
    return true;
}

DWORD WINAPI ServiceHandler(DWORD dwControl, DWORD dwEventType, LPVOID lpEventData, LPVOID lpContext)
{
    switch (dwControl)
    {
        case SERVICE_CONTROL_STOP:
        case SERVICE_CONTROL_SHUTDOWN:
            serviceStatus.dwCurrentState = SERVICE_STOPPED;
            WriteToLog("STOPPED");
            break;
        case SERVICE_CONTROL_PAUSE:
            serviceStatus.dwCurrentState = SERVICE_PAUSED;
            WriteToLog("PAUSED");
            break;
        case SERVICE_CONTROL_CONTINUE:
            serviceStatus.dwCurrentState = SERVICE_RUNNING;
            WriteToLog("CONTINUE");
            break;
        case SERVICE_CONTROL_INTERROGATE:
            WriteToLog("INTERROGATE");
            break;
        default:
            break;
    };

    SetServiceStatus(serviceStatusHandle, &serviceStatus);
    return NO_ERROR;
}

extern "C" __declspec(dllexport) VOID WINAPI ServiceMain(DWORD dwArgc, LPCWSTR * lpszArgv)
{
    serviceStatusHandle = RegisterServiceCtrlHandlerExW(SVCNAME, ServiceHandler, nullptr);
    if (!serviceStatusHandle) return;

    serviceStatus.dwCurrentState = SERVICE_RUNNING;
    SetServiceStatus(serviceStatusHandle, &serviceStatus);

    WriteToLog("RUNNING");
    MainServiceThread();
}