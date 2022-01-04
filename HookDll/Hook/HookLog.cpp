#include "StdAfx.h"
#include "HookLog.h"


HookLog::HookLog()
    : m_lastMessageCount(0)
{
    wchar_t tmpfolder[MAX_PATH];
    GetTempPath(MAX_PATH, tmpfolder);

    std::wstring tmpfile(tmpfolder);
	tmpfile += L"iagd_hook.log"; // %appdata%\..\local\temp\iagd_hook.log

    m_out.open(tmpfile);

    if (m_out.is_open())
    {
        m_out
            << L"****************************"  << std::endl
            << L"    Hook Logging Started"      << std::endl
            << L"****************************"  << std::endl;

        TCHAR buffer[MAX_PATH];
        DWORD size = GetCurrentDirectory(MAX_PATH, buffer);
        buffer[size] = '\0';

        m_out << L"Current Directory: " << buffer << std::endl;
    }
}


HookLog::~HookLog()
{
    if (m_out.is_open())
    {
        m_out
            << L"****************************" << std::endl
            << L"   Hook Logging Terminated  " << std::endl
            << L"****************************" << std::endl;

        m_out.close();
    }
}


void HookLog::out( std::wstring const& output )
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
				if (m_lastMessageCount > 1) {
					m_out << L"Last message was repeated " << m_lastMessageCount << L" times." << std::endl;
				}
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
