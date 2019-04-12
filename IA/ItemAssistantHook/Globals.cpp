#include <windows.h>

#define MCPY(x) { memcpy(buffer + pos, &x, sizeof(x)); pos += sizeof(x); }
// Extract a string from a "GDString"
// Returns [length][string]
int CopyGDString(const char* srcObj, char* buffer, size_t bufsize) {
	char* _strlen = (char*)srcObj + 16;
	char* _deref = (char*)srcObj + 20;
	bool requiresDeref = *_deref >= 0x10u;

	DWORD strlen = *(DWORD *)_strlen;
	size_t pos = 0;
	MCPY(strlen);

	if (strlen > 0xFFFFFFFE || strlen >= bufsize || strlen <= 0) {
		return 4;
	}

	const void* srcObj_ = srcObj;

	//if (*(DWORD *)(srcObj + 20) >= 0x10u)
	if (requiresDeref)
		srcObj_ = *((void**)srcObj);



	memcpy(buffer + 4, (const void *)(srcObj_), strlen);
	return strlen + 4;
}


DWORD GetDWORDRegKey(HKEY hKey, char* strValueName, DWORD nDefaultValue) {
	DWORD dwBufferSize(sizeof(DWORD));
	DWORD nResult(0);
	LONG nError = ::RegQueryValueExA(hKey,
		strValueName,
		0,
		NULL,
		reinterpret_cast<LPBYTE>(&nResult),
		&dwBufferSize);

	if (ERROR_SUCCESS == nError) {
		return nResult;
	}

	return nDefaultValue;
}
DWORD GetDWORDRegKey(char* strValueName, DWORD nDefaultValue) {
	HKEY hKey;
	LONG lRes = RegOpenKeyExW(HKEY_CURRENT_USER, L"SOFTWARE\\EvilSoft\\IAGD", 0, KEY_READ, &hKey);
	if (lRes == ERROR_SUCCESS) {
		auto result = GetDWORDRegKey(hKey, strValueName, nDefaultValue);
		RegCloseKey(hKey);
		return result;
	}

	return nDefaultValue;
}
