
#ifndef _YODIWO_API_HELPERS_H_
#define _YODIWO_API_HELPERS_H_




/* ======================================================================================================================= */
/* FromJson Functions                                                                                                      */
/* ======================================================================================================================= */
typedef int(ParseFunc)(jsmntok_t* tok, char* json, void** result);
typedef int(ParseFuncSubStruct)(char *json, size_t jsonSize, void* value);
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



#endif /* _Yodiwo_Plegma_HELPERS_H_ */