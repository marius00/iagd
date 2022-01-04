#ifndef HOOKLOG_H
#define HOOKLOG_H

#include <fstream>


class HookLog
{
public:
    HookLog();
    ~HookLog();

    void out(std::wstring const& output);

private:
    std::wofstream m_out;
    std::wstring m_lastMessage;
    unsigned int m_lastMessageCount;
};

#endif // HOOKLOG_H
