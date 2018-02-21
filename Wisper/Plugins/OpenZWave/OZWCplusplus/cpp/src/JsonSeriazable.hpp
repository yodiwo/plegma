#pragma once

#include "json.hpp"
#include <iostream>
#include <initializer_list>

#include "Utilities.hpp"

namespace Yodiwo
{
	using json = nlohmann::json;

	//=========================================================

	class JsonSeriazable
	{
	public:
		//-----------------------------------------
		JsonSeriazable() {}
		//-----------------------------------------
		JsonSeriazable(const json& input) {}
		//-----------------------------------------
		JsonSeriazable(const std::string& jsonString)
			: JsonSeriazable(json::parse(jsonString))
		{}
		//-----------------------------------------
		// Interface methods
		virtual ~JsonSeriazable() {}
		//-----------------------------------------
		virtual json toJson() const = 0;
		//-----------------------------------------
		friend std::ostream& operator<< (
			std::ostream& stream,
			JsonSeriazable& j)
		{
			stream << j.toJson().dump(4);
			return stream;
		}

		//-----------------------------------------

		static json Merge(
			const std::initializer_list<json> js)
		{
			return Utilities::SettingsParser::Merge(js);
		}

		//-----------------------------------------

		template <typename T>
		static json toJson(const std::vector<T>& array)
		{
			json result;

			for (auto item : array)
			{
				result.push_back(item.toJson());
			}

			return result;
		}

		//-----------------------------------------

		template <typename T>
		static T Factory(const std::string& filename)
		{
			if (Utilities::Filesystem::FileExists(filename) == false)
			{
				GENERIC_EXCEPTION("File [%s] does not exist", filename.c_str());
			}

			Utilities::SettingsParser parser(filename);
			return T(parser.get());
		}

		//-----------------------------------------
	};

	//=========================================================
};
