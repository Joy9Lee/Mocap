 /* 
 * File Name          : mc_hal_led.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 28/09/2015         | Initial Revision                | 
 */
 
#include "mc_hal_led.h"
#include "cc_hal_pwm.h"
#include "cc_pinmux.h"

//*****************************************************************************
void led_init(void)
{
    led_pinmux();
    InitPWMModules();
}

void led_display(unsigned char r, unsigned char g, unsigned char b)
{
    Update3(r,g,b);
}
