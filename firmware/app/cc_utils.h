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

#ifndef _CC_UTILS_H_
#define _CC_UTILS_H_

#include <stdio.h>



//#ifdef DEBUG
int printf(const char * a, ...);
//#endif

void Delay_us(unsigned long ulCount);
long Clocker();
void GetTime(unsigned long *  ulSecs, unsigned short *  usMsec);
void SetTime(unsigned long  ulSecs, unsigned short  usMsec);


#endif //_CC_UTILS_H_