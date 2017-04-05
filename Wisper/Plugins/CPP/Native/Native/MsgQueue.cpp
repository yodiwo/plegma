#include "MsgQueue.hpp"
#include "Utilities/Macros.hpp"

Yodiwo::MsgQueue::MsgQueue()
{
}


Yodiwo::MsgQueue::~MsgQueue()
{
	Stop();
}

std::string Yodiwo::MsgQueue::GetMessage()
{

	try
	{
		std::unique_lock<std::mutex> mlock(m_msgMutex);

		// Wait to have a message
		while (m_msgQueue.empty() == true && m_running == true)
		{
			m_msgCondition.wait(mlock);
		}

		// If we have stoped and the queue is empty just return
		if (m_running == false && m_msgQueue.empty() == true)
		{
			return std::string();
		}

		// Give the front item and remove it from the queue
		auto item = m_msgQueue.front();
		m_msgQueue.pop();
		return item;
	}
	catch (const std::exception& exp)
	{
		ERROR_MESSAGE("Failed to get msg. Exception: %s", exp.what());
	}

	return std::string();
}

bool Yodiwo::MsgQueue::SendMessage(const std::string & msg)
{
	try
	{
		// Push the msg to queue and return the C# is going to get the message async 
		std::unique_lock<std::mutex> mlock(m_msgMutex);
		m_msgQueue.push(msg);
		mlock.unlock();
		m_msgCondition.notify_one();
		return true;
	}
	catch (const std::exception& exp)
	{
		ERROR_MESSAGE("Failed to send msg. Exception: %s", exp.what());
	}
	return false;
}
