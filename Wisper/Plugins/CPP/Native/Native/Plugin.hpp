#pragma once
#include "MsgQueue.hpp"
#include "Plegma.hpp"
#include "Utilities/Utilities.hpp"

namespace Yodiwo
{

	class Plugin
	{
	private:
		Yodiwo::MsgQueue m_msgQueue;
		std::vector<std::shared_ptr<Yodiwo::Plegma::Thing>> m_things;
		std::string m_thingKeyPrefix;

		// Thing mapping
		std::map<std::string, std::map<std::string, std::shared_ptr<Yodiwo::Plegma::Port>>> m_map_ports;
		std::map<std::string, std::shared_ptr<Yodiwo::Plegma::Thing>> m_map_things;

	public:
		Plugin(const std::string &ThingKeyPrefix);
		~Plugin() { StopQueue(); };

		//=========================================================
		// Internal functions
		void StopQueue() { m_msgQueue.Stop(); }
		std::string GetMsg() { return m_msgQueue.GetMessage(); }
		std::vector<std::shared_ptr<Yodiwo::Plegma::Thing>> GetThings() { return m_things; }
		// Create unique thing key with the correct form
		std::string CreateThingKey(std::string subThingKey) { return m_thingKeyPrefix + "CPPExample_" + subThingKey; }

		void OnPortEvent(const std::vector<Yodiwo::Plegma::PortEventExt> &Ports);
		void UpdateThingMaps();
		bool SendPortEvent(const std::string &ThingKeyUID, const std::string &PortKeyUID, const std::string &state);
		bool SendEvent(const Yodiwo::Plegma::MsgType::Enum type, const JsonSeriazable &message);
		//=========================================================





		//=========================================================
		// Managment functions
		void OnTransportConnected(void);
		void OnTransportDisconnected(void);


	private:
		//=========================================================
		// We can save the things struct for easy access
		std::shared_ptr<Yodiwo::Plegma::Thing> echoThing;


		//=========================================================
		// Per application function
		void EchoPortIn(const Yodiwo::Plegma::Port &port, const Yodiwo::Plegma::PortEventExt &state);

	};
}