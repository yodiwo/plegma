#include "MsgQueue.h"

MsgQueue::MsgQueue()
{
}

MsgQueue::~MsgQueue()
{
	Stop();
}

std::string MsgQueue::GetMessage()
{
	try
	{
		std::unique_lock<std::mutex> mlock(m_msgMutex);
		// Wait to have a message
		while (m_msgQueue.empty() == true && m_running == true)
		{
			std::printf("MsgQueue::GetMessage: queue is empty wait!\n");
			m_msgCondition.wait(mlock);
		}

		// If we have stoped and the queue is empty just return
		if (m_running == false && m_msgQueue.empty() == true)
		{
			std::printf("MsgQueue::GetMessage: RETURN NOTHING!\n");
			return std::string();
		}

		// Give the front item and remove it from the queue
		auto item = m_msgQueue.front();
		std::printf("MsgQueue::GetMessage: POP IT!\n");
		m_msgQueue.pop();
		return item;
	}
	catch (const std::exception& exp)
	{
		std::printf("MsgQueue::GetMessage: exception \n");
		exp.what();
	}

	return std::string();
}

bool MsgQueue::SendMessage(const std::string & msg)
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
		std::printf("MsgQueue::SendMessage: exception \n");
		exp.what();
		return false;
	}
}
