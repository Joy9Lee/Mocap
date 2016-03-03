 /* 
 * File Name          : mc_hal_led.h
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 28/09/2015         | Initial Revision                | 
 
 */

#ifndef __MC_HAL_LED_H__
#define __MC_HAL_LED_H__

void led_init(void);
void led_display(unsigned char r, unsigned char g, unsigned char b);

#endif   // __MC_HAL_LED_H__