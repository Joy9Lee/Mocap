 /* 
 * File Name          : cc_hal_gpio.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 28/09/2015         | Initial Revision                | 
 
 */

// Standard includes
#include <stdio.h>
#include "cc_hal_gpio.h"
// Driverlib includes
#include "hw_types.h"
#include "hw_ints.h"
#include "hw_memmap.h"
#include "hw_apps_rcm.h"
#include "interrupt.h"
#include "pin.h"
#include "gpio.h"
#include "prcm.h"
#include "rom.h"
#include "rom_map.h"

// OS includes
#if defined(USE_TIRTOS) || defined(USE_FREERTOS) || defined(SL_PLATFORM_MULTI_THREADED)
#include <stdlib.h>
#include "osi.h"
#endif
static unsigned char GetPeripheralIntNum(unsigned int uiGPIOPort);
static void GPIO_IF_ConfigureNIntEnable(Gpio_Type gpio, unsigned int uiIntType, void (*pfnIntHandler)(void));
//****************************************************************************
//                      GLOBAL VARIABLES
//****************************************************************************
static unsigned long ulReg[]=
{
    GPIOA0_BASE,
    GPIOA1_BASE,
    GPIOA2_BASE,
    GPIOA3_BASE,
    GPIOA4_BASE
};


static unsigned char
GetPeripheralIntNum(unsigned int uiGPIOPort)
{
    switch(uiGPIOPort)
    {
       case GPIOA0_BASE:
          return INT_GPIOA0;
       case GPIOA1_BASE:
          return INT_GPIOA1;
       case GPIOA2_BASE:
          return INT_GPIOA2;
       case GPIOA3_BASE:
          return INT_GPIOA3;
       default:
          return INT_GPIOA0;
    }
}

//****************************************************************************
//
//! Configures the GPIO selected as input to generate interrupt on activity
//!
//! \param uiGPIOPort is the GPIO port address
//! \param ucGPIOPin is the GPIO pin of the specified port
//! \param uiIntType is the type of the interrupt (refer gpio.h)
//! \param pfnIntHandler is the interrupt handler to register
//!
//! This function
//!    1. Sets GPIO interrupt type
//!    2. Registers Interrupt handler
//!    3. Enables Interrupt
//!
//! \return None
//
//****************************************************************************
static void GPIO_IF_ConfigureNIntEnable(Gpio_Type gpio,
                                  unsigned int uiIntType,
                                  void (*pfnIntHandler)(void))
{
    //
    // Set GPIO interrupt type
    //
    MAP_GPIOIntTypeSet(gpio.Gport, gpio.Gpin, uiIntType);

    //
    // Register Interrupt handler
    //
    

#if defined(USE_TIRTOS) || defined(USE_FREERTOS) || defined(SL_PLATFORM_MULTI_THREADED)
     //USE_TIRTOS: if app uses TI-RTOS (either networking/non-networking)
     //USE_FREERTOS: if app uses Free-RTOS (either networking/non-networking)
     //SL_PLATFORM_MULTI_THREADED: if app uses any OS + networking(simplelink)
    osi_InterruptRegister(GetPeripheralIntNum(gpio.Gport),
                                        pfnIntHandler, INT_PRIORITY_LVL_1);

#else
   
    MAP_IntPrioritySet(GetPeripheralIntNum(gpio.Gport), INT_PRIORITY_LVL_1);
    MAP_GPIOIntRegister(gpio.Gport,pfnIntHandler);
#endif

    //
    // Enable Interrupt
    //
    MAP_GPIOIntClear(gpio.Gport, gpio.Gpin);
    MAP_GPIOIntEnable(gpio.Gport, gpio.Gpin);
}


//****************************************************************************
//
//! Get the port and pin of a given GPIO
//!
//! \param ucPin is the pin to be set-up as a GPIO (0:39)
//! \param-out gpio is the Gpio_Type object repect the gpio pin.  
//!
//! This function
//!    1. Return the GPIO port address and pin for a given external pin number
//!
//! \return None.
//
//****************************************************************************
void GPIO_IF_GetPortNPin(unsigned char ucPin, Gpio_Type *gpio)
{
    gpio->Pin = ucPin;
    //
    // Get the GPIO pin from the external Pin number
    //
    gpio->Gpin = 1 << (ucPin % 8);

    //
    // Get the GPIO port from the external Pin number
    //
    gpio->Gport = (ucPin / 8);
    gpio->Gport = ulReg[gpio->Gport];
}


//****************************************************************************
//
//! Set a value to the specified GPIO pin
//!
//! \param gpio is the Gpio_Type object repect the gpio pin.  
//! \param ucGPIOValue is the value to be set
//!
//! This function
//!    1. Sets a value to the specified GPIO pin
//!
//! \return None.
//
//****************************************************************************
void GPIO_IF_Set(Gpio_Type gpio, unsigned char ucGPIOValue)
{
    //
    // Set the corresponding bit in the bitmask
    //
    ucGPIOValue = ucGPIOValue << (gpio.Pin % 8);

    //
    // Invoke the API to set the value
    //
    MAP_GPIOPinWrite(gpio.Gport, gpio.Gpin, ucGPIOValue);
}

//****************************************************************************
//
//! Set a value to the specified GPIO pin
//!
//! \param gpio is the Gpio_Type object repect the gpio pin.  
//!
//! This function
//!    1. Gets a value of the specified GPIO pin
//!
//! \return value of the GPIO pin
//****************************************************************************
unsigned char GPIO_IF_Get(Gpio_Type gpio)
{
    unsigned char ucGPIOValue;
    long lGPIOStatus;

    //
    // Invoke the API to Get the value
    //
    lGPIOStatus =  MAP_GPIOPinRead(gpio.Gport, gpio.Gpin);

    //
    // Set the corresponding bit in the bitmask
    //
    ucGPIOValue = lGPIOStatus >> (gpio.Pin % 8);
    return ucGPIOValue;
}

//****************************************************************************
//
//! Clears the interrupt for the specified pin(s).
//!
//! \param gpio is the Gpio_Type object.  
//!
//! This function
//!    Clears the interrupt for the specified pin(s).
//!
//! \return none
//****************************************************************************
void GPIO_IF_INTCLR(Gpio_Type gpio)
{
    MAP_GPIOIntClear(gpio.Gport, gpio.Gpin);
}

//****************************************************************************
//
//! Initial the interupt for for the specified pin(s).
//!
//! \param gpio is the Gpio_Type object represent the gpio pin.  
//! \param uiIntType is the type of the interrupt (refer gpio.h)
//! \param pfnIntHandler is the interrupt handler to register
//!
//! This function
//!    Initial the interrupt for the specified pin(s).
//!
//! \return none
//****************************************************************************
void GPIO_IF_INTINI(Gpio_Type gpio, unsigned int uiIntType, void (*pfnIntHandler)(void))
{
    MAP_GPIOIntClear(gpio.Gport, gpio.Gpin);
    GPIO_IF_ConfigureNIntEnable(gpio , uiIntType, pfnIntHandler);
    MAP_IntPendClear(INT_GPIOA0 + gpio.Pin / 8);
    MAP_IntEnable(INT_GPIOA0 + gpio.Pin / 8);
}