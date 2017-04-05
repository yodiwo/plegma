#include "String.hpp"
#include "Vector.hpp"
#include "Constants.hpp"

using namespace Yodiwo::Utilities;

//=========================================================

std::string
String::Join(
		const std::vector<std::string>& tokens,
		const std::string& delimiter)
{
    std::stringstream ss;
    ss << tokens.front();
    std::for_each(
          begin(tokens) + 1,
          end(tokens),
          [&](const std::string& element)
		  {
    			ss << delimiter << element;
		  });
    return ss.str();
}

//=========================================================

std::string
String::Format(
		const std::string& fmt_str, ...)
{
    /* Reserve two times as much as the length of the fmt_str */
    int final_n, n = ((int) fmt_str.size()) * 2;
    std::string str;
    std::unique_ptr<char[]> formatted;
    va_list ap;

    while (1)
    {
    	/* Wrap the plain char array into the unique_ptr */
        formatted.reset(new char[n]);
        strcpy(&formatted[0], fmt_str.c_str());
        va_start(ap, fmt_str);
        final_n = vsnprintf(&formatted[0], n, fmt_str.c_str(), ap);
        va_end(ap);
        if (final_n < 0 || final_n >= n)
        {
            n += abs(final_n - n + 1);
        }
        else
        {
            break;
        }
    }

    return std::string(formatted.get());
}

//=========================================================

std::vector<std::string>
String::Split(
		const std::string& str,
		const std::vector<std::string>& delimiters)
{
    std::regex rgx(Join(EscapeStrings(delimiters), "|"));
    std::sregex_token_iterator
		first{begin(str), end(str), rgx, -1},
        last;

    return {first, last};
}

//=========================================================

std::vector<std::string>
String::Split(
		const std::string& str,
		const std::string& delimiter)
{
    std::vector<std::string> delimiters = {delimiter};
    return Split(str, delimiters);
}

//=========================================================

std::string
String::EscapeChar(
		char character)
{
    static const std::unordered_map<char, std::string>
    	EscapedSpecialCharacters = {
          {'.',  "\\."},
          {'|',  "\\|"},
          {'*',  "\\*"},
          {'?',  "\\?"},
          {'+',  "\\+"},
          {'(',  "\\("},
          {')',  "\\)"},
          {'{',  "\\{"},
          {'}',  "\\}"},
          {'[',  "\\["},
          {']',  "\\]"},
          {'^',  "\\^"},
          {'$',  "\\$"},
          {'\\', "\\\\"}
    };

    auto it = EscapedSpecialCharacters.find(character);

    if (it == EscapedSpecialCharacters.end())
    {
        return std::string(1, character);
    }

    return it->second;
}

//=========================================================

std::string
String::EscapeString(
		const std::string& str)
{
    std::stringstream ss;
    std::for_each(
    		begin(str),
			end(str),
			[&ss](const char character)
			{
    			ss << EscapeChar(character);
			});
    return ss.str();
}

//=========================================================

std::vector<std::string>
String::EscapeStrings(
		const std::vector<std::string>& delimiters)
{
    return Vector::VectorMap<std::string>(delimiters, EscapeString);
}

//=========================================================

bool
String::IsAnInteger(
		const std::string& token)
{
    const std::regex e(INTEGER_REGEX);
    return std::regex_match(token, e);
}

//=========================================================

std::string
String::ExtractRegion(
		const std::string& str,
		int from,
		int to)
{
	if (to < from)
	{
		GENERIC_EXCEPTION("to must be < from");
	}

    std::string region = "";
    int regionSize = to - from;
    return str.substr(from, regionSize);
}

//=========================================================

int
String::ToInt(
		const std::string &str)
{
    std::string::size_type sz;
    return std::stoi(str, &sz);
}

//=========================================================

double
String::ToDouble(
		const std::string &str)
{
    std::string::size_type sz;
    return std::stod(str, &sz);
}

//=========================================================

std::string
String::Random(
		size_t length,
		bool includeUpper,
		bool includeLower,
		bool includeNumeric)
{
	static const std::vector<char> numeric =
		{'0','1','2','3','4','5','6','7','8','9'};
	static const std::vector<char> upper =
		{'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I',
		 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
		 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'};
	static const std::vector<char> lower =
		{'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i',
		 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
		 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'};

	if (length <= 0)
	{
		GENERIC_EXCEPTION("length must be > 0");
	}

	std::vector<std::vector<char>> vocabularies;

	if (includeUpper == false &&
		includeLower == false &&
		includeNumeric == false)
	{
		GENERIC_EXCEPTION("At least one flag must be true");
	}

	if (includeUpper == true)
	{
		vocabularies.push_back(upper);
	}

	if (includeLower == true)
	{
		vocabularies.push_back(lower);
	}

	if (includeNumeric == true)
	{
		vocabularies.push_back(numeric);
	}

	std::stringstream ss;

	for (size_t i = 0; i < length; i++)
	{
		auto j = std::rand() % vocabularies.size();
		auto& voc = vocabularies.at(j);
		auto k = std::rand() % voc.size();
		ss << voc.at(k);
	}

	return ss.str();
}

//=========================================================

std::string
String::ToLower(const std::string& str)
{
	std::string result;
	std::transform(str.begin(), str.end(), result.begin(), ::tolower);
	return result;
}

//=========================================================

std::string
String::ToUpper(const std::string& str)
{
	std::string result;
	std::transform(str.begin(), str.end(), result.begin(), ::toupper);
	return result;
}

//=========================================================

std::string
String::HumanReadableByteCount(
		const long bytes,
		const bool si)
{
    static const std::string SiUnits = std::string("kMGTPE");
    static const std::string NonSiUnits = std::string("KMGTPE");
    const int unit = si ? 1000 : 1024;

    if (bytes < unit)
    {
    	return std::to_string(bytes) + " B";
    }

    int exp = (int)(std::log(bytes) / std::log(unit));
    std::string pre =
    		std::string() +
			(si ? SiUnits.at(exp-1) : NonSiUnits.at(exp-1)) +
			(si ? "" : "i");
    char buff[128] = {0};
    float r =  bytes / std::pow(unit, exp);
    snprintf(buff, sizeof(buff), "%.1f %sB", r, pre.c_str());
    return std::string(buff);
}

//=========================================================
