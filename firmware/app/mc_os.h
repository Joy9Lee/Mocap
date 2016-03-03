 /* 
 * File Name          : mc_os.h
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 15/12/2015         | Initial Revision                | 
 
 */
#ifndef __MC_OS_H__
#define __MC_OS_H__

#include "osi.h"
void MotionQueueInit();
void MotionQueueDeinit();
void MotionQueueWrite(void* pMsg);
void MotionQueueRead(void* pMsg) ;
void MotionSendTaskCreat();

#endif //__CC_OS_H__