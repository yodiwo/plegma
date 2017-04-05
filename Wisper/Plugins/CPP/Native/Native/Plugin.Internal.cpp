#include "Plugin.hpp"


//=========================================================


void Yodiwo::Plugin::UpdateThingMaps() {
	m_map_ports.clear();
	m_map_things.clear();
	for (const auto &t : m_things)
	{
		m_map_things[t->ThingKey] = t;
		for (const auto &p : t->Ports) {
			m_map_ports[t->ThingKey][p->PortKey] = p;
		}
	}
}

//=========================================================

void Yodiwo::Plugin::OnPortEvent(const std::vector<Yodiwo::Plegma::PortEventExt> &portsEvents) {
	INFO_MESSAGE("Recv Events [%zd]", portsEvents.size());

	try {
		// Seperate the events based on thing uuid
		std::map<std::string, std::vector<Yodiwo::Plegma::PortEventExt>> mapping;
		for (auto &p : portsEvents)
		{
			auto m = mapping.find(p.ThingUID);
			// If is not exist create a new entry for thing uid 
			if (m == mapping.end()) {
				mapping[p.ThingUID] = std::vector<Yodiwo::Plegma::PortEventExt>();
			}
			mapping[p.ThingUID].emplace_back(p);


			// Handle the port events
			INFO_MESSAGE("Get message for TUID->[%s] PUID->[%s] State->%s", p.ThingUID.c_str(), p.PortUID.c_str(), p.State.c_str());


			auto p_map = m_map_ports.find(p.ThingUID);
			if (p_map != m_map_ports.end()) {
				// Call the port event handler
				auto port = p_map->second.find(p.PortUID);
				if (port != p_map->second.end()) {
					if (port->second->OnPortEvent) {
						try {
							port->second->OnPortEvent(*port->second, p);
						}
						catch (const std::exception& exp) {
							ERROR_MESSAGE("Failed in OnPortEvent of the port %s. Exception: %s",
								port->second->Name.c_str(),
								exp.what());
						}
					}

					// Update the internal state
					port->second->State = p.State;
				}
			}
		}

		// Send event to thing
		for (auto &t : mapping) {
			// Call the thing event handler 
			auto thing = m_map_things.find(t.first);
			if (thing != m_map_things.end() &&
				thing->second->OnPortEvent) {
				try {
					thing->second->OnPortEvent(*thing->second, t.second);
				}
				catch (const std::exception& exp) {
					ERROR_MESSAGE("Failed in OnPortEvent of the thing %s. Exception: %s",
						thing->second->Name.c_str(),
						exp.what());
				}
			}
		}
	}
	catch (const std::exception& exp) {
		ERROR_MESSAGE("Failed to process OnPortEvent. Exception: %s", exp.what());
	}
}


//=========================================================

bool Yodiwo::Plugin::SendPortEvent(
	const std::string & ThingKeyUID,
	const std::string & PortKeyUID,
	const std::string & state)
{
	try {
		// Create the event
		Yodiwo::Plegma::SendPortEvent portEvent;
		portEvent.PortKey = PortKeyUID;
		portEvent.ThingKey = ThingKeyUID;
		portEvent.State = state;

		// Send the message
		return this->SendEvent(Yodiwo::Plegma::MsgType::SendPortEvent, portEvent);
	}
	catch (const std::exception& exp) { ERROR_MESSAGE("Failed to send message. Exception: %s", exp.what()); }
	return false;
}

bool Yodiwo::Plugin::SendEvent(const Yodiwo::Plegma::MsgType::Enum type,
	const JsonSeriazable &message)
{
	try {
		// Create the message
		Yodiwo::Plegma::CSharpMsg msg;
		msg.Type = type;
		msg.Message = message.toJson().dump();

		INFO_MESSAGE("Send event of type [%d: %s] with state: %s",
			(int)type,
			Yodiwo::Plegma::MsgType::ToString(type),
			msg.Message.c_str());

		// Send the message
		m_msgQueue.SendMessage(msg.toJson().dump());
		return true;
	}
	catch (const std::exception& exp) { ERROR_MESSAGE("Failed to send message. Exception: %s", exp.what()); }
	return false;
}

//=========================================================