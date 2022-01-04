 #include "StdAfx.h"
#include "DataQueue.h"


DataItem::DataItem(unsigned long _type, unsigned int _size, const char* _data)
    : m_type(_type)
    , m_size(_size) {
    m_data.reset(new char[_size]);
    memcpy(m_data.get(), _data, _size);
}


unsigned long DataItem::type() const {
    return m_type;
}


char* DataItem::data() const {
    return m_data.get();
}


unsigned int DataItem::size() const {
    return m_size;
}


/************************************************************************/
