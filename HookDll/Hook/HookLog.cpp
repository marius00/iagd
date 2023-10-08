#include "StdAfx.h"
#include "HookLog.h"
#include <filesystem>
#include <iostream>
#include <windows.h>
#include <shlobj.h>

std::wstring GetIagdFolder() {
    PWSTR path_tmp;
    auto get_folder_path_ret = SHGetKnownFolderPath(FOLDERID_RoamingAppData, 0, nullptr, &path_tmp);

    if (get_folder_path_ret != S_OK) {
        CoTaskMemFree(path_tmp);
        return std::wstring();
    }

    std::wstring path = path_tmp;
    CoTaskMemFree(path_tmp);

    return path + L"\\..\\local\\evilsoft\\iagd\\";
}

HookLog::HookLog() : m_lastMessageCount(0), m_initialized(false) {
    std::wstring iagdFolder = GetIagdFolder(); // %appdata%\..\local\evilsoft\iagd

    wchar_t tmpfolder[MAX_PATH]; // "%appdata%\..\local\temp\"
    GetTempPath(MAX_PATH, tmpfolder);

    std::wstring logFile(!iagdFolder.empty() ? iagdFolder : tmpfolder);
    logFile += L"iagd_hook.log"; 

    m_out.open(logFile);

    if (m_out.is_open()) {
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


HookLog::~HookLog() {
    if (m_out.is_open()) {
        m_out
            << L"****************************" << std::endl
            << L"   Hook Logging Terminated  " << std::endl
            << L"****************************" << std::endl;

        m_out.close();
    }
}

void HookLog::out(const char* src) {
	return out(std::wstring(src, src + strlen(src)));
}

void HookLog::out( std::wstring const& output ) {
    if (m_out.is_open()) {
        if (!m_lastMessage.empty()) {
            if (m_lastMessage.compare(output) == 0) {
                ++m_lastMessageCount;
            }
            else {
				if (m_lastMessageCount > 1) {
					//m_out << L"Last message was repeated " << m_lastMessageCount << L" times." << std::endl;
				}
                m_lastMessage = output;
                m_lastMessageCount = 1;
                m_out << output.c_str() << std::endl;
            }
        }
        else {
            m_lastMessage = output;
            m_lastMessageCount = 1;
            m_out << output.c_str() << std::endl;
        }

		if (!m_initialized) {
			m_out.flush();
		}
    }
}

void HookLog::setInitialized(bool b) {
	m_initialized = b;
}
