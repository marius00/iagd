#ifndef HOOKLOG_H
#define HOOKLOG_H

#include <fstream>
#include <mutex>
#include <string>


class HookLog {
public:
    HookLog();
    ~HookLog();

	/// <param name="forceFlush">
	/// Flush the stream before returning. Required for anything we want to survive a
	/// hard crash: once setInitialized(true) has run the stream is buffered, so the
	/// tail of the log -- the interesting part when the game AVs -- is otherwise lost.
	/// </param>
	void out(std::wstring const& output, bool forceFlush = false);
	void out(const char* output, bool forceFlush = false);
	void setInitialized(bool b);

private:
	void writeRepeatSummary();

    std::wofstream m_out;
    std::wstring m_lastMessage;
    unsigned int m_lastMessageCount;
	bool m_initialized;

	// out() is called from the game's update thread, the render thread and both IA
	// polling threads. std::wofstream is not thread safe, and neither is assigning
	// m_lastMessage, so everything below the lock needs the lock.
	std::mutex m_mutex;
};

#endif // HOOKLOG_H
