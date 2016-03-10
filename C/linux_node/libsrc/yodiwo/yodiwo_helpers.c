

#include <stdio.h>
#include <stdint.h>
#include <stdbool.h>
#include <string.h>
#include <stdlib.h>
#include "jsmn.h"
#include "yodiwo_api.h"
#include "yodiwo_helpers.h"


/* ======================================================================================================================= */
/* FromJson Functions                                                                                                      */
/* ======================================================================================================================= */

Yodiwo_Plegma_Json_e HelperJsonParseExec(char*json, int jsonSize, ParseTable table[], size_t tableSize)
{
	int i, tokIndex;
	int tokSize;
	jsmntok_t t[64]; /* We expect no more than 128 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((tokSize = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (tokSize < 1 || t[0].type != JSMN_OBJECT) return Yodiwo_JsonFailedObjectExpected;

	/* Loop over all keys of the root object */
	for (tokIndex = 1; tokIndex < tokSize; tokIndex++) {
		jsmntok_t *tok = &t[tokIndex];
//		printf("Type: %d \n", t[tokIndex + 1].type);

		// Scan the match table
		for (i = 0; i < tableSize; i++) {
			// Try to find a match
			int l = (tok->end - tok->start);
			if ((tok->type == JSMN_PRIMITIVE || tok->type == JSMN_STRING) &&
				(l == table[i].filedNameLength) &&
				(memcmp(&json[tok->start], table[i].fieldName, l) == 0))
			{
				if (table[i].parseFunc != NULL)
				{
					tokIndex += table[i].parseFunc(&t[tokIndex + 1], json, table[i].value);
				}
				else if (table[i].parseFuncSubStruct != NULL)
				{
					// We wait for object
					if (!((t[tokIndex + 1].type == JSMN_OBJECT) || (t[tokIndex + 1].type == JSMN_ARRAY)))
						break;

					table[i].parseFuncSubStruct(&json[t[tokIndex + 1].start], t[tokIndex + 1].end - t[tokIndex + 1].start, table[i].value);

					// Skip sub objects
					int blockEnd = t[tokIndex + 1].end;
					int j;
					for (j = tokIndex + 1; j < tokSize && t[j].end <= blockEnd; j++)
						tokIndex++;
				}
				break;
			}
		}
	}
	return Yodiwo_JsonSuccessParse;
}
// -----------------------------------------------------------------------------------------------------------------------
static void *os_zmalloc(size_t size)
{
	void *p = malloc(size);
	if (p != NULL)
		memset(p, 0, size);
	return p;
}
// -----------------------------------------------------------------------------------------------------------------------
int Parse_String(jsmntok_t* t, char* json, void** result)
{
	int s = t->end - t->start;
	//write NULL for empty strings
	if (s == 0) {
		*result = NULL;
		return 1;
	}
	int escaped = count_escaped(&json[t->start], s);
	*result = os_zmalloc(sizeof(char)*(s - escaped + 1));
	if (*result != NULL)
		memcpy_unescaped(*result, &json[t->start], s);
	return 1;
}
// -----------------------------------------------------------------------------------------------------------------------
int Parse_Double(jsmntok_t* t, char* json, void** result)
{
	int s = t->end - t->start;
	char d[100]; memset(d, 0, s);
	memcpy(d, &json[t->start], s);
	*(double *)result = atof(d);
	return 1;
}
// -----------------------------------------------------------------------------------------------------------------------
int Parse_Float(jsmntok_t* t, char* json, void** result)
{
	int s = t->end - t->start;
	char d[100]; memset(d, 0, s);
	memcpy(d, &json[t->start], s);
	*(float *)result = atof(d);
	return 1;
}
// -----------------------------------------------------------------------------------------------------------------------
int Parse_Int(jsmntok_t* t, char* json, void** result)
{
	int s = t->end - t->start;
	char d[100]; memset(d, 0, s);
	memcpy(d, &json[t->start], s);
	*(int32_t *)result = atoi(d);
	return 1;
}
// -----------------------------------------------------------------------------------------------------------------------
int Parse_Bool(jsmntok_t* t, char* json, void** result)
{
	int s = t->end - t->start;
	char d[100]; memset(d, 0, s);
	memcpy(d, &json[t->start], s);
	if (t->type == JSMN_STRING)
	{
		if (memcmp(d, "true", 4) == 0 ||
			memcmp(d, "True", 4) == 0 ||
			memcmp(d, "TRUE", 4) == 0)
			*(bool *)result = true;
		else
			*(bool *)result = false;
	}
	else
	{
		*(bool *)result = atoi(d) > 0;
	}
	return 1;
}
// -----------------------------------------------------------------------------------------------------------------------
// Parse array and find sub structs
// NOTE: this replace the T with the sub elements !!!!
int Helper_Json_ParseArray(jsmntok_t t[], int tokSize)
{
	int i, n = 0, j;
	for (i = 1; i < tokSize; i++)
	{
		// We wait for object
		if (t[i].type != JSMN_OBJECT)
			break;

		// store the element
		t[n] = t[i];
		n++;

		// Skip sub objects
		int blockEnd = t[i].end;
		for (j = i + 1; j < tokSize && t[j].end <= blockEnd; j++)
			i++;
	}
	return n;
}
// -----------------------------------------------------------------------------------------------------------------------
int Helper_Json_ParseArrayType(jsmntok_t t[], int tokSize, jsmntype_t type)
{
	int i, n = 0, j;
	for (i = 1; i < tokSize; i++)
	{
		// We wait for object
		if (t[i].type != type)
			break;

		// store the element
		t[n] = t[i];
		n++;

		// Skip sub objects
		int blockEnd = t[i].end;
		for (j = i + 1; j < tokSize && t[j].end <= blockEnd; j++)
			i++;
	}
	return n;
}
// -----------------------------------------------------------------------------------------------------------------------
// Primitive helperf ToJsonFunction
int Array_string_ToJson(char* jsonStart, size_t jsonSize, Array_string *array)
{
	int i = 0, len; char *json = jsonStart, *jsonEnd = json + jsonSize;
	*json = '['; json++;
	if (array != NULL) {
		for (i = 0; i < array->num && json < jsonEnd; i++) {
			if ((len = snprintf(json, jsonEnd - json - 2, "\"%s\"", array->elems[i])) < 0)
				return -1;
			else json += len;
			*json = ','; json++;
		}
		if (i > 0) json--; // remove last ,
	}
	*json = ']'; json++;
	*json = '\0'; json++;
	return json - jsonStart;
}
// -----------------------------------------------------------------------------------------------------------------------
Yodiwo_Plegma_Json_e Array_string_FromJson(char* json, size_t jsonSize, Array_string *array)
{
	int i = 0, r;
	Yodiwo_Plegma_Json_e  res = Yodiwo_JsonSuccessParse;
	jsmntok_t t[64]; /* We expect no more than 64 tokens */
	jsmn_parser p;
	jsmn_init(&p);
	if ((r = jsmn_parse(&p, json, jsonSize, t, sizeof(t) / sizeof(t[0]))) < 0) return Yodiwo_JsonFailedToParse;
	if (r < 1 || t[0].type != JSMN_ARRAY) return Yodiwo_JsonFailedObjectExpected;

	array->num = Helper_Json_ParseArrayType(t, r, JSMN_STRING);
	array->elems = (char**)os_zmalloc(array->num*sizeof(char*));
	for (i = 0; i < array->num; i++) {
		int size = t[i].end - t[i].start;
		array->elems[i] = (char*)os_zmalloc((size + 1)*sizeof(char)); // add and a zero lead
		if (array->elems[i] == NULL)
			break;

		// copy the string
		memcpy(array->elems[i], &json[t[i].start], size);
	}
	if (i == array->num)
		return Yodiwo_JsonSuccessParse;
	return res;
}
// -----------------------------------------------------------------------------------------------------------------------

