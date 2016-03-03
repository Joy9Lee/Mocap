 /* 
 * File Name          : cc_hal_wifi.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 10/14/2015         | Initial Revision                | 
 
 */
#include "mc_profiles.h"
#include "cc_utils.h"
// Simplelink includes
#include "simplelink.h"

#define USER_FILE_NAME          "profiles.txt"

Node_Profiles_t g_profiles;


static long m_FileHandle;
static unsigned long m_Token;

#define EMG_KEY_VALUE_TIMESTAMP         0
#define EMG_KEY_VALUE_DATACOUNT         1
#define EMG_KEY_VALUE_DATA              2

#define EMG_HEAD_LEN    2
#define EMG_DATA_NUM    20
#define EMG_DATA_LEN    12

static uint8_t m_EmgBuffer[EMG_HEAD_LEN + EMG_DATA_NUM * EMG_DATA_LEN];

static void Profiles_Init();

static void Profiles_Init()
{
	int i;
	g_profiles.ID 		= 0;
        g_profiles.BAT 		= 200;
        g_profiles.MOTION_EN    = 1;
	g_profiles.EMG_CHL 	= 0x00;
	g_profiles.EMG_RATE	= 1000;
	g_profiles.MOTION_RATE	= 100;
	for(i = 0; i < 3; i++)
	{
		g_profiles.ACC_FACTOR[i]= 	1;
		g_profiles.ACC_BIAS[i]	=	0;
		g_profiles.MEG_FACTOR[i]= 	1;
		g_profiles.MEG_BIAS[i]	=	0;
		g_profiles.GYO_FACTOR[i]= 	1;
		g_profiles.GYO_BIAS[i]	=	0;
	}
}

//*****************************************************************************
//
//!  This funtion includes the following steps:
//!  -open a user file for writing
//!  -write "Old MacDonalds" child song 37 times to get just below a 64KB file
//!  -close the user file
//!
//!  /param[out] m_Token : file token
//!  /param[out] m_FileHandle : file handle
//!
//!  /return  0:Success, -ve: failure
//
//*****************************************************************************
int WriteProfiles()
{
    long lRetVal = -1;

    //
    //  open a user file for writing
    //
    lRetVal = sl_FsOpen((unsigned char *)USER_FILE_NAME,
                        FS_MODE_OPEN_WRITE,
                        &m_Token,
                        &m_FileHandle);
    if(lRetVal < 0)
    {
        lRetVal = sl_FsClose(m_FileHandle, 0, 0, 0);
        printf("file may not exist.\n\r");

        //
        //  create a user file
        //
        lRetVal = sl_FsOpen((unsigned char *)USER_FILE_NAME,
                    FS_MODE_OPEN_CREATE(256, \
                              _FS_FILE_OPEN_FLAG_COMMIT|_FS_FILE_PUBLIC_WRITE),
                            &m_Token,
                            &m_FileHandle);
        if(lRetVal < 0)
        {
            //
            // File may already be created
            //
            lRetVal = sl_FsClose(m_FileHandle, 0, 0, 0);
            printf("File creat error. \n\r");
        }
        else
        {
            //
            // close the user file
            //
            lRetVal = sl_FsClose(m_FileHandle, 0, 0, 0);
            if (SL_RET_CODE_OK != lRetVal)
            {
                printf("file close error\n\r");
            }
        }
        //Open file again
        lRetVal = sl_FsOpen((unsigned char *)USER_FILE_NAME,
                            FS_MODE_OPEN_WRITE,
                            &m_Token,
                            &m_FileHandle);
        if(lRetVal < 0)
        {
            lRetVal = sl_FsClose(m_FileHandle, 0, 0, 0);
            printf("creat and open error.\n\r");
        }

    }

    //
    // write "Old MacDonalds" child song as many times to get just below a 64KB file
    //

	lRetVal = sl_FsWrite(m_FileHandle,
				0,
				(unsigned char *)(uint8_t *)(&g_profiles), sizeof(g_profiles));
	if (lRetVal < 0)
	{
		lRetVal = sl_FsClose(m_FileHandle, 0, 0, 0);
		printf("file write fail\n\r");
	}
    //
    // close the user file
    //
    lRetVal = sl_FsClose(m_FileHandle, 0, 0, 0);
    if (SL_RET_CODE_OK != lRetVal)
    {
        printf("file close error\n\r");
    }

    return 1;
}


//*****************************************************************************
//
//!  This funtion includes the following steps:
//!    -open the user file for reading
//!    -read the data and compare with the stored buffer
//!    -close the user file
//!
//!  /param[in] m_Token : file token
//!  /param[in] m_FileHandle : file handle
//!
//!  /return 0: success, -ve:failure
//
//*****************************************************************************
int ReadProfiles()
{
    long lRetVal = -1;

    //
    // open a user file for reading
    //
    lRetVal = sl_FsOpen((unsigned char *)USER_FILE_NAME,
                        FS_MODE_OPEN_READ,
                        &m_Token,
                        &m_FileHandle);
    if(lRetVal < 0)
    {
        lRetVal = sl_FsClose(m_FileHandle, 0, 0, 0);
        printf("file not exist.\n\r");
        Profiles_Init();

    }
    else
    {
		//
		// read the data and compare with the stored buffer
		//

		lRetVal = sl_FsRead(m_FileHandle,
							0,
							(uint8_t *)(&g_profiles),
							sizeof(g_profiles));
		if ((lRetVal < 0) || (lRetVal != sizeof(g_profiles)))
		{
			lRetVal = sl_FsClose(m_FileHandle, 0, 0, 0);
			printf("file read fail");
		}

		//
		// close the user file
		//
		lRetVal = sl_FsClose(m_FileHandle, 0, 0, 0);
		if (SL_RET_CODE_OK != lRetVal)
		{
			printf("file closed error\n\r");
		}
    }
    return 1;
}

int DelProfiles()
{
    long lRetVal = -1;
    lRetVal = sl_FsDel((unsigned char *)USER_FILE_NAME, m_Token);
    if(lRetVal < 0)
    {
        printf("file delete failed.\n\r");
    }
    return lRetVal;
}