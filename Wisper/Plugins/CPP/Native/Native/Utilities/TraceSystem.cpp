#include "TraceSystem.hpp"

using namespace Yodiwo::Utilities;

///////////////////////////////////////////////////////////////

TraceSystem&
TraceSystem::Instance()
{
	/*
	 * Since it's a static variable, if the class has already been created,
	 * It won't be created again.
	 * And it **is** thread-safe in C++11.
	 */
	static TraceSystem myInstance;

	return myInstance;
}

///////////////////////////////////////////////////////////////

void
TraceSystem::StopTrace()
{
	Instance().Stop();
}

///////////////////////////////////////////////////////////////


void
TraceSystem::SetMessageListener(
		std::function<void(TraceMsg& msg)> func)
{
	Instance().MessageHandler = func;
}

TraceSystem::~TraceSystem()
{
	Stop();
}

///////////////////////////////////////////////////////////////

void
TraceSystem::Stop()
{
	m_running = false;

	m_msgs_condition.notify_all();

	if (m_thread != nullptr && m_thread->joinable() == true)
	{
		m_thread->join();
	}
}

///////////////////////////////////////////////////////////////

void
TraceSystem::Trace(
	TRACE_LEVEL::Enum dbg_level,
	std::string file,
	std::string func_name,
	int line,
	std::string mesg)
{
	TraceMsg msg;
	msg.dbg_level = dbg_level;
	msg.file      = file;
	msg.func_name = func_name;
	msg.line      = line;
	msg.mesg      = mesg;
	gettimeofday(&msg.tv, NULL);

	std::unique_lock<std::mutex> mlock(m_msgs_mutex);
	m_msgs.push(std::move(msg));
	mlock.unlock();
	m_msgs_condition.notify_one();
}

///////////////////////////////////////////////////////////////

TraceSystem::TraceSystem() :
					m_running(true),
					PrintInStdErr(true),
					MessageHandler(nullptr)
{
	m_thread = std::make_shared<std::thread>(
		[this]()
		{
			while (m_running == true || m_msgs.empty() == false)
			{
				// Get the msg for showing
				std::unique_lock<std::mutex> lock(m_msgs_mutex);

				m_msgs_condition.wait(
						lock,
						[this]
						{
							return m_running == false || m_msgs.empty() == false;
						});

				if (m_running == false && m_msgs.empty() == true)
				{
					// exit from processing thread
					return;
				}

				auto msg = std::move(m_msgs.front());
				m_msgs.pop();
				lock.unlock();

				// Print the message or send it to logging server
				if (PrintInStdErr == true)
				{
					char _stime[32] = {0};
					std::strftime(_stime, sizeof(_stime) / sizeof(*_stime),
							 "%m-%d %H:%M:%S",
							 gmtime(&msg.tv.tv_sec));
					std::fprintf(
							stderr,
							"[%s][%s.%03d][%s:%s:%d] %s\n",
							TRACE_LEVEL::ToString(msg.dbg_level),
							_stime,
							int(msg.tv.tv_usec / 1000),
							msg.file.c_str(),
							msg.func_name.c_str(),
							msg.line,
							msg.mesg.c_str());
					std::fflush(stderr);
				}

				// Call external
				if (MessageHandler != nullptr)
				{
					try
					{
						MessageHandler(msg);
					}
					catch(const std::exception& exp)
					{
						std::fprintf(stderr, "Exception: %s", exp.what());
					}
				}
			}
		});

	// Constructor code goes here.
	Trace(TRACE_LEVEL::Enum::INFO, __FILE__, __FUNCTION__, __LINE__, "Trace System init.");
}

///////////////////////////////////////////////////////////////

