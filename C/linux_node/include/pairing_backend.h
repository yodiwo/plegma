
#ifndef __PAIRING_BACKEND_H__
#define __PAIRING_BACKEND_H__

#ifdef __cplusplus
extern "C" {
#endif

#include <stdint.h>
#include "jsmn.h"
#include "yodiwo_helpers.h"

typedef void (*onPaired_callback)(char *nodeKey, char *secretKey);

typedef int (*function_with_result)(void *arg);

typedef struct
{
    char *postUrl;
    char *uuid;
    char *name;
    char *token1;
    char *token2;
    char *nodeKey;
    char *secretKey;
    char *next_url;
    onPaired_callback onPaired;
} pairing_context;

typedef struct
{
    char *token1;
    char *token2; 
} tokens_t;

int pairing_context_init_with_defaults(pairing_context *ctx, onPaired_callback callback);
int pairing_context_init_from_config(pairing_context *ctx, onPaired_callback callback);

int pairing_get_tokens(pairing_context *ctx);
int pairing_get_keys(pairing_context *ctx);

char* get_next_url(char *hostname, int port, char *urlBase);
char* get_server_phase2_url(pairing_context *ctx);

#ifdef __cplusplus
}
#endif

#endif /* __PAIRING_BACKEND_H__ */
