#include <queue>
#include <mutex>
#include <string>
#include <thread>
#include <stddef.h>
#include <condition_variable>

#include "Plugin.hpp"
#include "Utilities/Utilities.hpp"

// ================================================================================================
// Helper functions
char *CopyForCSharp(const std::string &data) {
	int size = strlen(data.c_str()) + 1;

	//INFO_MESSAGE("Try to send [%d]:\"%s\"", size, data.c_str());
	if (size > 0) {
		// Dynamic allocation, the C# is going to free this memory
		char* newData = new char[size];

		// Copy the data
		memcpy(newData, data.c_str(), size);
		newData[size - 1] = '\0'; // null terminate just for safety

		return newData;
	}
	return NULL;
}


// ================================================================================================
// Declaration for .so 
extern "C" {
	// ================================================================================================
	// Variables
	std::shared_ptr<Yodiwo::Plugin> Plugin = nullptr;

	// ================================================================================================
	// Init

	int Initialize(char* ThingKeyPrefix)
	{
		INFO_MESSAGE("Initialize");

		// Create a memory space for plugin
		Plugin = std::make_shared<Yodiwo::Plugin>(std::string(ThingKeyPrefix));

		return 0;
	}

	int Deinitialize()
	{
		INFO_MESSAGE("Deinitialize");

		// Unblock get message
		Plugin->StopQueue();

		// Release the pointer
		Plugin.reset();


		INFO_MESSAGE("Deinitialize Complete");

		// Clean debug trace 
		Yodiwo::Utilities::TraceSystem::StopTrace();
		return 0;
	}


	// ================================================================================================


	//consumer: used by C# to retrieve commands/status
	const char* GetMessage(void)
	{
		if (Plugin != nullptr) {
			auto msg = Plugin->GetMsg();
			if (!msg.empty())
				return CopyForCSharp(msg);
		}
		return NULL;
	}

	// ================================================================================================
	// Connect with static functions
	void OnTransportConnected(void) { if (Plugin == nullptr) Plugin->OnTransportConnected(); }
	void OnTransportDisconnected(void) { if (Plugin == nullptr) Plugin->OnTransportDisconnected(); }
	// -----------------------------------------------------------------------------------------------
	const char* GetThings(void) {
		if (Plugin != nullptr)
		{
			try {
				Yodiwo::json things;
				for (auto &t : Plugin->GetThings()) {
					things.push_back(t->toJson());
				}
				INFO_MESSAGE("Get things [#%zd]", things.size());
				return CopyForCSharp(things.dump());
			}
			catch (const std::exception& exp) { ERROR_MESSAGE("Failed to get things. Exception: %s", exp.what()); }
		}
		return NULL;
	}
	// -----------------------------------------------------------------------------------------------
	void OnPortEvent(const char* msg)
	{
		if (Plugin != nullptr) {
			try {
				Yodiwo::json json = Yodiwo::json::parse(msg);

				// Parse ports
				std::vector<Yodiwo::Plegma::PortEventExt> Ports = Yodiwo::Utilities::SettingsParser::ToArray<Yodiwo::Plegma::PortEventExt>(json);

				// Send the event to Plugin
				Plugin->OnPortEvent(Ports);
			}
			catch (const std::exception& exp) { ERROR_MESSAGE("Failed to get things. Exception: %s", exp.what()); }
		}
	}
	// -----------------------------------------------------------------------------------------------
}
