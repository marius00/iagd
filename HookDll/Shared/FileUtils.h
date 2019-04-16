#pragma once

#include <shared/UnicodeSupport.h>

std::tstring BrowseForFolder(HWND hWndOwner, std::tstring const& title);
std::tstring BrowseForOutputFile(HWND hWndOwner, std::tstring const& title, TCHAR* filter, std::tstring const& defaultExtension);
