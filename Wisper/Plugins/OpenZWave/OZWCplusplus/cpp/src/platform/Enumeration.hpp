

#include <tuple>
#include <cstdio>
#include <vector>
#include <string>
#include <cstring>
#include <stdexcept>
#include <functional>
#include <boost/preprocessor.hpp>
#include <boost/preprocessor/tuple/elem.hpp>
#include <boost/preprocessor/tuple/enum.hpp>
#include <boost/preprocessor/tuple/size.hpp>
#include <boost/preprocessor/control/if.hpp>
#include <boost/preprocessor/seq/for_each.hpp>
#include <boost/preprocessor/comparison/equal.hpp>

#include "Macros.hpp"

/*
  Usage
  DEFINE_ENUM_TYPE(OS, uint8_t, ((Windows,0))((Linux,1))((Apple,2)));

  int main()
  {
	OS::Enum t = OS::FromString("Windows");
	std::cout << OS::ToString(t) << "=" << t << std::endl;
	t = OS::FromString("Apple");
	std::cout << OS::ToString(t) << "=" << t << std::endl;
 	t = OS::FromString("Linux");
 	std::cout << OS::ToString(t) << "=" << t << std::endl;

	OS::ActOnValue(
			OS::Other,
			{
				{OS::Windows, [](){INFO_MESSAGE("Fuck yeah");}},
				{OS::Linux,   [](){INFO_MESSAGE("Fuck yeah");}},
				{OS::Apple,   [](){INFO_MESSAGE("Fuck off");}},
				{OS::Other,   [](){INFO_MESSAGE("Whaaaat ?");}}
			});
  }
 */
#ifndef YODIWO_ENUMERATION_TEMPLATE
#define YODIWO_ENUMERATION_TEMPLATE

#define TUPLE_1_HANDLE(elem) \
		BOOST_PP_TUPLE_ELEM(2, 0, elem),

#define TUPLE_2_HANDLE(elem) \
		BOOST_PP_TUPLE_ELEM(2, 0, elem)=BOOST_PP_TUPLE_ELEM(2, 1, elem),

#define X_TUPLE_TO_ENUM(r, data, elem) \
		BOOST_PP_IF(BOOST_PP_EQUAL(BOOST_PP_TUPLE_SIZE(elem),1), \
			TUPLE_1_HANDLE, TUPLE_2_HANDLE)(elem)

#define X_DEFINE_ENUM_WITH_STRING_CONVERSIONS_TOSTRING_CASE_TUPLE(r, data, elem)    \
    case BOOST_PP_TUPLE_ELEM(2, 0, elem) : return BOOST_PP_STRINGIZE(BOOST_PP_TUPLE_ELEM(2, 0, elem));

#define X_DEFINE_ENUM_WITH_STRING_CONVERSIONS_TOSTRING_CASE_TUPLE2(r, data, elem)    \
	result.push_back(std::string(BOOST_PP_STRINGIZE(BOOST_PP_TUPLE_ELEM(2, 0, elem))));

#define X_DEFINE_ENUM_PUSH_BACK(r, data, elem)    \
	result.push_back(BOOST_PP_TUPLE_ELEM(2, 0, elem));

#define X_DEFINE_STRING_TO_ENUMERATION_TUPLE(r, data, elem)    \
    if (strcmp(value, BOOST_PP_STRINGIZE(BOOST_PP_TUPLE_ELEM(2, 0, elem))) == 0) return BOOST_PP_TUPLE_ELEM(2, 0, elem);

#define DEFINE_ENUM_TYPE(name, type, enumerators) 						      \
	struct name { 															  \
		enum Enum : type {                                                    \
			BOOST_PP_SEQ_FOR_EACH(X_TUPLE_TO_ENUM,_,enumerators)              \
		}; 																	  \
																			  \
		static const char* ToString(Enum v)               		              \
		{                                                                     \
			switch (v)                                                        \
			{                                                                 \
				BOOST_PP_SEQ_FOR_EACH(                                        \
					X_DEFINE_ENUM_WITH_STRING_CONVERSIONS_TOSTRING_CASE_TUPLE,\
					_,                                                        \
					enumerators                                               \
				);                                                            \
				default: return "[Unknown " BOOST_PP_STRINGIZE(name) "]";     \
			}                                                                 \
		}																	  \
																			  \
		static std::vector<std::string> Strings()    						  \
		{																	  \
			std::vector<std::string> result;								  \
			BOOST_PP_SEQ_FOR_EACH(                                            \
				X_DEFINE_ENUM_WITH_STRING_CONVERSIONS_TOSTRING_CASE_TUPLE2,   \
				_,                                                            \
				enumerators                                                   \
			);                                                                \
			return result;													  \
		}																	  \
																			  \
		static std::vector<Enum> Values()    								  \
		{																	  \
			std::vector<Enum> result;								          \
			BOOST_PP_SEQ_FOR_EACH(                                            \
				X_DEFINE_ENUM_PUSH_BACK,   									  \
				_,                                                            \
				enumerators                                                   \
			);                                                                \
			return result;													  \
		}																	  \
																			  \
		static Enum FromString(const char* value)							  \
		{																	  \
			char _output_buffer[2048] = {0};                                  \
			BOOST_PP_SEQ_FOR_EACH(                            				  \
				X_DEFINE_STRING_TO_ENUMERATION_TUPLE,            			  \
				_,                              							  \
				enumerators                                           		  \
			);                                                                \
            snprintf(_output_buffer, sizeof(_output_buffer), "[EXCEPTION][%s:%s:%d]:Unknown enumeration type [%s]\n", __FILE__, __FUNCTION__,  __LINE__, value); \
            throw Yodiwo::GenericException(_output_buffer);                   \
		}																	  \
																			  \
		static Enum FromString(const std::string& str)						  \
		{																	  \
			auto value = str.c_str();										  \
			return FromString(value);										  \
		}																	  \
																			  \
		static type ToType(Enum value)							      		  \
		{																	  \
			return static_cast<type>(value);								  \
		}																	  \
																			  \
		static Enum FromType(type value)							          \
		{																	  \
			return static_cast<Enum>(value);								  \
		}																	  \
																			  \
		struct ValueAction {												  \
			Enum Value;														  \
			std::function<void()> Action;									  \
		};																	  \
																			  \
		static void ActOnValue( 														\
				Enum value,  															\
				const std::vector<ValueAction>& valueActions)							\
		{																				\
			for (auto& valueAction : valueActions)										\
			{																			\
				if (value == valueAction.Value)											\
				{																		\
					valueAction.Action();												\
					return;																\
				}																		\
			}																			\
			char _output_buffer[2048] = {0};                                  			\
            snprintf(_output_buffer, sizeof(_output_buffer), "[EXCEPTION][%s:%s:%d]:Unknown enumeration type\n", __FILE__, __FUNCTION__,  __LINE__); \
            throw Yodiwo::GenericException(_output_buffer);                   			\
		} 																	  \
																			  \
		struct ValueActor {												      \
			std::vector<ValueAction> ValueActions;					          \
			ValueActor(const std::vector<ValueAction>& valueActions) 		  \
					: ValueActions(valueActions){		  					  \
			}																  \
																			  \
			void Act(Enum value)											  \
			{																  \
				ActOnValue(value, ValueActions);						      \
			}																  \
		};																	  \
	};

#endif

