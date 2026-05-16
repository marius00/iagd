#pragma once
#include <string>
#include <atomic>

enum class LogLevel {
	WARNING, INFO, FATAL
};

/// Global flag: set to true when the DLL is detaching.
/// Hook proxies should check this and skip all IA logic (just call the original).
extern std::atomic<bool> g_isDetaching;

void LogToFile(LogLevel level, const wchar_t* message);
void LogToFile(LogLevel level, std::wstring message);
void LogToFile(LogLevel level, const char* message);
void LogToFile(LogLevel level, std::wstringstream message);
void LogToFile(LogLevel level, const std::string message);