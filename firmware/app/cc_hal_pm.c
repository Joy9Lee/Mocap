 /* 
 * File Name          : mc_hal_pm.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 10/10/2015         | Initial Revision                | 
 
 */

#include "cc_hal_pm.h"
#include "hw_types.h"
#include "rom_map.h"
#include "prcm.h"
#include "utils.h"
//****************************************************************************
//
//! Enter the HIBernate mode configuring the wakeup timer
//!
//! \param none
//! 
//! This function  
//!    1. Sets up the wakeup RTC timer
//!    2. Enables the RTC
//!    3. Enters into HIBernate 
//!
//! \return None.
//
//****************************************************************************
void Reboot()
{
#define SLOW_CLK_FREQ           (32*1024)


    //
    // Enable the HIB RTC
    //
    MAP_PRCMHibernateWakeupSourceEnable(PRCM_HIB_SLOW_CLK_CTR);

    //MAP_UtilsDelay(80000);
    
    //
    // Configure the HIB module RTC wake time
    //
    MAP_PRCMHibernateIntervalSet(2 * SLOW_CLK_FREQ);
    //
    // Enter HIBernate mode
    //
    MAP_PRCMHibernateEnter();
    while(1){;}
}

//****************************************************************************
//
//! Enter the HIBernate mode configuring the wakeup timer
//!
//! \param none
//! 
//! This function  
//!    1. Sets up the wakeup RTC timer
//!    2. Enables the RTC
//!    3. Enters into HIBernate 
//!
//! \return None.
//
//****************************************************************************
void Hibernate()
{

    PRCMHibernateWakeUpGPIOSelect (PRCM_HIB_GPIO24, PRCM_HIB_FALL_EDGE);
    //
    // Enable the HIB RTC
    //
    MAP_PRCMHibernateWakeupSourceEnable(PRCM_HIB_GPIO24);

    MAP_UtilsDelay(80000);

    //
    // Enter HIBernate mode
    //
    MAP_PRCMHibernateEnter();
}