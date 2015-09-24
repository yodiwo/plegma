
#ifndef _YODIWO_API_HELPERS_H_
#define _YODIWO_API_HELPERS_H_

#include "yodiwo_api.h"

#define YODIWO_API_VERSION_STR "1"
#define KEY_SEPARATOR '-'

/* ======================================================================================================================= */
/* FromJson Functions                                                                                                      */
/* ======================================================================================================================= */
typedef int(ParseFunc)(jsmntok_t* tok, char* json, void** result);
typedef Yodiwo_Plegma_Json_e(ParseFuncSubStruct)(char *json, size_t jsonSize, void* value);
// -----------------------------------------------------------------------------------------------------------------------
typedef struct {
	char*		fieldName;
	int			filedNameLength;
	ParseFunc*	parseFunc;
	ParseFuncSubStruct* parseFuncSubStruct;
	void**		value;
} ParseTable;
// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e HelperJsonParseExec(char*json, int jsonSize, ParseTable table[], size_t tableSize);

int Parse_String(jsmntok_t* t, char* json, void** result);
int Parse_Double(jsmntok_t* t, char* json, void** result);
int Parse_Float(jsmntok_t* t, char* json, void** result);
int Parse_Int(jsmntok_t* t, char* json, void** result);
int Parse_Bool(jsmntok_t* t, char* json, void** result);
int Helper_Json_ParseArray(jsmntok_t t[], int tokSize);



// -----------------------------------------------------------------------------------------------------------------------
int Array_string_ToJson(char* jsonStart, size_t jsonSize, Array_string *array);
Yodiwo_Plegma_Json_e Array_string_FromJson(char* json, size_t jsonSize, Array_string *array);

int NodeKey_FromString(Yodiwo_Plegma_NodeKey_t *nodeKey, char *str);

char *make_nextKey_str(char *key, int nextId);
int fill_PortKey(Yodiwo_Plegma_Port_t *port, char *thingKey, int portId);
int fill_ThingKey(Yodiwo_Plegma_Thing_t *thing, char *nodeKey, int thingId);
int fill_Thing_Keys(Yodiwo_Plegma_Thing_t *thing, char *nodeKey, int thingId);


#endif /* _Yodiwo_Plegma_HELPERS_H_ */
