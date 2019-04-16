#include "StdAfx.h"
#include "FileUtils.h"
#include "shlobj.h"
#include "commdlg.h"


std::tstring BrowseForFolder(HWND hWndOwner, std::tstring const& title)
{
    BROWSEINFO udtBI;
    ITEMIDLIST *udtIDList;

    /* Initialise */
    udtBI.hwndOwner = hWndOwner;
    udtBI.pidlRoot = NULL;
    udtBI.pszDisplayName = NULL;
    udtBI.lpszTitle = title.c_str();
    udtBI.ulFlags = BIF_RETURNONLYFSDIRS;
    udtBI.lpfn = NULL;
    udtBI.lParam = NULL;
    udtBI.iImage = 0;

    /* Prompt user for folder */
    udtIDList = SHBrowseForFolder(&udtBI);

    /* Extract pathname */
    TCHAR strPath[MAX_PATH];
    if (!SHGetPathFromIDList(udtIDList, (TCHAR*)&strPath)) {
        strPath[0] = 0;	// Zero-length if failure
    }

    return std::tstring(strPath);
}


std::tstring BrowseForOutputFile(HWND hWndOwner, std::tstring const& title, TCHAR* filter, std::tstring const& defaultExtension)
{
    TCHAR strPath[MAX_PATH];
    ZeroMemory(strPath, sizeof(TCHAR) * MAX_PATH);

    OPENFILENAME ofn;
    ZeroMemory(&ofn, sizeof(OPENFILENAME));

    ofn.lStructSize = sizeof(OPENFILENAME);
    ofn.hwndOwner = hWndOwner;
    ofn.lpstrFilter = filter;
    ofn.nFilterIndex = 1;
    ofn.lpstrFile = strPath;
    ofn.nMaxFile = MAX_PATH;
    ofn.lpstrTitle = title.c_str();
    ofn.Flags = OFN_EXPLORER | OFN_OVERWRITEPROMPT;
    ofn.lpstrDefExt = defaultExtension.c_str();

    if (GetSaveFileName(&ofn)) {
        return std::tstring(strPath);
    }

    return _T("");
}
