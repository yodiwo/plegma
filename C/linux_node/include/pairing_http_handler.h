
#ifndef __PAIRING_HTTP_HANDLER_H__
#define __PAIRING_HTTP_HANDLER_H__

#ifdef __cplusplus
extern "C" {
#endif

#include "pairing_backend.h"

int start_pairing_http_server(void *ifdata, int port, onPaired_callback onPaired);

void pairing_handler(int id);


#ifdef __cplusplus
}
#endif

#endif /* __PAIRING_HTTP_HANDLER_H__ */
