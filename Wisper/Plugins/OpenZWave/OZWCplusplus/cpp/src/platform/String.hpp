#pragma once

#include <set>
#include <regex>
#include <tuple>
#include <regex>
#include <cmath>
#include <random>
#include <vector>
#include <cstdio>
#include <thread>
#include <fstream>
#include <sstream>
#include <iomanip>
#include <cstring>
#include <cstdarg>
#include <iterator>
#include <iostream>
#include <exception>
#include <stdexcept>
#include <algorithm>
#include <functional>
#include <type_traits>
#include <unordered_map>

namespace Yodiwo
{
	namespace Utilities
	{
		namespace String
		{
			//=========================================================

			std::string
			Join(
					const std::vector<std::string>& tokens,
					const std::string& delimiter);

			//=========================================================

			std::string
			Format(
					const std::string& fmt_str, ...);

			//=========================================================

			std::vector<std::string>
			Split(
					const std::string& str,
					const std::vector<std::string>& delimiters);

			//=========================================================

			std::vector<std::string>
			Split(
					const std::string& str,
					const std::string& delimiter);

			//=========================================================

			std::string
			EscapeChar(
					char character);

			//=========================================================

			std::string
			EscapeString(
					const std::string& str);

			//=========================================================

			std::vector<std::string>
			EscapeStrings(
					const std::vector<std::string>& delimiters);

			//=========================================================

			bool
			IsAnInteger(
					const std::string& token);

			//=========================================================

			std::string
			ExtractRegion(
					const std::string& str,
					int from,
					int to);

			//=========================================================

			int
			ToInt(
					const std::string& str);

			//=========================================================

			double
			ToDouble(
					const std::string& str);

			//=========================================================

			/*
			 * Create random string of given length
			 */
			std::string
			Random(
					size_t length,
					bool includeUpper = true,
					bool includeLower = true,
					bool includeNumeric = true);

			//=========================================================

			std::string
			ToLower(const std::string& str);

			//=========================================================

			std::string
			ToUpper(const std::string& str);

			//=========================================================

			std::string
			HumanReadableByteCount(const long bytes, const bool si = false);

			//=========================================================
		};
	};
};
