#ifndef AOITEMPARSER_H
#define AOITEMPARSER_H

#include <shared/AODB.h>


class AOItemParser
    : public ao_item
{
public:
    AOItemParser(char* pBuffer, unsigned int bufSize);

private:
    char* ParseFunctions(char* pBuffer, unsigned int bufSize, unsigned int ftype, unsigned int fkey);
    char* ParseRequirements(char *pBuffer, unsigned int bufSize, std::list<ao_item_req> &reqlist, unsigned int cnt, int rhook);
    char* ParseString(char *pBuffer, unsigned int bufSize, std::string &outText);
};


#endif // AOITEMPARSER_H
