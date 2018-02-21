#include <vector>
#include "platform/Utilities.hpp"
#include "platform/JsonSeriazable.hpp"
#include "Defs.h"
#include "value_classes/ValueID.h"

namespace Yodiwo
{
	namespace Plegma
	{
		class ValueIDmsg : public Yodiwo::JsonSeriazable
		{
		public:
			OpenZWave::ValueID::ValueGenre genre;
			uint8 commandClassId;
			OpenZWave::ValueID::ValueType  type;
			uint8 nodeID;
			uint8 groupId;
			uint8  instance;
			uint8  valueIndex;
			uint32 homeID;
			std::string label;

		public:
			ValueIDmsg() {}
			~ValueIDmsg() {}

			// Parse recv json
			ValueIDmsg(const json& input) {
				try {
					Yodiwo::Utilities::SettingsParser parser(input);

					// Parse json		
					parser.GetIfExists("homeID", homeID);
					parser.GetIfExists("nodeID", nodeID);
					parser.GetIfExists("groupId", groupId);
					parser.GetIfExists("genre", genre);
					parser.GetIfExists("commandClassId", commandClassId);
					parser.GetIfExists("instance", instance);
					parser.GetIfExists("valueIndex", valueIndex);
					parser.GetIfExists("type", type);
					parser.GetIfExists("label", label);
				}
				catch (const std::exception& exp) { exp.what(); }
			}

			json toJson() const
			{
				json j{
					{ "homeID", homeID },
					{ "nodeID", nodeID },
					{ "groupId", groupId },
					{ "genre", genre },
					{ "commandClassId", commandClassId },
					{ "instance", instance },
					{ "type", type },
					{ "valueIndex", valueIndex },
					{ "label", label },
				};
				return j;
			}
		};

		class CSharpMsg : public ValueIDmsg
		{
			// Fields
		public:
			enum NotificationCode
			{
				Code_MsgComplete = 0,			/**< Completed messages */
				Code_Timeout,					/**< Messages that timeout will send a Notification with this code. */
				Code_NoOperation,				/**< Report on NoOperation message sent completion  */
				Code_Awake,						/**< Report when a sleeping node wakes up */
				Code_Sleep,						/**< Report when a node goes to sleep */
				Code_Dead,						/**< Report when a node is presumed dead */
				Code_Alive						/**< Report when a node is revived */
			};

			OpenZWave::Notification::NotificationType NotificationType;
			uint8 NotificationCode;
			std::string name;
			std::string currentValue;

		private:

			// Functions
		public:
			CSharpMsg() {}
			~CSharpMsg() {}

			// Parse recv json
			CSharpMsg(const json& input) {
				try {
					Yodiwo::Utilities::SettingsParser parser(input);

					// Parse json
					parser.GetIfExists("NotificationType", NotificationType);
					parser.GetIfExists("NotificationCode", NotificationCode);
					parser.GetIfExists("homeID", homeID);
					parser.GetIfExists("nodeID", nodeID);
					parser.GetIfExists("groupId", groupId);
					parser.GetIfExists("genre", genre);
					parser.GetIfExists("commandClassId", commandClassId);
					parser.GetIfExists("instance", instance);
					parser.GetIfExists("valueIndex", valueIndex);
					parser.GetIfExists("type", type);
					parser.GetIfExists("label", label);
					parser.GetIfExists("name", name);
					parser.GetIfExists("currentValue", currentValue);
				}
				catch (const std::exception& exp) { exp.what(); }
			}

			json toJson() const
			{
				json j{
					{ "NotificationType", NotificationType },
					{ "NotificationCode", NotificationCode },
					{ "homeID", homeID },
					{ "nodeID", nodeID },
					{ "groupId", groupId },
					{ "genre", genre },
					{ "commandClassId", commandClassId },
					{ "instance", instance },
					{ "type", type },
					{ "label", label },
					{ "valueIndex", valueIndex },
					{ "name", name },
					{ "currentValue", currentValue },
				};
				return j;
			}
		};
	};
};