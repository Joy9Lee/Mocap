 /* 
 * File Name          : cc_hal_wifi.h
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 10/13/2015         | Initial Revision                | 
 
 */

#ifndef __CC_HAL_WIFI__H__
#define __CC_HAL_WIFI__H__

#include <stdint.h>

//
// Values for below macros shall be modified for setting the 'Ping' properties
//
#define PING_INTERVAL       1000    /* In msecs */
#define PING_TIMEOUT        3000    /* In msecs */
#define PING_PKT_SIZE       20      /* In bytes */
#define NO_OF_ATTEMPTS      3
//
// Values for below macros shall be modified as per access-point(AP) properties
// SimpleLink device will connect to following AP when application is executed
//
#define SSID_NAME           "MMocap"    /* AP SSID */
#define SECURITY_TYPE       SL_SEC_TYPE_OPEN/* Security type (OPEN or WEP or WPA*/
#define SECURITY_KEY        ""              /* Password of the secured AP */
#define SSID_LEN_MAX        32
#define BSSID_LEN_MAX       6


// Loop forever, user can change it as per application's requirement
#define LOOP_FOREVER() \
            {\
                while(1); \
            }

// check the error code and handle it
#define ASSERT_ON_ERROR(error_code)\
            {\
                 if(error_code < 0) \
                   {\
                        printf("%d",error_code);\
                        return error_code;\
                 }\
            }

#define SPAWN_TASK_PRIORITY     9
#define SL_STOP_TIMEOUT         200
#define UNUSED(x)               ((x) = (x))
#define SUCCESS                 0
#define FAILURE                 -1


// Status bits - These are used to set/reset the corresponding bits in
// given variable
typedef enum{
    STATUS_BIT_NWP_INIT = 0, // If this bit is set: Network Processor is
                             // powered up

    STATUS_BIT_CONNECTION,   // If this bit is set: the device is connected to
                             // the AP or client is connected to device (AP)

    STATUS_BIT_IP_LEASED,    // If this bit is set: the device has leased IP to
                             // any connected client

    STATUS_BIT_IP_AQUIRED,   // If this bit is set: the device has acquired an IP

    STATUS_BIT_SMARTCONFIG_START, // If this bit is set: the SmartConfiguration
                                  // process is started from SmartConfig app

    STATUS_BIT_P2P_DEV_FOUND,    // If this bit is set: the device (P2P mode)
                                 // found any p2p-device in scan

    STATUS_BIT_P2P_REQ_RECEIVED, // If this bit is set: the device (P2P mode)
                                 // found any p2p-negotiation request

    STATUS_BIT_CONNECTION_FAILED, // If this bit is set: the device(P2P mode)
                                  // connection to client(or reverse way) is failed

    STATUS_BIT_PING_DONE         // If this bit is set: the device has completed
                                 // the ping operation

}e_StatusBits;


#define CLR_STATUS_BIT_ALL(status_variable)  (status_variable = 0)
#define SET_STATUS_BIT(status_variable, bit) status_variable |= (1<<(bit))
#define CLR_STATUS_BIT(status_variable, bit) status_variable &= ~(1<<(bit))
#define CLR_STATUS_BIT_ALL(status_variable)   (status_variable = 0)
#define GET_STATUS_BIT(status_variable, bit) (0 != (status_variable & (1<<(bit))))

#define IS_NW_PROCSR_ON(status_variable)     GET_STATUS_BIT(status_variable,\
                                                            STATUS_BIT_NWP_INIT)
#define IS_CONNECTED(status_variable)        GET_STATUS_BIT(status_variable,\
                                                         STATUS_BIT_CONNECTION)
#define IS_IP_LEASED(status_variable)        GET_STATUS_BIT(status_variable,\
                                                           STATUS_BIT_IP_LEASED)
#define IS_IP_ACQUIRED(status_variable)       GET_STATUS_BIT(status_variable,\
                                                          STATUS_BIT_IP_AQUIRED)
#define IS_SMART_CFG_START(status_variable)  GET_STATUS_BIT(status_variable,\
                                                   STATUS_BIT_SMARTCONFIG_START)
#define IS_P2P_DEV_FOUND(status_variable)    GET_STATUS_BIT(status_variable,\
                                                       STATUS_BIT_P2P_DEV_FOUND)
#define IS_P2P_REQ_RCVD(status_variable)     GET_STATUS_BIT(status_variable,\
                                                    STATUS_BIT_P2P_REQ_RECEIVED)
#define IS_CONNECT_FAILED(status_variable)   GET_STATUS_BIT(status_variable,\
                                                   STATUS_BIT_CONNECTION_FAILED)
#define IS_PING_DONE(status_variable)        GET_STATUS_BIT(status_variable,\
                                                           STATUS_BIT_PING_DONE)
void BsdUdpServer(void *PortNum);
void BsdTcpServer(void *PortNum);

void WlanStationMode( void *pvParameters );
int SendMsgByUdp(uint8_t* content, uint16_t len, uint8_t key_num);
int SendMsgByTcp(const uint8_t* content, uint16_t len, uint8_t key_num);



#endif //__CC_HAL_WIFI__H__