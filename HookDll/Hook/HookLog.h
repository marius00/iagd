#ifndef HOOKLOG_H
#define HOOKLOG_H

#include <fstream>


class HookLog {
public:
    HookLog();
    ~HookLog();

    void out(std::wstring const& output);
	void setInitialized(bool b);

private:
    std::wofstream m_out;
    std::wstring m_lastMessage;
    unsigned int m_lastMessageCount;
	bool m_initialized;
};

#endif // HOOKLOG_H
