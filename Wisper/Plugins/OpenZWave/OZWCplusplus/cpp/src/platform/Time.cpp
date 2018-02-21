#include <ctime>
#include <sys/time.h>

#include "Time.hpp"

using namespace Yodiwo::Utilities;

///////////////////////////////////////////////////////////////////

double
Time::GetWallTime()
{
	struct timeval time;

	if (gettimeofday(&time, NULL) != 0)
	{
		//  Handle error
		return 0;
	}
	return (double)time.tv_sec + (double)time.tv_usec * .000001;
}

///////////////////////////////////////////////////////////////////

double
Time::GetCpuTime()
{
	return (double)clock() / CLOCKS_PER_SEC;
}

///////////////////////////////////////////////////////////////////

std::string
Time::CurrentDateTimeBase(const std::string& timeFormat)
{
	time_t now = std::time(0);
	char buffer[64] = {0};
	struct tm tstruct = {0};
	tstruct = *localtime(&now);
	std::strftime(buffer, sizeof(buffer), timeFormat.c_str(), &tstruct);
	return std::string(buffer);
}

std::string
Time::CurrentDateTime()
{
	return CurrentDateTimeBase("%Y-%m-%d.%X");
}

///////////////////////////////////////////////////////////////////

std::string
Time::DateStamp()
{
	return CurrentDateTimeBase("%Y_%m_%d_%H_%M_%S");
}

///////////////////////////////////////////////////////////////////

std::string
Time::TimeStamp()
{
	return CurrentDateTimeBase("%H:%M:%S");
}

///////////////////////////////////////////////////////////////////

std::string
Time::YodiwoCompliantTimeStamp()
{
	return CurrentDateTimeBase("%Y-%m-%dT%H:%M:%S%z");
}

///////////////////////////////////////////////////////////////////
