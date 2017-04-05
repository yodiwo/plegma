#pragma once
#include <vector>
#include "Utilities/Utilities.hpp"
#include "Utilities/JsonSeriazable.hpp"

namespace Yodiwo
{
	namespace Plegma
	{
		// --------------------------------------------
		DEFINE_ENUM_TYPE(
			ioPortDirection,
			int,
			((Undefined, 0))
			((InputOutput, 1))
			((Output, 2))
			((Input, 3)));
		// --------------------------------------------
		DEFINE_ENUM_TYPE(
			ePortType,
			int,
			((Undefined, 0))
			((Integer, 1))
			((Decimal, 2))
			((DecimalHigh, 3))
			((Boolean, 4))
			((Color, 5))
			((String, 6))
			((VideoDescriptor, 7))
			((AudioDescriptor, 8))
			((BinaryResourceDescriptor, 9))
			((I2CDescriptor, 10))
			((JsonString, 11))
			((IncidentDescriptor, 12))
			((Timestamp, 13)));

		// --------------------------------------------
		DEFINE_ENUM_TYPE(
			MsgType,
			int,
			((Undefined, 0))
			((SendPortEvent, 1))
		);
		// ------------------------------------------------------------------------
		class PortEventExt
			: public Yodiwo::JsonSeriazable
		{
			// Variables
		public:
			std::string ThingUID;	/// Splitted Thing Key
			std::string PortUID;	/// Splitted Port Key
			std::string PortKey;	/// <see cref="PortKey"/> of the <see cref="Port"/> this message refers to (either generating the event, or receiving the event)
			std::string State;		/// Contents of the event in string form. See <see cref="Port.State"/>
			uint RevNum;			/// Revision number of this update; matches the Port State's internal sequence numbering. See <see cref="Port.State"/>
			ulong Timestamp;		/// Timestamp (in msec since Unix Epoch) of event creation


			//=========================================================
			// Functions
		public:
			PortEventExt() {}
			~PortEventExt() {}

			// Parse json
			PortEventExt(const json& input) {
				try {
					Yodiwo::Utilities::SettingsParser parser(input);

					// Parse json
					parser.GetIfExists("ThingUID", ThingUID);
					parser.GetIfExists("PortUID", PortUID);
					parser.GetIfExists("PortKey", PortKey);
					parser.GetIfExists("State", State);
					parser.GetIfExists("RevNum", RevNum);
					parser.GetIfExists("Timestamp", Timestamp);

				}
				catch (const std::exception& exp)
				{
					ERROR_MESSAGE("Feild to parse Tking json %s", exp.what());
				}
			}

			//=========================================================
			// Json
			json toJson() const
			{
				json j{
					{ "ThingUID", ThingUID },
					{ "PortUID", PortUID },
					{ "PortKey", PortKey },
					{ "State", State },
					{ "RevNum", RevNum },
					{ "Timestamp", Timestamp }
				};
				return j;
			}

			//=========================================================
		};
		// ------------------------------------------------------------------------
		class Port
			: public Yodiwo::JsonSeriazable
		{

			//=========================================================
			// Fields
		public:
			/// <summary>Globally unique string identifying this port; Construct it using the <see cref="PortKey"/> constructor</summary>
			std::string PortKey;

			/// <summary>Friendly name of this Port (as it will appear in the Cyan UI and blocks)</summary>
			std::string Name;

			/// <summary>Description of Port to show in Cyan (tooltip, etc)</summary>
			std::string Description;

			/// <summary>Direction (<see cref="ioPortDirection"/>) of Port</summary>
			ioPortDirection::Enum ioDirection;

			/// <summary>type (<see cref="ePortType"/>) of values that each Port sends / receives</summary>
			ePortType::Enum Type;

			/// <summary cref="Port.State">Current (at latest update/sampling/trigger/etc) value of Port as String.
			/// Contains a string representation of the port's state, encoded according to the port's <see cref="ePortType"/>
			/// On receiving events the Cloud Server will attempt to parse the State based on its <see cref="ePortType"/>
			/// When sending events the Cloud Server will encode the new state into a string, again according to the Port's <see cref="ePortType"/>
			/// </summary>
			std::string State;

			//=========================================================
			// Internal pointer functions for handling events

			std::function<void(const Port&, const PortEventExt&)> OnPortEvent;


			//=========================================================
			// Functions
		public:
			Port() {}
			~Port() {}

			// Parse recv json
			Port(const json& input) {
				try {
					Yodiwo::Utilities::SettingsParser parser(input);

					// Parse json
					PortKey = parser.value<std::string>("PortKey");
					parser.GetIfExists("Name", Name);
					parser.GetIfExists("Description", Description);
					parser.GetIfExists("State", State);
					parser.GetIfExistsCast<int>("ioDirection", ioDirection);
					parser.GetIfExistsCast<int>("Type", Type);
				}
				catch (const std::exception& exp)
				{
					ERROR_MESSAGE("Failed to parse port json %s", exp.what());
				}
			}

			//=========================================================
			// Json
			json toJson() const
			{
				json j{
					{ "PortKey", PortKey },
					{ "Name", Name },
					{ "Description", Description },
					{ "ioDirection", (int)ioDirection },
					{ "Type", (int)Type },
					{ "State", State }
				};
				return j;
			}

			//=========================================================
		};
		// ------------------------------------------------------------------------
		class Thing
			: public Yodiwo::JsonSeriazable
		{
			// Fields
		public:
			/// <summary>
			/// Globally unique Key string of this Thing
			/// </summary>
			std::string ThingKey;

			/// <summary>
			/// friendly name of this Thing
			/// </summary>
			std::string Name;

			/// <summary>
			/// list of vendor provided configuration parameters (changeable by the user)
			/// </summary>
			std::string Type;

			/// <summary>
			/// list of ports (inputs / outputs) that this Thing implements
			/// </summary>
			std::vector<std::shared_ptr<Port>> Ports;

			/// <summary>
			/// Specifies the Thing's hierarchy within the node's modeled ecosystem.
			/// Specifies a hierarchical view (separated by '/') of the Thing's position in the User's ecosystem of devices. Must start with '/'
			/// May be left null or empty.
			/// </summary>
			std::string Hierarchy;

			// UI
			/// <summary>URI of icon to show in Cyan for this thing</summary>
			std::string IconURI;

			/// <summary>Description of Thing to show in Cyan (tooltip, etc)</summary>
			std::string Description;


			//=========================================================
			// Internal pointer functions for handling events

			std::function<void(const Thing&, const std::vector<PortEventExt>&)> OnPortEvent;

			//=========================================================
			// Functions
		public:
			Thing() {}
			~Thing() {}

			// Parse recv json
			Thing(const json& input) {
				try {
					Yodiwo::Utilities::SettingsParser parser(input);

					// Parse json
					parser.GetIfExists("ThingKey", ThingKey);
					parser.GetIfExists("Name", Name);
					parser.GetIfExists("Type", Type);
					parser.GetIfExists("Hierarchy", Hierarchy);

					// Parse UIHints
					if (parser.contains("UIHints")) {
						Yodiwo::Utilities::SettingsParser uiHints(parser.get("UIHints"));
						uiHints.GetIfExists("IconURI", IconURI);
						uiHints.GetIfExists("Description", Description);
					}


					// Parse ports
					Ports = Yodiwo::Utilities::SettingsParser::ToArraySharePtr<Port>(parser.get("Ports"));

				}
				catch (const std::exception& exp)
				{
					ERROR_MESSAGE("Failed to parse Tking json %s", exp.what());
				}
			}

			//=========================================================
			// Json
			json toJson() const
			{
				json j{
					{ "ThingKey", ThingKey },
					{ "Name", Name },
					{ "Type", Type }
				};

				j["UIHints"] = {
					{"IconURI", IconURI },
					{"Description", Description}
				};


				json portsJson;
				for (auto &p : Ports) {
					portsJson.push_back(p->toJson());
				}
				j["Ports"] = portsJson;

				return j;
			}

			//=========================================================
		};
		// ------------------------------------------------------------------------

