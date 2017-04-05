#pragma once

#include <queue>
#include <mutex>
#include <string>
#include <thread>
#include <stddef.h>
#include <condition_variable>

namespace Yodiwo
{
	class MsgQueue
	{
	private:
		std::mutex                  m_msgMutex;
		std::queue<std::string>		m_msgQueue; // All the messages are in json string form
		std::condition_variable     m_msgCondition;
		bool                        m_running = true;

	public:
		MsgQueue();
		~MsgQueue();

		//=========================================================

		void Stop() {
			if (m_running) {
				m_running = false;
				// Trigger the blocking mechanism to unblock
				m_msgCondition.notify_all();
			}
		}

		//=========================================================

		// Get Message from queue
		std::string GetMessage();
		bool SendMessage(const std::string &msg);

		//=========================================================
	};
}