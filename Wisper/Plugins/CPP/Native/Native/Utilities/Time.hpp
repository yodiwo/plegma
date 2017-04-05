#pragma once

#include <string>

namespace Yodiwo
{
	namespace Utilities
	{
		namespace Time
		{
			double GetCpuTime();
			double GetWallTime();
			std::string TimeStamp();
			std::string DateStamp();
			std::string CurrentDateTime();
			std::string YodiwoCompliantTimeStamp();
			std::string CurrentDateTimeBase(const std::string& timeFormat);
		}
	};
}
