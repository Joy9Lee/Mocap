 /* 
 * File Name          : cc_hal_pwm.h
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 28/09/2015         | Initial Revision                | 
 
 */
 
#ifndef __CC_HAL_PWM_H__
#define __CC_HAL_PWM_H__

void Update3(unsigned char ucLevel1, unsigned char ucLevel2, unsigned char ucLevel3 );
void InitPWMModules();
void DeInitPWMModules();

#endif   // __CC_HAL_PWM_H__