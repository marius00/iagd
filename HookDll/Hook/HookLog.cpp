#include "StdAfx.h"
#include "HookLog.h"
#include <filesystem>
#include <iostream>
#include <windows.h>
#include <shlobj.h>
#include "Logger.h"

// TODO: What's this doing in HookLog.cpp ??
//
// Deliberately does not log. It runs from HookLog's own constructor, so on the failure path it used to write through g_log -- the very object still being built -- and it does so before any log file exists,
// which makes the resulting crash completely silent. The one caller that can safely report a failure (the constructor below) does so once its stream is open.
std::wstring GetIagdFolder() {
    // Documented to be set to null on failure, but it is an out-parameter on a call that may not have run at all, and this used to be freed uninitialised.
    PWSTR path_tmp = nullptr;
    auto get_folder_path_ret = SHGetKnownFolderPath(FOLDERID_RoamingAppData, 0, nullptr, &path_tmp);

    if (get_folder_path_ret != S_OK) {
        if (path_tmp != nullptr) {
            CoTaskMemFree(path_tmp);
        }
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

        // The only safe place to report this: GetIagdFolder cannot log, and by here we have a stream.
        if (iagdFolder.empty()) {
            m_out << L"WARNING Could not find the roaming appdata folder, logging to the temp folder instead." << std::endl;
        }

        TCHAR buffer[MAX_PATH];
        DWORD size = GetCurrentDirectory(MAX_PATH, buffer);
        buffer[size] = '\0';

        m_out << L"Current Directory: " << buffer << std::endl;
    }
}


HookLog::~HookLog() {
    if (m_out.is_open()) {
		writeRepeatSummary();
        m_out
            << L"****************************" << std::endl
            << L"   Hook Logging Terminated  " << std::endl
            << L"****************************" << std::endl;

        m_out.close();
    }
}

void HookLog::out(const char* src, bool forceFlush) {
	return out(std::wstring(src, src + strlen(src)), forceFlush);
}

/// Emit the "repeated N times" line for the message we are about to move off of.
/// Callers must already hold m_mutex.
void HookLog::writeRepeatSummary() {
	if (m_lastMessageCount > 1) {
		m_out << L"    (last message repeated " << m_lastMessageCount << L" times)" << std::endl;
	}
}

void HookLog::out( std::wstring const& output, bool forceFlush ) {
	std::lock_guard<std::mutex> guard(m_mutex);

    if (m_out.is_open()) {
        if (!m_lastMessage.empty()) {
            if (m_lastMessage.compare(output) == 0) {
                ++m_lastMessageCount;
            }
            else {
				writeRepeatSummary();
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

		if (!m_initialized || forceFlush) {
			m_out.flush();
		}
    }
}

void HookLog::setInitialized(bool b) {
	m_initialized = b;
}
