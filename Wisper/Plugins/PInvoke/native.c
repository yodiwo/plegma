/*
Native source code

compiled as shared library using :
gcc -fPIC -shared native.c -o native.so

*/

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <pthread.h>
#include <unistd.h>

//Variables
int isRunning;
char* command_queue[1]; //replace with real queue mechanism
pthread_t sendhello_thread;
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

//simple test function called by managed host 
int calculate(int a, int b)
{
    return a + b;
}

//consumer: used by host to retrieve commands
//this is a simple spinning mechanism to check for new commands.
//you should implement a more efficient mechanism (eg using semaphores and mutex conditions) to unlock the sleeping thread to dequeue command.
char* dequeue_Command(void)
{
    char* cmd = NULL;
    while (isRunning)
    {
        //lock to check queue
        pthread_mutex_lock(&mutex);
        {
            cmd = command_queue[0];
            command_queue[0] = NULL;
        }
        pthread_mutex_unlock(&mutex);

        //check
        if (cmd == NULL)
            usleep(250 * 1000); //sleep for 250ms
        else
            break; //got something
    }
    return cmd;
}

//used by host to retrieve commands
void enqueue_Command(char* cmd)
{
    //lock to add command to queue
    pthread_mutex_lock(&mutex);
    {
        command_queue[0] = cmd;
    }
    pthread_mutex_unlock(&mutex);
}

// this function is run by the native thread
void* sendhello(void *args)
{
    while (isRunning)
    {
        //create command buffer
        int length = 10;
        char* cmd = (char*)malloc(length);
        strcpy(cmd, "hi there");

        //enqueue command to be retrieved by host
        enqueue_Command(cmd);

        //put thread to sleep
        sleep(1);
    }
    return NULL;
}

int initialize()
{
    //init
    isRunning = 1;
    command_queue[0] = NULL;

    //create native thread which acts as the producer */
    if (pthread_create(&sendhello_thread, NULL, sendhello, NULL))
        return -1;
    else
        return 0;
}

int deinitialize()
{
    isRunning = 0;

    //wait for the native thread to finish
    if (pthread_join(sendhello_thread, NULL))
        return -1;
    else
        return 0;
}