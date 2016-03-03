 /* 
 * File Name          : mc_profiles.h
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 11/15/2015         | Initial Revision                | 
 
 */

#ifndef __MC_PROFILES_H__
#define __MC_PROFILES_H__

#include "stdint.h"

#define PROFILES_SIZE   80

typedef struct
{
    //To memery align, sort the members by length.
    uint8_t	ID;
    uint8_t 	BAT;
    uint8_t	MOTION_EN;
    uint8_t	EMG_CHL;
    uint16_t	MOTION_RATE;    
    uint16_t	EMG_RATE;
    float	ACC_FACTOR[3];
    float	ACC_BIAS[3];
    float	MEG_FACTOR[3];
    float	MEG_BIAS[3];
    float	GYO_FACTOR[3];
    float	GYO_BIAS[3];
}Node_Profiles_t;

typedef struct
{
    uint8_t     emg1[3];
    uint8_t     emg2[3];
    uint8_t     emg3[3];
    uint8_t     emg4[4];
}Emg_Data_t;


extern Node_Profiles_t g_profiles;

int WriteProfiles();
int ReadProfiles();
int DelProfiles();

#endif //__MC_PROFILES_H__