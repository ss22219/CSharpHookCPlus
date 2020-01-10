// UnrealCore.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"

#include <iostream>
#include "CLRHost.h"
#include "Types.h"
#include "detours.h"

CLRHost* Host = NULL;
extern "C" void _declspec(dllexport) LoadClrLibrary(LoadClrLibraryParam* param) {
	if (Host == NULL) {
		Host = new CLRHost();
		Host->Start();
		if (!Host->IsOK()) {
			printf_s("Failed to start CLR\n", Host->GetErrorResult());
			return;
		}

		unsigned long result;
		Host->LoadIntoDefaultAppDomain(param->AssemblyPath, param->ClassName, param->StaticMethodName, param->Argument, result);
		if (!Host->IsOK()) {
			wprintf_s(TEXT("Failed to start CLR and load %s (%d)\n"), param->AssemblyPath, Host->GetErrorResult());
			return;
		}
	}
}

extern "C"  void  _declspec(dllexport) _stdcall  Hook(void** sourceFunc, void* targetFunc) {
	DetourTransactionBegin();
	DetourAttach(sourceFunc, targetFunc);
	DetourTransactionCommit();
}

extern "C"  void  _declspec(dllexport) _stdcall  UnHook(void** sourceFunc, void* targetFunc) {
	DetourTransactionBegin();
	DetourDetach(sourceFunc, targetFunc);
	DetourTransactionCommit();
}