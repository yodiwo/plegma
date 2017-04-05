#pragma once
#ifndef YODIWO_UTILITIES_MACROS_HPP
#define YODIWO_UTILITIES_MACROS_HPP

#include <cstdio>
#include <string>
#include <cstring>
#include <cstdarg>
#include <sys/stat.h>
#include <sys/types.h>
#include <sys/time.h>
#include <sys/sysinfo.h>
#include <linux/unistd.h>
#include <linux/kernel.h>

#include "Time.hpp"
#include "GenericException.hpp"

#include "TraceSystem.hpp"

//=========================================================

inline std::string
ExtractClassName(
		const std::string& prettyFunction)
{
	static const std::string DoubleDots = "::";
	auto colons = prettyFunction.find(DoubleDots);

    if (colons == std::string::npos)
    {
        return std::string(DoubleDots);
    }

    auto begin = prettyFunction.substr(0,colons).rfind(" ") + 1;
    auto end = colons - begin;

    return prettyFunction.substr(begin,end);
}

#define __CLASS_NAME__ ExtractClassName(__PRETTY_FUNCTION__)

//=========================================================

inline std::string
ExtractMethodName(const std::string& prettyFunction)
{
	static const std::string DoubleDots = "::";
    size_t colons = prettyFunction.find(DoubleDots);
    size_t begin = prettyFunction.substr(0,colons).rfind(" ") + 1;
    size_t end = prettyFunction.rfind("(") - begin;
    return prettyFunction.substr(begin,end) + "()";
}

#define __METHOD_NAME__ ExtractMethodName(__PRETTY_FUNCTION__)

//=========================================================
/*
 * This is bad practice
 * replace with something else
 */
#define __MESSAGE(DBG_LEVEL, STR, ...)\
    do{\
        char _msg[512] = {0}; \
        snprintf(_msg, sizeof(_msg) - 1, STR , ##__VA_ARGS__); \
        Yodiwo::Utilities::TraceSystem::Instance().Trace(DBG_LEVEL, __FILE__, __FUNCTION__,  __LINE__, _msg); \
    }while(0)

//=========================================================

#define INFO_MESSAGE(...)  __MESSAGE(Yodiwo::Utilities::TRACE_LEVEL::Enum::INFO, __VA_ARGS__)
#define ERROR_MESSAGE(...) __MESSAGE(Yodiwo::Utilities::TRACE_LEVEL::Enum::ERROR, __VA_ARGS__)
#define DEBUG_MESSAGE(...) __MESSAGE(Yodiwo::Utilities::TRACE_LEVEL::Enum::DEBUG, __VA_ARGS__)
#define WARNING_MESSAGE(...) __MESSAGE(Yodiwo::Utilities::TRACE_LEVEL::Enum::WARN, __VA_ARGS__)
#define TODO_MESSAGE() __MESSAGE(Yodiwo::Utilities::TRACE_LEVEL::Enum::WARN, "===============> NOT IMPLEMENTED YET <===============")

//=========================================================

#define __DIRECT_MESSAGE(DBG_LEVEL, STR, ...)\
	do{\
		struct timeval _tv; char _stime[32] = {0}; \
		gettimeofday(&_tv, NULL); \
		std::strftime(_stime, sizeof(_stime) / sizeof(*_stime), "%m-%d %H:%M:%S", gmtime(&_tv.tv_sec)); \
		std::fprintf(stderr, DBG_LEVEL "[%s.%03d][%s:%s:%d]:" STR "\n", _stime, (int)(_tv.tv_usec / 1000), __FILE__, __FUNCTION__,  __LINE__, ##__VA_ARGS__); \
		std::fflush(stderr); \
	}while(0)

//=========================================================

#define INFO_DIRECT_MESSAGE(...)  __DIRECT_MESSAGE("[INFO]", __VA_ARGS__)
#define ERROR_DIRECT_MESSAGE(...) __DIRECT_MESSAGE("[ERROR]", __VA_ARGS__)
#define DEBUG_DIRECT_MESSAGE(...) __DIRECT_MESSAGE("[DEBUG]", __VA_ARGS__)
#define WARNING_DIRECT_MESSAGE(...) __DIRECT_MESSAGE("[DEBUG]", __VA_ARGS__)

//=========================================================

#define OUTPUT_MESSAGE(STR, ...)\
    do{\
        fprintf(stdout, STR "\n", ##__VA_ARGS__); \
        fflush(stdout); \
    }while(0)

//=========================================================

#define CONDITIONAL_DEBUG_MESSAGE(_condition, ...) \
    do{\
        if (_condition) \
        { \
            DEBUG_MESSAGE(__VA_ARGS__); \
        }\
    }while(0)

//=========================================================

#define EXCEPTION_MESSAGE_BUFFER 2048
#define GENERIC_EXCEPTION(STR, ...)\
    do{\
        char _output_buffer[EXCEPTION_MESSAGE_BUFFER] = {0};  \
        snprintf(_output_buffer, sizeof(_output_buffer), "[EXCEPTION][%s:%s:%d]:" STR , __FILE__, __FUNCTION__,  __LINE__, ##__VA_ARGS__); \
        Yodiwo::Utilities::TraceSystem::Instance().Trace(Yodiwo::Utilities::TRACE_LEVEL::Enum::CRITICAL, __FILE__, __FUNCTION__,  __LINE__, _output_buffer); \
        throw Yodiwo::GenericException(_output_buffer); \
    }while(0)

//=========================================================

#define GENERIC_CHECK(_expr, ...)\
    do{\
        if ((_expr) == false)\
        {\
            GENERIC_EXCEPTION(__VA_ARGS__);\
        }\
    }while(0)

//=========================================================

#endif
