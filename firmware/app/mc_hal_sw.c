 /* 
 * File Name          : mc_hal_sw.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 10/10/2015         | Initial Revision                | 
 
 */

#include "mc_hal_sw.h"
#include "cc_hal_gpio.h"
#include "cc_pinmux.h"
#include "cc_hal_timer.h"
#include "cc_utils.h"

#include "gpio.h"
#include "prcm.h"
   
#define SW_GPIO_NUM 24

#define PRCM_TIMERA0              0x0000000E
#define TIMERA0_BASE            0x40030000
#define TIMERA0_BASE            0x40030000
#define TIMER_A                 0x000000ff  // Timer A
#define TIMER_CFG_A_ONE_SHOT     0x00000021  // Timer A one-shot timer
#define TIMER_CFG_ONE_SHOT       0x00000021 
#define TIMER_BOTH              0x0000ffff  // Timer Both


static Gpio_Type m_sButton; 
static Button_Handler_Type m_pClkHdr;
static Button_Handler_Type m_pDoubleClkHdr;
static Button_Handler_Type m_pLongClkHdr;


static void SW_Handler();

//****************************************************************************
//
//! Initial the m_sButtonitch as the button.
//!
//! \param pfnIntHandler is the callback function, the button triggered.
//!
//! This function
//!    Initial the interrupt for the specified pin(s).
//!
//! \return none

void sw_init(Button_Handler_Type pClkHdr, Button_Handler_Type pDoubleClkHdr, Button_Handler_Type pLongClkHdr )
{
    GPIO_IF_GetPortNPin(SW_GPIO_NUM, &m_sButton);
    sw_pinmux();
    GPIO_IF_INTCLR(m_sButton);
    GPIO_IF_INTINI(m_sButton, GPIO_BOTH_EDGES, SW_Handler);
    PRCMRTCInUseSet(); 

    m_pClkHdr = pClkHdr;
    m_pDoubleClkHdr = pDoubleClkHdr;
    m_pLongClkHdr = pLongClkHdr;
}


//****************************************************************************
//
//! The Interupt handler to clear flag and call the user function.
//!
//!
//! \return none
static void SW_Handler()
{
    static unsigned long m_RtcSecs1 = 0;
    static unsigned short m_RTCMsec1 = 0;
    static unsigned long m_RtcSecs2 = 0;
    static unsigned short m_RTCMsec2 = 0;
    static unsigned long gap;
    
    GPIO_IF_INTCLR(m_sButton);
    long state = GPIOPinRead(m_sButton.Gport, m_sButton.Gpin);
    if(state == 1 << (m_sButton.Gpin - 1))
    {
        GetTime(&m_RtcSecs1, &m_RTCMsec1);
    }
    else
    {
        gap = (m_RtcSecs1 - m_RtcSecs2) * 1000 + (m_RTCMsec1 - m_RTCMsec2);
        if(gap < 500)
        {
            (*m_pDoubleClkHdr)();
            return;
        }
        
        GetTime(&m_RtcSecs2, &m_RTCMsec2);
        gap = (m_RtcSecs2 - m_RtcSecs1) * 1000 + (m_RTCMsec2 - m_RTCMsec1);
        if(gap < 1000)
        {
            (*m_pClkHdr)();
        }
        else
        {
            (*m_pLongClkHdr)();
        }
    }
}
