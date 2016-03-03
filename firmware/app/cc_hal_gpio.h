 /* 
 * File Name          : cc_hal_gpio.h
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 28/09/2015         | Initial Revision                | 
 
 */
#ifndef __CC_HAL_GPIO_H__
#define __CC_HAL_GPIO_H__

#include "stdint.h"

typedef struct _Gpio_Type{
  uint8_t Pin;
  uint32_t Gport;
  uint8_t Gpin;
}Gpio_Type;


void GPIO_IF_GetPortNPin(unsigned char ucPin, Gpio_Type* gpio);

void GPIO_IF_Set(Gpio_Type gpio, unsigned char ucGPIOValue);
unsigned char GPIO_IF_Get(Gpio_Type gpio);
void GPIO_IF_INTCLR(Gpio_Type gpio);
void GPIO_IF_INTINI(Gpio_Type gpio, unsigned int uiIntType, void (*pfnIntHandler)(void));

#endif //__CC_HAL_GPIO_H__