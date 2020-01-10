#pragma once
struct net_packet_s
{
	unsigned long length;
	unsigned long timestamp;
	unsigned short identifier;
};
typedef net_packet_s net_packet_t;

class DemoClass
{
public:
	DemoClass();
	~DemoClass();
	int m_userId;
	const wchar_t* m_userName;
	void SendMsg(net_packet_t* packet);
	static DemoClass* GetInstance();
};

