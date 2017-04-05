#include "SettingsParser.hpp"

using namespace Yodiwo::Utilities;

//=========================================================

SettingsParser::~SettingsParser()
{}

//=========================================================

SettingsParser::SettingsParser(
	const char* filename) : SettingsParser(std::string(filename)) {}

//=========================================================

SettingsParser::SettingsParser(
	const std::string& filename)
{
	load(filename);
}

//=========================================================

SettingsParser::SettingsParser()
{
	m_initialized = false;
}

//=========================================================

SettingsParser::SettingsParser(
	const json& settings)
{
	load(settings);
}

//=========================================================

json
SettingsParser::get()
{
	return get("");
}

//=========================================================

json
SettingsParser::get(
	const char* header)
{
	std::string strHeader(header);
	return get(strHeader);
}

//=========================================================

json
SettingsParser::get(
	const std::string& header)
{
	if (m_initialized == false)
	{
		GENERIC_EXCEPTION("Settings parse is not initialized");
	}

	if (header.empty())
	{
		return m_json;
	}

	try
	{
		json settings;

		// Iterate json values and parse structures
		for (auto iter = m_json.begin();
			iter != m_json.end();
			iter++)
		{
			if (iter.key() == header)
			{
				return iter.value();
			}
		}

		return settings;
	}
	catch (const std::exception& exp)
	{
		throw std::domain_error("SettingsParser.cpp: Get {" + std::string(header) + "} failed with: {" + std::string(exp.what()) + "}");
	}
}

//=========================================================

bool
SettingsParser::contains(
	const std::string& header)
{
	if (m_initialized == false)
	{
		GENERIC_EXCEPTION("Settings parse is not initialized");
	}

	if (header.empty())
	{
		return true;
	}

	if (m_json.find(header) != m_json.end())
	{
		return true;
	}

	return false;
}

//=========================================================

bool
SettingsParser::load(
	const json& settings)
{
	m_json = settings;
	m_initialized = true;
	return m_initialized;
}

//=========================================================

bool
SettingsParser::load(
	const char* filename)
{
	return load(std::string(filename));
}

//=========================================================

bool
SettingsParser::load(
	const std::string& filename)
{
	std::string jsonString;
	m_initialized = false;

	if (Utilities::Filesystem::FileExists(filename) == false)
	{
		GENERIC_EXCEPTION("Settings file [%s] does not exist", filename.c_str());
	}

	try
	{
		jsonString = Utilities::Filesystem::FileToString(filename);
	}
	catch (const std::exception& exp)
	{
		ERROR_MESSAGE("Exception %s", exp.what());
		GENERIC_EXCEPTION("Failed to read settings file [%s]", filename.c_str());
	}

	try
	{
		m_json = json::parse(jsonString);
	}
	catch (const std::exception& exp)
	{
		ERROR_MESSAGE("Exception %s", exp.what());
		GENERIC_EXCEPTION("Failed to parse settings file [%s]", filename.c_str());
	}

	m_initialized = true;
	return m_initialized;
}

//=========================================================

bool
SettingsParser::Save(
	const json& input,
	const std::string& filename)
{
	std::ofstream outputFile;
	outputFile.open(filename.c_str());
	outputFile << input.dump(4);
	outputFile.close();
	return true;
}

//=========================================================

json
SettingsParser::Merge(
	const json& lhs,
	const json& rhs)
{
	if (lhs.empty() == true &&
		rhs.empty() == true)
	{
		return json{};
	}

	if (lhs.empty() == false &&
		rhs.empty() == true)
	{
		return lhs;
	}

	if (lhs.empty() == true &&
		rhs.empty() == false)
	{
		return rhs;
	}

	json result = lhs;

	for (auto iter = rhs.begin();
		iter != rhs.end();
		iter++)
	{
		auto lhsIter = lhs.find(iter.key());

		if (lhsIter == lhs.end() ||
			lhsIter->is_null() == true)
		{
			result[iter.key()] = iter.value();
			continue;
		}

		if (lhsIter->is_primitive() == true &&
			iter->is_primitive() == true)
		{
			result[iter.key()] = iter.value();
			continue;
		}

		if (lhsIter->is_array() == true &&
			iter->is_array() == true)
		{
			result[iter.key()] = iter.value();
			continue;
		}

		if (lhsIter->is_object() == true &&
			iter->is_object() == true)
		{
			result[iter.key()] = Merge(lhsIter.value(), iter.value());
			continue;
		}

		WARNING_MESSAGE("SHOULD NEVER REACH HERE");
	}

	return result;
}

//=========================================================

json
SettingsParser::Merge(
	const std::vector<json>& inputs)
{
	if (inputs.size() == 0)
	{
		return json{};
	}

	if (inputs.size() == 1)
	{
		return inputs.at(0);
	}

	json result{};

	for (const auto& input : inputs)
	{
		result = Merge(result, input);
	}

	return result;
}

//=========================================================

json
SettingsParser::Merge(
	const std::vector<std::string>& inputs)
{
	if (inputs.size() == 0)
	{
		return json{};
	}

	if (inputs.size() == 1)
	{
		return SettingsParser(inputs.at(0)).get();
	}

	json result{};

	for (const auto& input : inputs)
	{
		result = Merge(result, SettingsParser(input).get());
	}

	return result;
}
//=========================================================

