 /* 
 * File Name          : mc_process.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 15/12/2015         | Initial Revision                | 
 
 */

#include "mc_process.h"
#include "mc_protocol.h"
#include "mc_os.h"

Motion_Package_Struct MotionPackage;

void MotionDataProcess()
{
    int i = 0, count = 0;
    int16_t temp;
    Motion_Data_Struct MotionData;
    Motion_Send_Struct Data;
    while(1)
    {
        MotionQueueRead(&(MotionData));
    //        for(i = 0; i<3; i++)
//        {
//        	temp = (MotionData.Acc[i][0]<<8) | MotionData.Acc[i][1];
//        	Data.Acc[i] = (float)temp / 2048 *9.8;
//        	temp = (MotionData.Meg[i][1]<<8) | MotionData.Meg[i][0];
//        	Data.Meg[i] = (float)temp * 0.3;
//        	temp = (MotionData.Gyo[i][0]<<8) | MotionData.Gyo[i][1];
//        	Data.Gyo[i] = (float)temp / 16.4;
//        }
        for(i = 0; i<3; i++)
        {
            Data.Acc[i] = (MotionData.Acc[i][0]<<8) | MotionData.Acc[i][1];
            Data.Meg[i] = (MotionData.Meg[i][1]<<8) | MotionData.Meg[i][0];
            Data.Gyo[i] = (MotionData.Gyo[i][0]<<8) | MotionData.Gyo[i][1];
        }
        //meg coordinate align to acc and gyo
        temp = Data.Meg[0];
        Data.Meg[0] = Data.Meg[1];
        Data.Meg[1] = temp;
        Data.Meg[2] = -Data.Meg[2];

        //right hand coordinate to left
        Data.Acc[1] = -Data.Acc[1];
        Data.Meg[1] = -Data.Meg[1];
        Data.Gyo[1] = -Data.Gyo[1];

        Data.Gyo[0] = -Data.Gyo[0];
        Data.Gyo[1] = -Data.Gyo[1];
        Data.Gyo[2] = -Data.Gyo[2];

//        //calibration
//        for(i = 0;i < 3; i++)
//        {
//        	Data.Acc[i] = Data.Acc[i] * g_profiles.ACC_FACTOR[i] + g_profiles.ACC_BIAS[i];
//        	Data.Meg[i] = Data.Meg[i] * g_profiles.MEG_FACTOR[i] + g_profiles.MEG_BIAS[i];
//        	Data.Gyo[i] = Data.Gyo[i] * g_profiles.GYO_FACTOR[i] + g_profiles.GYO_BIAS[i];
//        }

        MotionPackage.MotionData[count] = Data;
        if(count>=9)
        {
            MotionPackage.TimeStape++;
            SendMsgByTcp(&MotionPackage, sizeof(Motion_Package_Struct), 0x30);
            count=0;
        }
        else
        {
            count++;
        }
    }
}