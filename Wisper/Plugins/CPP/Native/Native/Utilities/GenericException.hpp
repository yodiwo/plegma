#pragma once

#include <string>
#include <iostream>
#include <exception>

namespace Yodiwo
{
	class GenericException :
			public std::exception
	{
		private:
			std::string m_message;
		public:
			GenericException(
					const std::string& message) :
						std::exception() , m_message(message)
			{

			}

			virtual const char*
			what() const throw()
			{
				return this->m_message.c_str();
			}
	};
};
