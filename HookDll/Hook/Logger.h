#pragma once
#include <string>

enum class LogLevel {
	WARNING, INFO, FATAL
};


void LogToFile(LogLevel level, const wchar_t* message);
void LogToFile(LogLevel level, std::wstring message);
void LogToFile(LogLevel level, const char* message);
void LogToFile(LogLevel level, std::wstringstream message);
void LogToFile(LogLevel level, const std::string message);