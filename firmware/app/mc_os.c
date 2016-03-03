 /* 
 * File Name          : mc_os.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 15/12/2015         | Initial Revision                | 
 
 */

#include "mc_os.h"
#include "mc_protocol.h"
#include "cc_utils.h"
#include "mc_process.h"

#define OSI_STACK_SIZE      2048

#define MOTION_QUEUE_SIZE 3
#define EMG_QUEUE_SIZE 3

OsiTaskHandle MotionTaskHandle;

static OsiMsgQ_t MotionQueue;

void MotionQueueInit()
{
    OsiReturnVal_e ret;

    ret = osi_MsgQCreate(&MotionQueue, (char *)"123", sizeof(Motion_Data_Struct), 3);
    if(ret < 0)
    {
        printf("motion queue creat error, %d.\r\n", ret);
    }
}

void MotionQueueDeinit()
{
    OsiReturnVal_e ret;
    
    ret = osi_MsgQDelete(MotionQueue);
    if(ret < 0)
    {
        printf("motion queue delete error, %d.\r\n", ret);
    }
}

void MotionQueueWrite(void* pMsg)  
{
    OsiReturnVal_e ret;
    
    ret =  osi_MsgQWrite(&MotionQueue, pMsg , OSI_WAIT_FOREVER);//OSI_WAIT_FOREVER
    if(ret < 0)
    {
        printf("motion queue write error, %d.\r\n", ret);
    }
}

void MotionQueueRead(void* pMsg)  
{
    OsiReturnVal_e ret;
    ret = osi_MsgQRead(&MotionQueue, pMsg , OSI_WAIT_FOREVER);
    if(ret < 0)
    {
        printf("motion queue read error, %d.\r\n", ret);
    }
}

void MotionSendTaskCreat()  
{
    OsiReturnVal_e ret;
    ret = osi_TaskCreate( MotionDataProcess, \
                                (const signed char*)"Motion Wifi Send Task", \
                                OSI_STACK_SIZE, NULL, 1, &MotionTaskHandle );
    if(ret < 0)
    {
        printf("motion send tack create error, %d.\r\n", ret);
    }
}