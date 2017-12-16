#ifndef DATAQUEUE_H
#define DATAQUEUE_H

#include <queue>
#include <boost/shared_ptr.hpp>
#include <boost/shared_array.hpp>
#include <boost/thread.hpp>


class DataItem
{
public:
    DataItem(unsigned long _type, unsigned int _size, char* _data);

    unsigned long type() const;
    char* data() const;
    unsigned int size() const;

private:
    boost::shared_array<char> m_data;
    unsigned int m_size;
    unsigned long m_type;
};

typedef boost::shared_ptr<DataItem> DataItemPtr;


/**
 * This is a data queue class that supports multiple threads.
 */
class DataQueue {
public:
    DataQueue();
    ~DataQueue();

    /// This pushes a copy of the specified data on the queue.
    void push(DataItemPtr item);

    /// This pops the front of the queue.
    DataItemPtr pop();

    /// Returns true if no items on queue
    bool empty() const { return m_queue.empty(); }

private:
    typedef std::queue<DataItemPtr> DataItemQueue;
    DataItemQueue m_queue;
    boost::mutex m_mutex;
    //typedef std::queue<COPYDATASTRUCT*> COPYDATASTRUCTQUEUE;
    //COPYDATASTRUCTQUEUE g_dataQueue;
};

#endif // DATAQUEUE_H
