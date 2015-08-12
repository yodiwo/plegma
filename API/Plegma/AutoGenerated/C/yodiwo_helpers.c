

#include <stdio.h>
#include <stdint.h>
#include <stdbool.h>
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
		printf("Type: %d \n", t[tokIndex + 1].type);

		// Scan the match table
		for (i = 0; i < tableSize; i++) {
			// Try to find a match
			int l = (tok->end - tok->start);
			if ((tok->type == JSMN_PRIMITIVE) &&
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
	*result = os_zmalloc(sizeof(char)*(s + 1));
	if (*result != NULL)
		memcpy(*result, &json[t->start], s);
	return 1;
}
// -----------------------------------------------------------------------------------------------------------------------
int Parse_Double(jsmntok_t* t, char* json, void** result)
{
	int s = t->end - t->start;
	char d[100]; memset(d, 0, s);
	memcpy(d, &json[t->start], s);
	*result = atof(d);
	return 1;
}
// -----------------------------------------------------------------------------------------------------------------------
int Parse_Float(jsmntok_t* t, char* json, void** result)
{
	int s = t->end - t->start;
	char d[100]; memset(d, 0, s);
	memcpy(d, &json[t->start], s);
	*result = atof(d);
	return 1;
}
// -----------------------------------------------------------------------------------------------------------------------
int Parse_Int(jsmntok_t* t, char* json, void** result)
{
	int s = t->end - t->start;
	char d[100]; memset(d, 0, s);
	memcpy(d, &json[t->start], s);
	*result = atoi(d);
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
			*result = true;
		else
			*result = false;
	}
	else
	{
		*result = atoi(d) > 0;
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
