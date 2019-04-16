#include "StdAfx.h"
#include "DataQueue.h"


DataItem::DataItem(unsigned long _type, unsigned int _size, char* _data)
    : m_type(_type)
    , m_size(_size)
{
    m_data.reset(new char[_size]);
    memcpy(m_data.get(), _data, _size);
}


unsigned long DataItem::type() const
{
    return m_type;
}


char* DataItem::data() const
{
    return m_data.get();
}


unsigned int DataItem::size() const
{
    return m_size;
}


/************************************************************************/


DataQueue::DataQueue()
{
}


DataQueue::~DataQueue()
{
}


void DataQueue::push(DataItemPtr item)
{
    boost::lock_guard<boost::mutex> guard(m_mutex);
    m_queue.push(item);
}


DataItemPtr DataQueue::pop()
{
    boost::lock_guard<boost::mutex> guard(m_mutex);
    DataItemPtr pData = m_queue.front();
    m_queue.pop();
    return pData;
}
