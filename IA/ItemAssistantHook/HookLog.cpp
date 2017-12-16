#include "StdAfx.h"
#include "HookLog.h"


HookLog::HookLog()
    : m_lastMessageCount(0)
{
    char tmpfolder[MAX_PATH];
    GetTempPath(MAX_PATH, tmpfolder);

    std::string tmpfile(tmpfolder);
	tmpfile += "aoia_hook.log";
    //tmpfile = _T("C:\\Users\\Andrew\\Desktop\\aoia_hook.log");

    m_out.open(tmpfile);

    if (m_out.is_open())
    {
        m_out
            << "****************************"  << std::endl
            << "    Hook Logging Started"      << std::endl
            << "****************************"  << std::endl;

        TCHAR buffer[MAX_PATH];
        DWORD size = GetCurrentDirectory(MAX_PATH, buffer);
        buffer[size] = '\0';

        m_out << "Current Directory: " << buffer << std::endl;
    }
}


HookLog::~HookLog()
{
    if (m_out.is_open())
    {
        m_out
            << "****************************" << std::endl
            << "   Hook Logging Terminated  " << std::endl
            << "****************************" << std::endl;

        m_out.close();
    }
}


void HookLog::out( std::string const& output )
{
    if (m_out.is_open())
    {
        if (!m_lastMessage.empty())
        {
            if (m_lastMessage.compare(output) == 0)
            {
                ++m_lastMessageCount;
            }
            else
            {
                m_out << "Last message was repeated " << m_lastMessageCount << " times." << std::endl;
                m_lastMessage = output;
                m_lastMessageCount = 1;
                m_out << output.c_str() << std::endl;
            }
        }
        else
        {
            m_lastMessage = output;
            m_lastMessageCount = 1;
            m_out << output.c_str() << std::endl;
        }
    }
}