int lastIndexOf(char c, char *str)
{
    int r = -1;
    int i = 0;
    for(r = -1, i = 0; str[i] != '\0'; i++) {
        if (str[i] == c)
            r = i;
    }
    return r;
}

// -----------------------------------------------------------------------------------------------------------------------
int NodeKey_FromString(Yodiwo_Plegma_NodeKey_t *nodeKey, char *str)
{
    int len = lastIndexOf(KEY_SEPARATOR, str);
    if (len <= 0)
        return -1;
    nodeKey->UserKey.UserID = (char *)malloc((len + 1) * sizeof(char));
    memcpy(nodeKey->UserKey.UserID, str, len);
    nodeKey->UserKey.UserID[len] = '\0';
    nodeKey->NodeID = atoi(&(str[len + 1]));
    return 0;
}

// -----------------------------------------------------------------------------------------------------------------------
char *make_nextKey_str(char *key, int nextId)
{
    int idlen;
    char id_str[10];
    char *nextKey;
    int r;

    idlen = sprintf(id_str, "%d", nextId);

    if (nextId < 0 || idlen < 0)
        return NULL;
    r = strlen(key) + idlen + 1 + 1; //separator + null terminationportlen;
    nextKey = (char *)malloc(r * sizeof(char) + 2);
    if (nextKey == NULL)
        return NULL;
    r = sprintf(nextKey, "%s-%s", key, id_str);
    if (r < 0) {
        free(nextKey);
        return NULL;
    }
    return nextKey;
}

// -----------------------------------------------------------------------------------------------------------------------
int fill_PortKey(Yodiwo_Plegma_Port_t *port, char *thingKey, int portId)
{
    port->PortKey = make_nextKey_str(thingKey, portId);
    return (port->PortKey != NULL) ? 0 : -1;
}

// -----------------------------------------------------------------------------------------------------------------------
int fill_ThingKey(Yodiwo_Plegma_Thing_t *thing, char *nodeKey, int thingId)
{
    thing->ThingKey = make_nextKey_str(nodeKey, thingId);
    return (thing->ThingKey != NULL) ? 0 : -1;
}

// -----------------------------------------------------------------------------------------------------------------------
int fill_Thing_Keys(Yodiwo_Plegma_Thing_t *thing, char *nodeKey, int thingId)
{
    int i;
    int r;
    r = fill_ThingKey(thing, nodeKey, thingId);
    if (r < 0)
        return -1;
    for (i = 0; i < thing->Ports.num; i++) {
        r = fill_PortKey(&thing->Ports.elems[i], thing->ThingKey, i + 1);
        if (r < 0)
            return -1;
    }
    return 0;
}
