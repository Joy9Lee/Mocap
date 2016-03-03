//*****************************************************************************
// pinmux.c
//
// configure the device pins for different peripheral signals
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

// This file was automatically generated on 7/21/2014 at 3:06:20 PM
// by TI PinMux version 3.0.334
//
//*****************************************************************************

#include "cc_pinmux.h"
#include "hw_types.h"
#include "hw_memmap.h"
#include "hw_gpio.h"
#include "pin.h"
#include "rom.h"
#include "rom_map.h"
#include "prcm.h"
#include "gpio.h"

//*****************************************************************************
void led_pinmux(void)
{
    //
    // Configure PIN_64 for TIMERPWM5 GT_PWM05
    //
    MAP_PinTypeTimer(PIN_64, PIN_MODE_3);

    //
    // Configure PIN_01 for TIMERPWM6 GT_PWM06
    //
    MAP_PinTypeTimer(PIN_01, PIN_MODE_3);

    //
    // Configure PIN_02 for TIMERPWM7 GT_PWM07
    //
    MAP_PinTypeTimer(PIN_02, PIN_MODE_3);
}

void sw_pinmux(void)
{
    //
    // Configure PIN_17 for GPIO Input
    //
    MAP_PRCMPeripheralClkEnable(PRCM_GPIOA3, PRCM_RUN_MODE_CLK);
    MAP_PinTypeGPIO(PIN_17, PIN_MODE_0, false);
   // MAP_ConfigSet(PIN_04, PIN_STRENGTH_2MA, PIN_TYPE_STD);
    MAP_GPIODirModeSet(GPIOA3_BASE, GPIO_PIN_1, GPIO_DIR_MODE_IN);
    
}

void mpu9250_pinmux()
{
    MAP_PRCMPeripheralClkEnable(PRCM_GPIOA2, PRCM_RUN_MODE_CLK);
    /* SPI PINMUX */
    // Configure PIN_05 for SPI0 GSPI_CLK
    MAP_PinTypeSPI(PIN_05, PIN_MODE_7);
    
    // Configure PIN_06 for SPI0 GSPI_MISO
    MAP_PinTypeSPI(PIN_06, PIN_MODE_7);
    
    // Configure PIN_07 for SPI0 GSPI_MOSI
    MAP_PinTypeSPI(PIN_07, PIN_MODE_7);
    
    // Configure PIN_07 for SPI0 GSPI_CS
    //MAP_PinTypeSPI(PIN_08, PIN_MODE_7);
    // Configure PIN_08 for GPIO Output, CS
    MAP_PinTypeGPIO(PIN_08, PIN_MODE_0, false);
    MAP_GPIODirModeSet(GPIOA2_BASE, 0x2, GPIO_DIR_MODE_OUT);
    
    // Configure PIN_15 for GPIO Input, INT
    MAP_PinTypeGPIO(PIN_15, PIN_MODE_0, false);
    MAP_GPIODirModeSet(GPIOA2_BASE, 0x40, GPIO_DIR_MODE_IN);
}

void Ads129x_Pinmux()
{
    /* SPI PINMUX */
    // Configure PIN_05 for SPI0 GSPI_CLK
    MAP_PinTypeSPI(PIN_05, PIN_MODE_7);
    
    // Configure PIN_06 for SPI0 GSPI_MISO
    MAP_PinTypeSPI(PIN_06, PIN_MODE_7);
    
    // Configure PIN_07 for SPI0 GSPI_MOSI
    MAP_PinTypeSPI(PIN_07, PIN_MODE_7);
    
    // Configure PIN_58 for GPIO Output, SPI_CS
    MAP_PinTypeGPIO(PIN_58, PIN_MODE_0, false);
    MAP_GPIODirModeSet(GPIOA0_BASE, 0x8, GPIO_DIR_MODE_OUT);
    
    
    /* GPIO PINMUX */
    // Configure PIN_60 for GPIO Output, RESET
    MAP_PinTypeGPIO(PIN_60, PIN_MODE_0, false);
    MAP_GPIODirModeSet(GPIOA0_BASE, 0x20, GPIO_DIR_MODE_OUT);

    // Configure PIN_61 for GPIO Output, PWND
    MAP_PinTypeGPIO(PIN_61, PIN_MODE_0, false);
    MAP_GPIODirModeSet(GPIOA0_BASE, 0x40, GPIO_DIR_MODE_OUT);

    // Configure PIN_62 for GPIO Input, DRDY
    MAP_PinTypeGPIO(PIN_62, PIN_MODE_0, false);
    MAP_GPIODirModeSet(GPIOA0_BASE, 0x80, GPIO_DIR_MODE_IN);

    // Configure PIN_08 for GPIO Output, START
    MAP_PinTypeGPIO(PIN_08, PIN_MODE_0, false);
    MAP_GPIODirModeSet(GPIOA2_BASE, 0x2, GPIO_DIR_MODE_OUT);

    // Configure PIN_15 for GPIO Input, INT
    MAP_PinTypeGPIO(PIN_15, PIN_MODE_0, false);
    MAP_GPIODirModeSet(GPIOA2_BASE, 0x40, GPIO_DIR_MODE_IN);
}
