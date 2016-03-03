 /* 
 * File Name          : free_rtos_hook.h
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 13/10/2015         | Initial Revision                | 
 
 */

typedef void* OsiTaskHandle;

void vAssertCalled( const char *pcFile, unsigned long ulLine );
void vApplicationIdleHook( void);
void vApplicationIdleHook( void);
void vApplicationStackOverflowHook( OsiTaskHandle *pxTask,
                                   signed char *pcTaskName);