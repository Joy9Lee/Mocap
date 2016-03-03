 /* 
 * File Name          : mc_protocol.h
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 10/29/2015         | Initial Revision                | 
 
 */

#ifndef __MC_PROTOCOL_H__
#define __MC_PROTOCOL_H__

#include <stdint.h>

#define PACKET_HEADER_LENGTH    4
#define PROTOCAL_VERSION        0xA0


//raw motion data
typedef struct _Motion_Data_Struct
{
	uint8_t Acc[3][2];
	uint8_t Meg[3][2];
	uint8_t Gyo[3][2];

}Motion_Data_Struct;

typedef struct _Motion_Send_Struct
{
	int16_t	Acc[3];
	int16_t	Meg[3];
	int16_t	Gyo[3];
}Motion_Send_Struct;

typedef struct _Motion_Package_Struct
{
    uint16_t 		TimeStape;
    Motion_Send_Struct	MotionData[10];
    uint16_t	        CkSum;

}Motion_Package_Struct;

typedef struct
{
  uint8_t       version;
  uint8_t       key;
  uint16_t      length;
}Packet_Header_t;


typedef enum    
{
  OTA = 0x00,
  SET = 0x01,
  CTRL = 0x02,
  DATA = 0x03,
  DEBUG = 0x04
}APP_COMMAND;


typedef enum
{
    //CTRL1_KEY
    PFL_SET = 0x10,
    
    //CTRL2_KEY
    BROADCAST = 0x20,
    START_RAW = 0x21,
    STOP_RAW = 0x22,
    START_IMU = 0x23,
    STOP_IMU = 0x24,
    START_EMG = 0x25,
    STOP_EMG = 0x26,
    RESET = 0x27,
    HIBERNATE = 0x28,
        
    //DATA_KEY
    RAW = 0x30,
    IMU = 0x31,
    EMG = 0x32
}COMMAND_KEY;


void ParsePacket(uint8_t* data, uint16_t len, uint8_t key_num);

#endif //__MC_PROTOCOL_H__