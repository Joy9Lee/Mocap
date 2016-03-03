 /* 
 * File Name          : mc_hal_sw.h
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 10/10/2015         | Initial Revision                | 
 
 */

#ifndef __MC_HAL_SW_H__
#define __MC_HAL_SW_H__

typedef void (*Button_Handler_Type)(void ); 
void sw_init(Button_Handler_Type pClkHdr, Button_Handler_Type pDoubleClkHdr, Button_Handler_Type pLongClkHdr );


#endif //__MC_HAL_SW_H__