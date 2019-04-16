#ifndef TEXTFORMAT_H
#define TEXTFORMAT_H

#include <shared/UnicodeSupport.h>

class TextFormat
{
public:
    TextFormat(int, char*);
    ~TextFormat();

    // Convert a Long to a formatted Number String
	std::tstring TextFormat::FormatLongToString(long l) const;
	std::tstring TextFormat::FormatLongLongToString(unsigned __int64 l) const;
private:
    int m_width;
    char m_delimeter;
};


#endif // TEXTFORMAT_H
