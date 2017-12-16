#pragma once

int FindOffset();
int CopyGDString(const char* srcObj, char* buffer, size_t bufsize);
DWORD GetDWORDRegKey(char* strValueName, DWORD nDefaultValue);