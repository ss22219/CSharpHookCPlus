// DemoNativeLib.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "DemoClass.h"
extern "C" int _declspec(dllexport) NativeMethod() {
	return 1;
}

extern "C" void _declspec(dllexport) _stdcall SendMsg(unsigned short identifier) {
	net_packet_t* packet = new net_packet_t();
	packet->identifier = identifier;
	DemoClass::GetInstance()->SendMsg(packet);
	delete packet;
}

