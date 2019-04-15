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
