 /* 
 * File Name          : mc_protocol.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 10/29/2015         | Initial Revision                | 
 
 */

#include "mc_protocol.h"
#include "mc_profiles.h"
#include "cc_hal_wifi.h"
#include "cc_hal_pm.h"
#include "cc_utils.h"
#include "mc_profiles.h"
#include "mc_hal_mpu9250.h"

//****************************************************************************
//
//! \brief	This fuction prase the command, which is recieve from pc by udp or tcp.
//!
//!
//! \param[in] packet is the point to the data packet head.
//!
//! \param[in] len is the length of the data packet.
//!
//! \param[in] command is the command code.
//!
//! \return     1 on success, error code on error.
//!

//
//****************************************************************************
void ParsePacket(uint8_t* key_value, uint16_t key_length, uint8_t key)
{
    switch( (COMMAND_KEY)key ){
    //OTA update
    case OTA: 
        break;
    
    /* CTRL1 command */
    //Configuration, set profiles.
    case PFL_SET:
        if(key_length == PROFILES_SIZE)
        {
            memcpy(&g_profiles, key_value, key_length);
            WriteProfiles();
        }
        else
            printf("Profiles size error!\r\n");
        break;
    
    /* CTRL2 command */
    case BROADCAST: 
        //SendMsgByUdp((uint8_t*)(&g_profiles), PROFILES_SIZE, BROADCAST);
        SendMsgByUdp((uint8_t*)(&g_profiles), sizeof(g_profiles), BROADCAST);
        break;
    case START_RAW:
        MPU_Start();
        break;
    case STOP_RAW: 
        MPU_Stop();
        break;
    case RESET:
        Reboot();
        break;
    case HIBERNATE:
        Hibernate();
        break;
                
        
    }
}
