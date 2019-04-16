#include "stdafx.h"
#include "UnicodeSupport.h"


std::string to_ascii_copy(std::wstring const& input)
{
   int len_buffer = input.length() + 1;
   char* abuffer = (char*)malloc( len_buffer );
   ZeroMemory(abuffer, len_buffer );

   WideCharToMultiByte(CP_ACP, NULL, input.c_str(), input.length(), abuffer, len_buffer, NULL, NULL);

   std::string result(abuffer);

   free(abuffer);

   return result;
}


inline std::string to_ascii_copy(std::string const& input)
{
   return input;
}


std::string to_utf8_copy(std::tstring const& input)
{
#ifdef UNICODE

   int len_buffer = input.length() + 1;
   char* abuffer = (char*)malloc( len_buffer );
   ZeroMemory(abuffer, len_buffer );

   WideCharToMultiByte(CP_UTF8, NULL, input.c_str(), input.length(), abuffer, len_buffer, NULL, NULL);

   std::string result(abuffer);

   free(abuffer);

   return result;

#else

   return input;

#endif
}


std::tstring from_ascii_copy(std::string const& input)
{
#ifdef UNICODE

   int len_buffer = input.length() + 1;
   TCHAR* wbuffer = (TCHAR*)malloc( len_buffer * 2 );
   ZeroMemory(wbuffer, len_buffer * 2 );

   MultiByteToWideChar(CP_ACP, NULL, input.c_str(), input.length(), wbuffer, len_buffer);

   std::tstring result(wbuffer);

   free(wbuffer);

   return result;

#else

   return input;

#endif
}


std::tstring from_utf8_copy(std::string const& input)
{
#ifdef UNICODE

   int len_buffer = input.length() + 1;
   TCHAR* wbuffer = (TCHAR*)malloc( len_buffer * 2 );
   ZeroMemory(wbuffer, len_buffer * 2 );

   MultiByteToWideChar(CP_UTF8, NULL, input.c_str(), input.length(), wbuffer, len_buffer);

   std::tstring result(wbuffer);

   free(wbuffer);

   return result;

#else

   return input;

#endif
}
