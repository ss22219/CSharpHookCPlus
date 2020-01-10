#pragma once
typedef struct LoadClrLibraryParam {
	wchar_t* AssemblyPath;
	wchar_t* ClassName;
	wchar_t* StaticMethodName;
	wchar_t* Argument;
};