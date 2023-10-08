#pragma once
#include <string>

void LogToFile(const wchar_t* message);
void LogToFile(std::wstring message);
void LogToFile(const char* message);
void LogToFile(std::wstringstream message);
void LogToFile(const std::string message);