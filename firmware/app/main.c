//*****************************************************************************
//
// Copyright (C) 2014 Texas Instruments Incorporated - http://www.ti.com/ 
// 
// 
//  Redistribution and use in source and binary forms, with or without 
//  modification, are permitted provided that the following conditions 
//  are met:
//
//    Redistributions of source code must retain the above copyright 
//    notice, this list of conditions and the following disclaimer.
//
//    Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the   
//    distribution.
//
//    Neither the name of Texas Instruments Incorporated nor the names of
//    its contributors may be used to endorse or promote products derived
//    from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT 
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT 
//  OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
//  SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT 
//  LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
//  DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
//  THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
//  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//*****************************************************************************

//*****************************************************************************
//
// Application Name     - PWM Application
// Application Overview - The general purpose timers (GPTs) support a 16-bit 
//                        pulse-width modulation (PWM) mode with software-
//                        programmable output inversion of the PWM signal.
//                        The brightness of the LEDs are varied from Off to On 
//                        as PWM output varies.
// Application Details  -
// http://processors.wiki.ti.com/index.php/CC32xx_PWM
// or
// docs\examples\CC32xx_PWM.pdf
//
//*****************************************************************************

//****************************************************************************
//
//! \addtogroup pwm
//! @{
//
//****************************************************************************
#include <stdio.h>

// Driverlib includes
#include "hw_types.h"
#include "hw_ints.h"
#include "hw_memmap.h"
#include "hw_apps_rcm.h"
#include "hw_common_reg.h"
#include "interrupt.h"
#include "rom.h"
#include "rom_map.h"
#include "utils.h"
#include "prcm.h"



#include <stdlib.h>

#include "osi.h"

#include "cc_pinmux.h"
#include "cc_hal_gpio.h"
#include "mc_hal_led.h"
#include "cc_hal_pm.h"
#include "cc_hal_wifi.h"
#include "mc_hal_sw.h"
#include "cc_utils.h"


#define APPLICATION_VERSION     "1.1.1"
#define DBG_PRINT               Report


//
// The PWM works based on the following settings:
//     Timer reload interval -> determines the time period of one cycle
//     Timer match value -> determines the duty cycle 
//                          range [0, timer reload interval]
// The computation of the timer reload interval and dutycycle granularity
// is as described below:
// Timer tick frequency = 80 Mhz = 80000000 cycles/sec
// For a time period of 0.5 ms, 
//      Timer reload interval = 80000000/2000 = 40000 cycles
// To support steps of duty cycle update from [0, 255]
//      duty cycle granularity = ceil(40000/255) = 157
// Based on duty cycle granularity,
//      New Timer reload interval = 255*157 = 40035
//      New time period = 0.5004375 ms
//      Timer match value = (update[0, 255] * duty cycle granularity)
//
#define TIMER_INTERVAL_RELOAD   40035 /* =(255*157) */
#define DUTYCYCLE_GRANULARITY   157

#define OSI_STACK_SIZE      2048
#define MAX_MSG_LENGTH			16


//*****************************************************************************
//                 GLOBAL VARIABLES -- Start
//*****************************************************************************
#if defined(ccs)
extern void (* const g_pfnVectors[])(void);
#endif
#if defined(ewarm)
extern uVectorEntry __vector_table;
#endif
//*****************************************************************************
//                 GLOBAL VARIABLES -- End
//*****************************************************************************


/****************************************************************************/
/*                      LOCAL FUNCTION DEFINITIONS                          */
/****************************************************************************/


//*****************************************************************************
//
//! Board Initialization & Configuration
//!
//! \param  None
//!
//! \return None
//
//*****************************************************************************
static void
BoardInit(void)
{
/* In case of TI-RTOS vector table is initialize by OS itself */
#ifndef USE_TIRTOS
  //
  // Set vector table base
  //
#if defined(ccs)
    MAP_IntVTableBaseSet((unsigned long)&g_pfnVectors[0]);
#endif
#if defined(ewarm)
    MAP_IntVTableBaseSet((unsigned long)&__vector_table);
#endif
#endif
    //
    // Enable Processor
    //
    MAP_IntMasterEnable();
    MAP_IntEnable(FAULT_SYSTICK);

    PRCMCC3200MCUInit();
}




//****************************************************************************
//
//! Demonstrates the controlling of LED brightness using PWM
//!
//! \param none
//! 
//! This function  
//!    1. Pinmux the GPIOs that drive LEDs to PWM mode.
//!    2. Initializes the timer as PWM.
//!    3. Loops to continuously change the PWM value and hence brightness 
//!       of LEDs.
//!
//! \return None.
//
//****************************************************************************
void led_on()
{
    led_display(254,245,254);
}

void led_off()
{
    led_display(254,254,254);
}

void main()
{
    long lRetVal = -1;
    //
    // Board Initialisation
    //

    BoardInit();
    Delay_us(200);
    sw_init(led_on, led_off, Hibernate);
    Delay_us(200);
    led_init();
    Delay_us(200);
        printf("OK! \r\n");
        
    Mpu9250_Init();
    



        led_display(254,235,245);
        Delay_us(2000000);


        

     //
    // Creating a queue for 10 elements.
    //
   // OsiReturnVal_e osi_retVal;
   // osi_retVal = osi_MsgQCreate(&MsgQ, "MSGQ", MAX_MSG_LENGTH, 10);

    
    //
    // Simplelinkspawntask
    //
    
    
    lRetVal = VStartSimpleLinkSpawnTask(SPAWN_TASK_PRIORITY);
    if(lRetVal < 0)
    {
        printf("%d",lRetVal);
        LOOP_FOREVER();
    }    

    //
    // Create WlanStationMode Task
    //
    lRetVal = osi_TaskCreate(WlanStationMode, (signed char*)"WlanStationMode", \
                                OSI_STACK_SIZE, NULL, \
                                1, NULL );
    if(lRetVal < 0)
    {
        printf("%d",lRetVal);
        LOOP_FOREVER();
    } 
    

    //
    // Start OS Scheduler
    //
    osi_start();

    while (1)
    {

    }
    
    while(1)
    {

     // led_display(80,80,80);
      //MAP_UtilsDelay(8000000);
      //led_display(2,2,2);
     // MAP_UtilsDelay(8000000);
    }

    //
    // De-Init peripherals - will not reach here...
    //
    //DeInitPWMModules();
}

//*****************************************************************************
//
// Close the Doxygen group.
//! @}
//
//*****************************************************************************
