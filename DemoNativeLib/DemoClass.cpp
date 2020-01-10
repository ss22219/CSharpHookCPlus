#include "stdafx.h"
#include "DemoClass.h"
#include <iostream>

DemoClass* _instance;
DemoClass* DemoClass::GetInstance()
{
	if (_instance == nullptr)
		_instance = new DemoClass();
	return _instance;
}

void DemoClass::SendMsg(net_packet_t * packet)
{
	printf_s("size:%ld msgType:%u\r\n", packet->length, packet->identifier);
	this->m_userId = packet->identifier;
}

DemoClass::DemoClass()
{
	this->m_userId = 0x12345;
	this->m_userName = TEXT("MyUserName");
}


DemoClass::~DemoClass()
{
}
