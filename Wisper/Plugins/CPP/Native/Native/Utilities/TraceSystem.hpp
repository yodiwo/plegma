#pragma once

#include <ctime>
#include <queue>
#include <mutex>
#include <thread>
#include <memory>
#include <cstdio>
#include <cstring>
#include <cstdlib>
#include <iostream>
#include <condition_variable>

#include <sys/stat.h>
#include <sys/types.h>
#include <sys/time.h>
#include <sys/sysinfo.h>

#include "Enumeration.hpp"

///////////////////////////////////////////////////////////////

namespace Yodiwo
{
	namespace Utilities
	{
		// The levels are based on severity levels of linux
		DEFINE_ENUM_TYPE(
			TRACE_LEVEL,
			int,
			((EMERG, 0))
			((ALERT, 1))
			((CRITICAL, 2))
			((ERROR, 3))
			((WARN, 4))
			((NOTICE, 5))
			((INFO, 6))
			((DEBUG, 7)));

		///////////////////////////////////////////////////////////////

		class TraceMsg
		{
		public:
			int line;
			std::string file;
			std::string mesg;
			std::string func_name;
			struct timeval tv;
			TRACE_LEVEL::Enum dbg_level;
		};

		///////////////////////////////////////////////////////////////

		class TraceSystem
		{
		public:
			static void StopTrace();
			static TraceSystem& Instance();
			static void SetMessageListener(std::function<void(TraceMsg& msg)> func);
			// delete copy and move constructors and assign operators
			TraceSystem(TraceSystem &&) = delete; // Move construct
			TraceSystem(TraceSystem const &) = delete; // Copy construct
			TraceSystem &operator=(TraceSystem &&) = delete; // Move assign
			TraceSystem &operator=(TraceSystem const &) = delete; // Copy assign
		protected:
			TraceSystem();
			~TraceSystem();
			void Stop();
		private:
			bool m_running;
			std::mutex m_msgs_mutex;
			std::queue<TraceMsg> m_msgs;
			std::shared_ptr<std::thread> m_thread;
			std::condition_variable m_msgs_condition;
		public:
			bool PrintInStdErr;
			std::function<void(TraceMsg &msg)> MessageHandler;

			void Trace(
				TRACE_LEVEL::Enum dbg_level,
				std::string file,
				std::string func_name,
				int line,
				std::string mesg);
		};
	};
};

///////////////////////////////////////////////////////////////

