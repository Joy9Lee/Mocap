 /* 
 * File Name          : cc_hal_timer.h
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 28/09/2015         | Initial Revision                | 
 
 */

#ifndef __CC_HAL_TIMER_H__
#define	__CC_HAL_TIMER_H__

#include "hw_types.h"
#include "timer.h"
//*****************************************************************************
//
// If building with a C++ compiler, make all of the definitions in this header
// have a C binding.
//
//*****************************************************************************
#ifdef __cplusplus
extern "C"
{
#endif


/****************************************************************************/
/*								MACROS										*/
/****************************************************************************/
#define SYS_CLK				    80000000
#define MILLISECONDS_TO_TICKS(ms)   ((SYS_CLK/1000) * (ms))
#define PERIODIC_TEST_LOOPS     5

void Timer_IF_Init( unsigned long ePeripheralc, unsigned long ulBase,
    unsigned long ulConfig, unsigned long ulTimer, unsigned long ulValue);
void Timer_IF_IntSetup(unsigned long ulBase, unsigned long ulTimer,
                   void (*TimerBaseIntHandler)(void));
void Timer_IF_InterruptClear(unsigned long ulBase);
void Timer_IF_Start(unsigned long ulBase, unsigned long ulTimer,
                unsigned long ulValue);
void Timer_IF_Stop(unsigned long ulBase, unsigned long ulTimer);
void Timer_IF_ReLoad(unsigned long ulBase, unsigned long ulTimer,
                unsigned long ulValue);
unsigned int Timer_IF_GetCount(unsigned long ulBase, unsigned long ulTimer);
void Timer_IF_DeInit(unsigned long ulBase,unsigned long ulTimer);

#ifdef __cplusplus
}
#endif
#endif