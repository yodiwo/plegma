#pragma once

#include <json.hpp>

#include <regex>
#include <vector>
#include <string>
#include <fstream>
#include <sstream>
#include <iostream>
#include <iterator>
#include <exception>
#include <stdexcept>
#include <algorithm>
#include <unordered_map>

#include "Utilities.hpp"

namespace Yodiwo
{
	namespace Utilities
	{
		//=========================================================

		using json = nlohmann::json;

		class SettingsParser
		{
		protected:
			//-----------------------------------------
			json m_json;
			bool m_initialized;
		public:
			//-----------------------------------------
			~SettingsParser();
			//-----------------------------------------
			SettingsParser();
			//-----------------------------------------
			SettingsParser(const json& settings);
			//-----------------------------------------
			SettingsParser(const char* filename);
			//-----------------------------------------
			SettingsParser(const std::string& filename);
			//-----------------------------------------
			bool load(const json& settings);
			//-----------------------------------------
			bool load(const char* filename);
			//-----------------------------------------
			bool load(const std::string& filename);
			//-----------------------------------------
			json get();
			//-----------------------------------------
			json get(const char* header);
			//-----------------------------------------
			json get(const std::string& header);
			//-----------------------------------------
			bool contains(const std::string& header);
			//-----------------------------------------
			template<typename T>
			T value(
				const std::string& header)
			{
				try
				{
					return get(header).get<T>();
				}
				catch (const std::exception& exp)
				{
					throw std::domain_error("SettingsParser: Value {" + std::string(header) + "} failed with: {" + std::string(exp.what()) + "}");
				}
			}
			//-----------------------------------------
			template<typename T>
			T value(
				const char* header)
			{
				try
				{
					return value<T>(std::string(header));
				}
				catch (const std::exception& exp)
				{
					throw std::domain_error("SettingsParser: Value {" + std::string(header) + "} failed with: {" + std::string(exp.what()) + "}");
				}
			}
			//-----------------------------------------
			template<typename T>
			std::vector<T> valueArray(
				const std::string& header)
			{
				try
				{
					std::vector<T> array;

					for (auto item : get(header))
					{
						array.push_back(T(item));
					}
					return array;
				}
				catch (const std::exception& exp)
				{
					throw std::domain_error("SettingsParser: ValueArray {" + std::string(header) + "} failed with: {" + std::string(exp.what()) + "}");
				}
			}
			//-----------------------------------------
			template<typename T>
			std::vector<std::vector<T>> valueArrayArray(
				const std::string& header)
			{
				try
				{
					std::vector<std::vector<T>> array;

					for (auto item1 : get(header))
					{
						std::vector<T> array1;
						for (auto item : item1)
						{
							array1.push_back(T(item));
						}
						array.push_back(array1);
					}
					return array;
				}
				catch (const std::exception& exp)
				{
					throw std::domain_error("SettingsParser: ValueArrayArray {" + std::string(header) + "} failed with: {" + std::string(exp.what()) + "}");
				}
			}
			//-----------------------------------------
			template<typename T>
			std::vector<T> valueArray()
			{
				return valueArray<T>("");
			}
			//-----------------------------------------
			/*
			* If header exists in json, set toValue to it
			*/
			template<typename T>
			void
				GetIfExists(const std::string& header, T& toValue)
			{
				if (contains(header) == true)
				{
					toValue = this->value<T>(header);
				}
			}
			//-----------------------------------------
			template<typename K, typename T>
			void
				GetIfExistsCast(const std::string& header, T& toValue)
			{
				if (contains(header) == true)
				{

					toValue = static_cast<T>(this->value<K>(header));
				}
			}
			//-----------------------------------------
			template<typename T>
			void
				GetIfExists(const std::string& header, std::vector<T>& toValue)
			{
				if (contains(header) == true)
				{
					toValue = this->valueArray<T>(header);
				}
			}
			//-----------------------------------------
			/*
			* If header exists in json, set toValue to it
			* otherwise set default
			*/
			template<typename T>
			void
				GetIfExists(const std::string& header, T& toValue, T defaultValue)
			{
				if (contains(header) == true)
				{
					toValue = this->value<T>(header);
				}
				else
				{
					toValue = defaultValue;
				}
			}
			//-----------------------------------------
			static bool Save(
				const json& input,
				const std::string& filename);
			//-----------------------------------------
			template<typename T>
			static std::map<std::string, T>
				ToMap(const json& input)
			{
				std::map<std::string, T> result;

				for (auto iter = input.begin();
					iter != input.end();
					iter++)
				{
					try
					{
						result[iter.key()] = iter.value().get<T>();
					}
					catch (const std::exception& exp)
					{
						ERROR_MESSAGE("Exception: %s", exp.what());
						GENERIC_EXCEPTION("Could not convert [%s] to template class", iter.key().c_str());
					}
				}

				return result;
			}
			//-----------------------------------------
			template<typename T, bool Constructor = true>
			static std::vector<T>
				ToArray(const std::string& filename)
			{
				SettingsParser tmp(filename);
				return ToArray<T, Constructor>(tmp.get());
			}
			//-----------------------------------------
			template<typename T, bool Constructor = true>
			static std::vector<T>
				ToArray(const char* filename)
			{
				SettingsParser tmp(std::string(filename));
				return ToArray<T, Constructor>(tmp.get());
			}
			//-----------------------------------------
			template<typename T, bool Constructor = true>
			static std::vector<T>
				ToArray(const json& input)
			{
				std::vector<T> result;

				for (auto iter = input.begin();
					iter != input.end();
					iter++)
				{
					try
					{
						auto valueJson = iter.value();
						result.push_back(T(valueJson));
					}
					catch (const std::exception& exp)
					{
						ERROR_MESSAGE("Exception: %s", exp.what());
						GENERIC_EXCEPTION("Could not convert [%s]", iter.key().c_str());
					}
				}

				return result;
			}
			//-----------------------------------------
			template<typename T, bool Constructor = true>
			static std::vector<std::shared_ptr<T>>
				ToArraySharePtr(const json& input)
			{
				std::vector<std::shared_ptr<T>> result;

				for (auto iter = input.begin();
					iter != input.end();
					iter++)
				{
					try
					{
						auto valueJson = iter.value();
						result.push_back(std::make_shared<T>(valueJson));
					}
					catch (const std::exception& exp)
					{
						ERROR_MESSAGE("Exception: %s", exp.what());
						GENERIC_EXCEPTION("Could not convert [%s]", iter.key().c_str());
					}
				}

				return result;
			}
			//-----------------------------------------
			static json
				Merge(const json& lhs, const json& rhs);
			//-----------------------------------------
			static json
				Merge(const std::vector<json>& inputs);
			//-----------------------------------------
			static json
				Merge(const std::vector<std::string>& inputs);
			//-----------------------------------------
		};

		//=========================================================
	};
};
