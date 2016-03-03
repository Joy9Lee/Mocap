 /* 
 * File Name          : cc_utils.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 10/10/2015         | Initial Revision                | 
 
 */

#include "cc_utils.h"
#include "utils.h"
#include "hw_types.h"
#include "hw_memmap.h"
#include "timer.h"
#include "prcm.h"

#define PRCM_HIB_SLOW_CLK_CTR     0x00000001
static long last_counter = 0;
//*****************************************************************************
//
//! Provides a small delay.
//!
//! \param ulCount is the number of usec delay iterations to perform.
//!
//! This function provides a means of generating a constant length delay.  It
//! is written in assembly to keep the delay consistent across tool chains,
//! avoiding the need to tune the delay based on the tool chain in use.
//!
//! The UtilsDelay takes 3(???) cycles, the processor provide 80Mhz. Real test is 6 cycles
//!
//! \return None.
//
//*****************************************************************************
void Delay_us(unsigned long ulCount)
{
	UtilsDelay((ulCount*80)/6);
}

//*****************************************************************************
//
//! Count the routines time consumption.
//!
//! \param None
//!

//! \return the running time in us.
//
//*****************************************************************************
long Clocker()
{
	long time;
	long current_counter;
	current_counter = TimerValueGet(TIMERA0_BASE,TIMER_A);
	if(last_counter > current_counter)
	{
	time =(last_counter - TimerValueGet(TIMERA0_BASE,TIMER_A))/80;	//GPT is decrease counter
	}
	else
	{
		time = (10*1000*80 - current_counter + last_counter)/80;	//10ms period	10ms*//(ms/us)//*80Mhz*2instruct
	}
	last_counter = TimerValueGet(TIMERA0_BASE,TIMER_A);
	return time;
}

//*****************************************************************************
//
//! Get the instantaneous calendar time from the device.
//!
//! \param ulSecs refers to the seconds part of the calendar time  
//! \param usMsec refers to the fractional (ms) part of the second 
//!
//! This function fetches the instantaneous value of the ticking calendar 
//! time from the device. The calendar time is outlined in terms of seconds 
//! and milliseconds.
//!
//! The device provides the calendar value that has been maintained across 
//! active and low power states.

//! \return none.
//
//*****************************************************************************
void GetTime(unsigned long *  ulSecs, unsigned short *  usMsec)
{
    PRCMRTCGet(ulSecs, usMsec);
}

//*****************************************************************************
//
//! Set the instantaneous calendar time from the device.
//!
//! \param ulSecs refers to the seconds part of the calendar time  
//! \param usMsec refers to the fractional (ms) part of the second 
//!
//! \return none.
//
//*****************************************************************************
void SetTime(unsigned long  ulSecs, unsigned short  usMsec)
{
    PRCMRTCInUseSet(); 
    PRCMRTCSet(ulSecs, usMsec);
}

//#ifdef DEBUG
int printf(const char * a, ...)
{
    return 0;
}
//#endif