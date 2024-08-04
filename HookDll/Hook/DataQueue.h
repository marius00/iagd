#ifndef DATAQUEUE_H
#define DATAQUEUE_H

#include <queue>
#include <boost/shared_array.hpp>
#include <boost/thread.hpp>


class DataItem {
public:
    DataItem(unsigned long _type, unsigned int _size, const char* _data);

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
template <typename T>
class BaseDataQueue {
public:
    BaseDataQueue();
    ~BaseDataQueue();

    /// This pushes a copy of the specified data on the queue.
    void push(T item);
    bool push(T item, int limit);

    /// This pops the front of the queue.
    T pop();

    /// Returns true if no items on queue
	bool empty() const { return m_queue.empty(); }
	size_t size();

private:
    typedef std::queue<T> DataItemQueue;
    DataItemQueue m_queue;
    boost::mutex m_mutex;
};

template <typename T>
BaseDataQueue<T>::BaseDataQueue() {}


template <typename T>
BaseDataQueue<T>::~BaseDataQueue() {}

template <typename T>
void BaseDataQueue<T>::push(T item) {
    boost::lock_guard<boost::mutex> guard(m_mutex);
    m_queue.push(item);
}

template <typename T>
bool BaseDataQueue<T>::push(T item, int limit) {
	boost::lock_guard<boost::mutex> guard(m_mutex);
	if (m_queue.size() <= limit) {
		m_queue.push(item);
		return true;
	}
	else {
		return false;
	}
}


template <typename T>
size_t BaseDataQueue<T>::size() {
	boost::lock_guard<boost::mutex> guard(m_mutex);
	return m_queue.size();
}



template <typename T>
T BaseDataQueue<T>::pop() {
    boost::lock_guard<boost::mutex> guard(m_mutex);
    T pData = m_queue.front();
    m_queue.pop();
    return pData;
}

typedef BaseDataQueue<DataItemPtr> DataQueue;

#endif // DATAQUEUE_H
