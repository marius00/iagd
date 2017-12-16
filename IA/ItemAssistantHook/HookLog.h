#ifndef HOOKLOG_H
#define HOOKLOG_H

#include <fstream>


class HookLog
{
public:
    HookLog();
    ~HookLog();

    void out(std::string const& output);

private:
    std::ofstream m_out;
    std::string m_lastMessage;
    unsigned int m_lastMessageCount;
};

#endif // HOOKLOG_H