		class CSharpMsg
			: public Yodiwo::JsonSeriazable
		{
			// Fields
		public:
			MsgType::Enum Type;

			/// Json form of the actual message
			std::string Message;

			//=========================================================
			// Functions
		public:
			CSharpMsg() {}
			~CSharpMsg() {}

			// Parse recv json
			CSharpMsg(const json& input) {
				try {
					Yodiwo::Utilities::SettingsParser parser(input);

					// Parse json
					parser.GetIfExistsCast<int>("Type", Type);
					parser.GetIfExists("Message", Message);
				}
				catch (const std::exception& exp)
				{
					ERROR_MESSAGE("Failed to parse Tking json %s", exp.what());
				}
			}

			//=========================================================
			// Json
			json toJson() const
			{
				json j{
					{ "Type", (int)Type },
					{ "Message", Message }
				};
				return j;
			}

			//=========================================================

		};
		// ------------------------------------------------------------------------

		class SendPortEvent
			: public Yodiwo::JsonSeriazable
		{
			// Fields
		public:
			/// <summary>
			/// Globally unique Key string of this Thing
			/// </summary>
			std::string ThingKey;

			/// <summary>Globally unique string identifying this port; Construct it using the <see cref="PortKey"/> constructor</summary>
			std::string PortKey;

			/// <summary>
			/// New State
			/// </summary>
			std::string State;

			//=========================================================
			// Functions
		public:
			SendPortEvent() {}
			~SendPortEvent() {}

			// Parse recv json
			SendPortEvent(const json& input) {
				try {
					Yodiwo::Utilities::SettingsParser parser(input);

					// Parse json
					parser.GetIfExists("ThingKey", ThingKey);
					parser.GetIfExists("PortKey", PortKey);
					parser.GetIfExists("State", State);
				}
				catch (const std::exception& exp)
				{
					ERROR_MESSAGE("Failed to parse Tking json %s", exp.what());
				}
			}

			//=========================================================
			// Json
			json toJson() const
			{
				json j{
					{ "ThingKey", ThingKey },
					{ "PortKey", PortKey },
					{ "State", State }
				};
				return j;
			}

			//=========================================================

		};

		// ------------------------------------------------------------------------
	}
}