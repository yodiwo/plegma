#include "Plugin.hpp"


//=========================================================

Yodiwo::Plugin::Plugin(const std::string &ThingKeyPrefix) :
	m_thingKeyPrefix(ThingKeyPrefix)
{
	INFO_MESSAGE("Init CPP Plugin with ThingKeyPrefix:[%s]", ThingKeyPrefix.c_str());

	// Create the list with things
	echoThing = std::make_shared<Yodiwo::Plegma::Thing>();

	echoThing->ThingKey = CreateThingKey("Echo");
	echoThing->Name = "Ping";
	echoThing->Type = "";
	echoThing->IconURI = "https://apk-dl.com/detail/image/com.lipinic.ping-w250.png";
	echoThing->OnPortEvent = [&](const Yodiwo::Plegma::Thing& thing, const std::vector<Yodiwo::Plegma::PortEventExt>& events) {
		INFO_MESSAGE("Get port events [%zd] for thing[%s]", events.size(), thing.Name.c_str());
	};


	// Create port out
	std::shared_ptr<Yodiwo::Plegma::Port> echoPortOut = std::make_shared<Yodiwo::Plegma::Port>();
	echoPortOut->PortKey = "echoPortOut";
	echoPortOut->Name = "Echo";
	echoPortOut->State = "";
	echoPortOut->Type = Yodiwo::Plegma::ePortType::String;
	echoPortOut->ioDirection = Yodiwo::Plegma::ioPortDirection::Output;
	echoThing->Ports.push_back(echoPortOut);


	// Create port in
	std::shared_ptr<Yodiwo::Plegma::Port> echoPortIn = std::make_shared<Yodiwo::Plegma::Port>();
	echoPortIn->PortKey = "echoPortIn";
	echoPortIn->Name = "Echo";
	echoPortIn->State = "";
	echoPortIn->Type = Yodiwo::Plegma::ePortType::String;
	echoPortIn->ioDirection = Yodiwo::Plegma::ioPortDirection::Input;
	echoPortIn->OnPortEvent = [&](const Yodiwo::Plegma::Port &port, const Yodiwo::Plegma::PortEventExt &state) { EchoPortIn(port, state); };
	echoThing->Ports.push_back(echoPortIn);

	// Add the thing to the list
	m_things.push_back(echoThing);

	// Update m_ports mapping
	UpdateThingMaps();
}

//=========================================================

void Yodiwo::Plugin::OnTransportConnected(void) {
	INFO_MESSAGE("OnTransportConnected");
}

void Yodiwo::Plugin::OnTransportDisconnected(void) {
	INFO_MESSAGE("OnTransportDisconnected");
}

//=========================================================

void Yodiwo::Plugin::EchoPortIn(const Yodiwo::Plegma::Port &port,
	const Yodiwo::Plegma::PortEventExt &state)
{
	INFO_MESSAGE("EchoPortIn: %s", state.State.c_str());

	// Send the message back :) 
	if (echoThing != nullptr)
		SendPortEvent(
			echoThing->ThingKey,
			"echoPortOut",
			state.State
		);
}

//=========================================================
