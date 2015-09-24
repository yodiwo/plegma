#include <stdint.h>
#include <stdbool.h>
#include <stdio.h>
#include <string.h>
#include "jsmn.h"
#include "yodiwo_helpers.h"
#include "config.h"
#include "system.h"
#include "pairing_backend.h"

char recvBuff[1024*20];
int __get_tokens(pairing_context *ctx);
int __get_keys(pairing_context *ctx);

int do_on_thread(function_with_result func, void *args, unsigned int stack_size);
void *__thread_wrapper(void *arg);

#define PAGE_SERVER_PHASE2 "userconfirm"

int pairing_context_init_with_defaults(pairing_context *ctx, onPaired_callback callback)
{
    ctx->postUrl = "http://10.30.254.199:3334/pairing/";
    ctx->uuid = "1337CNode";
    ctx->name = "Nodas";
    ctx->onPaired = callback;
    return 0;
}

extern Yodiwo_Tools_APIGenerator_CNodeConfig_t *activeConfig;

int pairing_context_init_from_config(pairing_context *ctx, onPaired_callback callback)
{
    printf("server url: %s\n", activeConfig->PairingServerUrl);
    printf("uuid: %s\n", activeConfig->Uuid);
    printf("name: %s\n", activeConfig->Name);
    ctx->postUrl = activeConfig->PairingServerUrl;
    ctx->uuid = activeConfig->Uuid;
    ctx->name = activeConfig->Name;
    ctx->onPaired = callback;
    return 0;    
}

#define STACK_SIZE 24000


int pairing_get_tokens(pairing_context *ctx)
{
    printf("getting tokens from server\n");
    return do_on_thread((function_with_result)__get_tokens, ctx, STACK_SIZE);
}

char* get_server_phase2_url(pairing_context *ctx, char *hostname, int port, char *urlBase)
{
        int len = 200; //TODO: proper calculation
        char* result = (char *)malloc(sizeof(char) * len);
        if (!result)
            return NULL;
        sprintf(result, "%s" "1/" PAGE_SERVER_PHASE2 "?token2=%s&noderedirect=http://%s:%d%snext",
        		ctx->postUrl, ctx->token2, hostname, (port == 0) ? 80 : port, urlBase);
        return result;        
}

int pairing_get_keys(pairing_context *ctx)
{
    printf("getting keys from server\n");
    return do_on_thread((function_with_result)__get_keys, ctx, STACK_SIZE);
}


struct thread_info
{
    function_with_result func;
    void * args;
    int result;
};

void *__thread_wrapper(void *arg)
{
    struct thread_info *info = (struct thread_info *)arg;
    info->result = info->func(info->args);
    return NULL;
}

int do_on_thread(function_with_result func, void *args, unsigned int stack_size)
{
	int r;
    struct thread_info info;
    info.func = func;
    info.args = args;
    thread_t t;
    r = thread_run(&t, __thread_wrapper, &info, 0, stack_size);
    printf("run: %d\n", r);
    if (r < 0)
    	return -1;
    thread_join(&t);

    return info.result;    
}
    
int __get_tokens(pairing_context *ctx)
{
	char url[100];
    char postfields[100];
    char response[512];
    int ret;

    strcpy(url, ctx->postUrl);
    strcat(url, "/1/gettokensreq");

    //POST data
    sprintf(postfields, "name=%s&uuid=%s", ctx->name, ctx->uuid);
    printf("\nTrying to post data...\n");
    ret = http_post(url, postfields, response, 512);
    Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_t tokens;
    if (ret > 0)
    {
        printf("Executed POST successfully - read %zu characters\n", strlen(response));
        printf("Result: %s\n", response);
        int jret = Yodiwo_Plegma_NodePairing_PairingServerTokensResponse_FromJson(response, strlen(response), &tokens);
        if (jret == Yodiwo_JsonSuccessParse) {
            ctx->token1 = tokens.token1;
            ctx->token2 = tokens.token2;
            return 0;
        } else {
            printf("error parsing response");
            return -2;
        }
    }
    else
    {
      printf("Error HTTP Post return code = %d\n", ret);
    }
    return ret;
    
}


int __get_keys(pairing_context *ctx)
{
	char url[100];
    char postfields[100];
    char response[512];

    strcpy(url, ctx->postUrl);
    strcat(url, "/1/getkeysreq");
    int ret;

    //POST data
    sprintf(postfields, "uuid=%s&token1=%s", ctx->uuid, ctx->token1);

    printf("\nTrying to post data...\n");
    ret = http_post(url, postfields, response, 512);
    Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_t keys;
    if (ret > 0)
    {
        printf("Executed POST successfully - read %zu characters\n", strlen(response));
        printf("Result: %s\n", response);
        int jret = Yodiwo_Plegma_NodePairing_PairingServerKeysResponse_FromJson(response, strlen(response), &keys);
        if (jret == Yodiwo_JsonSuccessParse) {
            ctx->nodeKey = keys.nodeKey;
            ctx->secretKey = keys.secretKey;
            return 0;
        } else {
            printf("error parsing response");
            return -2;
        }
    }
    else
    {
        printf("Error HTTP Post return code = %d\n", ret);
    }
    return ret;
    
}
