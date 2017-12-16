#include "StdAfx.h"
#include "Parser.h"

Parser::Parser(char* memory, unsigned int size)
    : m_start(memory)
    , m_end(memory + size)
    , m_pos(memory)
{
}


Parser::~Parser()
{
}


unsigned int Parser::remaining() const
{
    if (m_end > m_start) {
        return (unsigned int)(m_end - m_pos);
    }
    return 0;
}


void Parser::skip( unsigned int count )
{
    assert(count <= remaining());

    if (count <= remaining()) {
        m_pos += count;
    }
}


unsigned char Parser::popChar() const
{
    return *(m_pos++);
}


unsigned short Parser::popShort() const
{
    unsigned short retval = 0;
    memcpy(&retval, m_pos, 2);
    m_pos += 2;
    return _byteswap_ushort(retval);
}


unsigned int Parser::popInteger() const
{
    unsigned int retval = 0;
    memcpy(&retval, m_pos, 4);
    m_pos += 4;
    return _byteswap_ulong(retval);
}


std::string Parser::popString() const
{
    unsigned short len = popChar();
    std::string retval(m_pos, len);
    m_pos += len + 1;
    return retval;
}


unsigned int Parser::pop3F1Count() const
{
    unsigned int val = popInteger();
    return (val / 1009) - 1;
}
