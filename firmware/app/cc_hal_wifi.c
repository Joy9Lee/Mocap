 /* 
 * File Name          : cc_hal_wifi.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 10/13/2015         | Initial Revision                | 
 
 */
 
#include "cc_hal_wifi.h"
#include "mc_hal_led.h"
#include "cc_utils.h"
#include "mc_protocol.h"


#include <stdio.h>
// Simplelink includes
#include "simplelink.h"

//Driverlib includes
#include "hw_types.h"
#include "hw_ints.h"
#include "rom.h"
#include "rom_map.h"
#include "interrupt.h"
#include "prcm.h"

#include "osi.h"

//Free_rtos/ti-rtos includes
//#include "osi.h"

#define IP_ADDR             0xc0a8006E /* 192.168.0.110 */
#define TCP_PORT_NUM        5001
#define UDP_PORT_NUM        5002


#define BUF_SIZE            1400
#define OSI_STACK_SIZE      2048
#define WAIT_FOREVER (UInt)~(0)

unsigned long  m_ulStatus = 0;//SimpleLink Status
unsigned long  m_ulPingPacketsRecv = 0; //Number of Ping Packets received
unsigned long  m_ulGatewayIP = 0; //Network Gateway IP address
unsigned char  m_ucConnectionSSID[SSID_LEN_MAX+1]; //Connection SSID
unsigned char  m_ucConnectionBSSID[BSSID_LEN_MAX]; //Connection BSSID


unsigned long  m_ulDestinationIp = IP_ADDR;
unsigned int   m_uiTcpPortNum = TCP_PORT_NUM;
unsigned char  m_ucConnectionStatus = 0;
unsigned char  m_ucSimplelinkstarted = 0;
unsigned long  m_ulIpAddr = 0;
uint8_t m_SendBuf[BUF_SIZE];
//Tcp global
int             m_TcpSock;
int             m_TcpListenSock;
//Udp global
int             m_UdpSock;
SlSockAddrIn_t  m_sUdpAddr;



#if defined(gcc)
extern void (* const g_pfnVectors[])(void);
#endif
#if defined(ewarm)
extern uVectorEntry __vector_table;
#endif

//*****************************************************************************
//                 GLOBAL VARIABLES -- End
//*****************************************************************************


// Application specific status/error codes
typedef enum{
    // Choosing -0x7D0 to avoid overlap w/ host-driver's error codes
    SOCKET_CREATE_ERROR = -0x7D0,
    BIND_ERROR = SOCKET_CREATE_ERROR - 1,
    LISTEN_ERROR = BIND_ERROR -1,
    SOCKET_OPT_ERROR = LISTEN_ERROR -1,
    CONNECT_ERROR = SOCKET_OPT_ERROR -1,
    ACCEPT_ERROR = CONNECT_ERROR - 1,
    SEND_ERROR = ACCEPT_ERROR -1,
    RECV_ERROR = SEND_ERROR -1,
    SOCKET_CLOSE_ERROR = RECV_ERROR -1,
    DEVICE_NOT_IN_STATION_MODE = SOCKET_CLOSE_ERROR - 1,
    LAN_CONNECTION_FAILED = DEVICE_NOT_IN_STATION_MODE-1,
    INTERNET_CONNECTION_FAILED = LAN_CONNECTION_FAILED-1,
    STATUS_CODE_MAX = -0xBB8
}e_AppStatusCodes;
//*****************************************************************************
// SimpleLink Asynchronous Event Handlers -- Start
//*****************************************************************************


//*****************************************************************************
//
//! \brief The Function Handles WLAN Events
//!
//! \param[in]  pWlanEvent - Pointer to WLAN Event Info
//!
//! \return None
//!
//*****************************************************************************
void SimpleLinkWlanEventHandler(SlWlanEvent_t *pWlanEvent)
{
    switch(pWlanEvent->Event)
    {
        case SL_WLAN_CONNECT_EVENT:
        {
            SET_STATUS_BIT(m_ulStatus, STATUS_BIT_CONNECTION);

            //
            // Information about the connected AP (like name, MAC etc) will be
            // available in 'slWlanConnectAsyncResponse_t'-Applications
            // can use it if required
            //
            //  slWlanConnectAsyncResponse_t *pEventData = NULL;
            // pEventData = &pWlanEvent->EventData.STAandP2PModeWlanConnected;
            //
            
            // Copy new connection SSID and BSSID to global parameters
            memcpy(m_ucConnectionSSID,pWlanEvent->EventData.
                   STAandP2PModeWlanConnected.ssid_name,
                   pWlanEvent->EventData.STAandP2PModeWlanConnected.ssid_len);
            memcpy(m_ucConnectionBSSID,
                   pWlanEvent->EventData.STAandP2PModeWlanConnected.bssid,
                   SL_BSSID_LENGTH);

            printf("[WLAN EVENT] STA Connected to the AP: %s ,"
                        "BSSID: %x:%x:%x:%x:%x:%x\n\r",
                      m_ucConnectionSSID,m_ucConnectionBSSID[0],
                      m_ucConnectionBSSID[1],m_ucConnectionBSSID[2],
                      m_ucConnectionBSSID[3],m_ucConnectionBSSID[4],
                      m_ucConnectionBSSID[5]);
        }
        break;

        case SL_WLAN_DISCONNECT_EVENT:
        {
            slWlanConnectAsyncResponse_t*  pEventData = NULL;

            CLR_STATUS_BIT(m_ulStatus, STATUS_BIT_CONNECTION);
            CLR_STATUS_BIT(m_ulStatus, STATUS_BIT_IP_AQUIRED);

            pEventData = &pWlanEvent->EventData.STAandP2PModeDisconnected;

            // If the user has initiated 'Disconnect' request, 
            //'reason_code' is SL_USER_INITIATED_DISCONNECTION 
            if(SL_USER_INITIATED_DISCONNECTION == pEventData->reason_code)
            {
                printf("[WLAN EVENT]Device disconnected from the AP: %s,"
                "BSSID: %x:%x:%x:%x:%x:%x on application's request \n\r",
                           m_ucConnectionSSID,m_ucConnectionBSSID[0],
                           m_ucConnectionBSSID[1],m_ucConnectionBSSID[2],
                           m_ucConnectionBSSID[3],m_ucConnectionBSSID[4],
                           m_ucConnectionBSSID[5]);
            }
            else
            {
                printf("[WLAN ERROR]Device disconnected from the AP AP: %s,"
                "BSSID: %x:%x:%x:%x:%x:%x on an ERROR..!! \n\r",
                           m_ucConnectionSSID,m_ucConnectionBSSID[0],
                           m_ucConnectionBSSID[1],m_ucConnectionBSSID[2],
                           m_ucConnectionBSSID[3],m_ucConnectionBSSID[4],
                           m_ucConnectionBSSID[5]);
            }
            memset(m_ucConnectionSSID,0,sizeof(m_ucConnectionSSID));
            memset(m_ucConnectionBSSID,0,sizeof(m_ucConnectionBSSID));
        }
        break;

        default:
        {
            printf("[WLAN EVENT] Unexpected event [0x%x]\n\r",
                       pWlanEvent->Event);
        }
        break;
    }
}


//*****************************************************************************
//
//! \brief This function handles network events such as IP acquisition, IP
//!           leased, IP released etc.
//!
//! \param[in]  pNetAppEvent - Pointer to NetApp Event Info
//!
//! \return None
//!
//*****************************************************************************
void SimpleLinkNetAppEventHandler(SlNetAppEvent_t *pNetAppEvent)
{
    switch(pNetAppEvent->Event)
    {
        case SL_NETAPP_IPV4_IPACQUIRED_EVENT:
        {
            SlIpV4AcquiredAsync_t *pEventData = NULL;

            SET_STATUS_BIT(m_ulStatus, STATUS_BIT_IP_AQUIRED);
            
            //Ip Acquired Event Data
            pEventData = &pNetAppEvent->EventData.ipAcquiredV4;

            //Gateway IP address
            m_ulGatewayIP = pEventData->gateway;
            
            printf("[NETAPP EVENT] IP Acquired: IP=%d.%d.%d.%d , "
            "Gateway=%d.%d.%d.%d\n\r",
            SL_IPV4_BYTE(pNetAppEvent->EventData.ipAcquiredV4.ip,3),
            SL_IPV4_BYTE(pNetAppEvent->EventData.ipAcquiredV4.ip,2),
            SL_IPV4_BYTE(pNetAppEvent->EventData.ipAcquiredV4.ip,1),
            SL_IPV4_BYTE(pNetAppEvent->EventData.ipAcquiredV4.ip,0),
            SL_IPV4_BYTE(pNetAppEvent->EventData.ipAcquiredV4.gateway,3),
            SL_IPV4_BYTE(pNetAppEvent->EventData.ipAcquiredV4.gateway,2),
            SL_IPV4_BYTE(pNetAppEvent->EventData.ipAcquiredV4.gateway,1),
            SL_IPV4_BYTE(pNetAppEvent->EventData.ipAcquiredV4.gateway,0));
        }
        break;

        default:
        {
            printf("[NETAPP EVENT] Unexpected event [0x%x] \n\r",
                       pNetAppEvent->Event);
        }
        break;
    }
}


//*****************************************************************************
//
//! \brief This function handles HTTP server events
//!
//! \param[in]  pServerEvent - Contains the relevant event information
//! \param[in]    pServerResponse - Should be filled by the user with the
//!                                      relevant response information
//!
//! \return None
//!
//****************************************************************************
void SimpleLinkHttpServerCallback(SlHttpServerEvent_t *pHttpEvent,
                                  SlHttpServerResponse_t *pHttpResponse)
{
    // Unused in this application
}

//*****************************************************************************
//
//! \brief This function handles General Events
//!
//! \param[in]     pDevEvent - Pointer to General Event Info 
//!
//! \return None
//!
//*****************************************************************************
void SimpleLinkGeneralEventHandler(SlDeviceEvent_t *pDevEvent)
{
    //
    // Most of the general errors are not FATAL are are to be handled
    // appropriately by the application
    //
    printf("[GENERAL EVENT] - ID=[%d] Sender=[%d]\n\n",
               pDevEvent->EventData.deviceEvent.status, 
               pDevEvent->EventData.deviceEvent.sender);
}



//*****************************************************************************
//
//! This function handles socket events indication
//!
//! \param[in]      pSock - Pointer to Socket Event Info
//!
//! \return None
//!
//*****************************************************************************
void SimpleLinkSockEventHandler(SlSockEvent_t *pSock)
{
    //
    // This application doesn't work w/ socket - Events are not expected
    //
    switch( pSock->Event )
    {
        case SL_SOCKET_TX_FAILED_EVENT:
            switch( pSock->socketAsyncEvent.SockTxFailData.status)
            {
                case SL_ECLOSE: 
                    printf("[SOCK ERROR] - close socket (%d) operation "
                                "failed to transmit all queued packets\n\n", 
                                    pSock->socketAsyncEvent.SockTxFailData.sd);
                    break;
                default: 
                    printf("[SOCK ERROR] - TX FAILED  :  socket %d , reason "
                                "(%d) \n\n",
                                pSock->socketAsyncEvent.SockTxFailData.sd, pSock->socketAsyncEvent.SockTxFailData.status);
                  break;
            }
            break;

        default:
        	printf("[SOCK EVENT] - Unexpected Event [%x0x]\n\n",pSock->Event);
          break;
    }

}


//*****************************************************************************
//
//! \brief This function handles ping report events
//!
//! \param[in]     pPingReport - Ping report statistics
//!
//! \return None
//!
//*****************************************************************************
static void SimpleLinkPingReport(SlPingReport_t *pPingReport)
{
    SET_STATUS_BIT(m_ulStatus, STATUS_BIT_PING_DONE);
    m_ulPingPacketsRecv = pPingReport->PacketsReceived;
}

//*****************************************************************************
// SimpleLink Asynchronous Event Handlers -- End
//*****************************************************************************


//*****************************************************************************
//! \brief This function puts the device in its default state. It:
//!           - Set the mode to STATION
//!           - Configures connection policy to Auto and AutoSmartConfig
//!           - Deletes all the stored profiles
//!           - Enables DHCP
//!           - Disables Scan policy
//!           - Sets Tx power to maximum
//!           - Sets power policy to normal
//!           - Unregister mDNS services
//!           - Remove all filters
//!
//! \param   none
//! \return  On success, zero is returned. On error, negative is returned
//*****************************************************************************

static long ConfigureSimpleLinkToDefaultState()
{
    SlVersionFull   ver = {0};
    _WlanRxFilterOperationCommandBuff_t  RxFilterIdMask = {0};

    unsigned char ucVal = 1;
    unsigned char ucConfigOpt = 0;
    unsigned char ucConfigLen = 0;
    unsigned char ucPower = 0;

    long lRetVal = -1;
    long lMode = -1;

    lMode = sl_Start(0, 0, 0);
    ASSERT_ON_ERROR(lMode);

    // If the device is not in station-mode, try configuring it in station-mode 
    if (ROLE_STA != lMode)
    {
        if (ROLE_AP == lMode)
        {
            // If the device is in AP mode, we need to wait for this event 
            // before doing anything 
            while(!IS_IP_ACQUIRED(m_ulStatus))
            {
#ifndef SL_PLATFORM_MULTI_THREADED
              _SlNonOsMainLoopTask();
#endif
            }
        }

        // Switch to STA role and restart 
        lRetVal = sl_WlanSetMode(ROLE_STA);
        ASSERT_ON_ERROR(lRetVal);

        lRetVal = sl_Stop(0xFF);
        ASSERT_ON_ERROR(lRetVal);

        lRetVal = sl_Start(0, 0, 0);
        ASSERT_ON_ERROR(lRetVal);

        // Check if the device is in station again 
        if (ROLE_STA != lRetVal)
        {
            // We don't want to proceed if the device is not coming up in STA-mode 
            ASSERT_ON_ERROR(DEVICE_NOT_IN_STATION_MODE);
        }
    }
    
    // Get the device's version-information
    ucConfigOpt = SL_DEVICE_GENERAL_VERSION;
    ucConfigLen = sizeof(ver);
    lRetVal = sl_DevGet(SL_DEVICE_GENERAL_CONFIGURATION, &ucConfigOpt, 
                                &ucConfigLen, (unsigned char *)(&ver));
    ASSERT_ON_ERROR(lRetVal);
    
    printf("Host Driver Version: %s\n\r",SL_DRIVER_VERSION);
    printf("Build Version %d.%d.%d.%d.31.%d.%d.%d.%d.%d.%d.%d.%d\n\r",
    ver.NwpVersion[0],ver.NwpVersion[1],ver.NwpVersion[2],ver.NwpVersion[3],
    ver.ChipFwAndPhyVersion.FwVersion[0],ver.ChipFwAndPhyVersion.FwVersion[1],
    ver.ChipFwAndPhyVersion.FwVersion[2],ver.ChipFwAndPhyVersion.FwVersion[3],
    ver.ChipFwAndPhyVersion.PhyVersion[0],ver.ChipFwAndPhyVersion.PhyVersion[1],
    ver.ChipFwAndPhyVersion.PhyVersion[2],ver.ChipFwAndPhyVersion.PhyVersion[3]);

    // Set connection policy to Auto + SmartConfig 
    //      (Device's default connection policy)
    lRetVal = sl_WlanPolicySet(SL_POLICY_CONNECTION, 
                                SL_CONNECTION_POLICY(1, 0, 0, 0, 1), NULL, 0);
    ASSERT_ON_ERROR(lRetVal);

    // Remove all profiles
    lRetVal = sl_WlanProfileDel(0xFF);
    ASSERT_ON_ERROR(lRetVal);



    //
    // Device in station-mode. Disconnect previous connection if any
    // The function returns 0 if 'Disconnected done', negative number if already
    // disconnected Wait for 'disconnection' event if 0 is returned, Ignore 
    // other return-codes
    //
    lRetVal = sl_WlanDisconnect();
    if(0 == lRetVal)
    {
        // Wait
        while(IS_CONNECTED(m_ulStatus))
        {
#ifndef SL_PLATFORM_MULTI_THREADED
              _SlNonOsMainLoopTask(); 
#endif
        }
    }

    // Enable DHCP client
    lRetVal = sl_NetCfgSet(SL_IPV4_STA_P2P_CL_DHCP_ENABLE,1,1,&ucVal);
    ASSERT_ON_ERROR(lRetVal);

    // Disable scan
    ucConfigOpt = SL_SCAN_POLICY(0);
    lRetVal = sl_WlanPolicySet(SL_POLICY_SCAN , ucConfigOpt, NULL, 0);
    ASSERT_ON_ERROR(lRetVal);

    // Set Tx power level for station mode
    // Number between 0-15, as dB offset from max power - 0 will set max power
    ucPower = 0;
    lRetVal = sl_WlanSet(SL_WLAN_CFG_GENERAL_PARAM_ID, 
            WLAN_GENERAL_PARAM_OPT_STA_TX_POWER, 1, (unsigned char *)&ucPower);
    ASSERT_ON_ERROR(lRetVal);

    // Set PM policy to normal
    lRetVal = sl_WlanPolicySet(SL_POLICY_PM , SL_NORMAL_POLICY, NULL, 0);
    ASSERT_ON_ERROR(lRetVal);

    // Unregister mDNS services
    lRetVal = sl_NetAppMDNSUnRegisterService(0, 0);
    ASSERT_ON_ERROR(lRetVal);

    // Remove  all 64 filters (8*8)
    memset(RxFilterIdMask.FilterIdMask, 0xFF, 8);
    lRetVal = sl_WlanRxFilterSet(SL_REMOVE_RX_FILTER, (_u8 *)&RxFilterIdMask,
                       sizeof(_WlanRxFilterOperationCommandBuff_t));
    ASSERT_ON_ERROR(lRetVal);

    lRetVal = sl_Stop(SL_STOP_TIMEOUT);
    ASSERT_ON_ERROR(lRetVal);

    
    return lRetVal; // Success
}

//*****************************************************************************
//! \brief This function checks the LAN connection by pinging the AP's gateway
//!
//! \param  None
//!
//! \return 0 on success, negative error-code on error
//!
//*****************************************************************************
static long CheckLanConnection()
{
    SlPingStartCommand_t pingParams = {0};
    SlPingReport_t pingReport = {0};

    long lRetVal = -1;

    CLR_STATUS_BIT(m_ulStatus, STATUS_BIT_PING_DONE);
    m_ulPingPacketsRecv = 0;

    // Set the ping parameters
    pingParams.PingIntervalTime = PING_INTERVAL;
    pingParams.PingSize = PING_PKT_SIZE;
    pingParams.PingRequestTimeout = PING_TIMEOUT;
    pingParams.TotalNumberOfAttempts = NO_OF_ATTEMPTS;
    pingParams.Flags = 0;
    pingParams.Ip = m_ulGatewayIP;

    // Check for LAN connection
    lRetVal = sl_NetAppPingStart((SlPingStartCommand_t*)&pingParams, SL_AF_INET,
                            (SlPingReport_t*)&pingReport, SimpleLinkPingReport);
    ASSERT_ON_ERROR(lRetVal);

    // Wait for NetApp Event
    while(!IS_PING_DONE(m_ulStatus))
    {
#ifndef SL_PLATFORM_MULTI_THREADED
        _SlNonOsMainLoopTask();
#endif
    }

    if(0 == m_ulPingPacketsRecv)
    {
        //Problem with LAN connection
        ASSERT_ON_ERROR(LAN_CONNECTION_FAILED);
    }

    // LAN connection is successful
    return SUCCESS;
}

//****************************************************************************
//
//! \brief Connecting to a WLAN Accesspoint
//!
//!  This function connects to the required AP (SSID_NAME) with Security
//!  parameters specified in te form of macros at the top of this file
//!
//! \param  None
//!
//! \return  None
//!
//! \warning    If the WLAN connection fails or we don't aquire an IP
//!            address, It will be stuck in this function forever.
//
//****************************************************************************
static long WlanConnect()
{
    SlSecParams_t secParams = {0};
    long lRetVal = 0;

    secParams.Key = (signed char*)SECURITY_KEY;
    secParams.KeyLen = strlen(SECURITY_KEY);
    secParams.Type = SECURITY_TYPE;

    lRetVal = sl_WlanConnect((signed char*)SSID_NAME, strlen(SSID_NAME), 0, &secParams, 0);
    ASSERT_ON_ERROR(lRetVal);

    // Wait for WLAN Event
    while((!IS_CONNECTED(m_ulStatus)) || (!IS_IP_ACQUIRED(m_ulStatus)))
    {
        // Toggle LEDs to Indicate Connection Progress
        led_display(100,0,0);
        Delay_us(100000);
        led_display(0,100,100);
        Delay_us(100000);
    }

    return SUCCESS;

}

//****************************************************************************
//
//! \brief Start simplelink, connect to the ap and run the ping test
//!
//! This function starts the simplelink, connect to the ap and start the ping
//! test on the default gateway for the ap
//!
//! \param[in]  pvParameters - Pointer to the list of parameters that
//!             can bepassed to the task while creating it
//!
//! \return  None
//
//****************************************************************************
void WlanStationMode( void *pvParameters )
{

    long lRetVal = -1;

    //
    // Following function configure the device to default state by cleaning
    // the persistent settings stored in NVMEM (viz. connection profiles &
    // policies, power policy etc)
    //
    // Applications may choose to skip this step if the developer is sure
    // that the device is in its default state at start of applicaton
    //
    // Note that all profiles and persistent settings that were done on the
    // device will be lost
    //
   
    lRetVal = ConfigureSimpleLinkToDefaultState();
    if(lRetVal < 0)
    {
        if (DEVICE_NOT_IN_STATION_MODE == lRetVal)
        {
            printf("Failed to configure the device in its default state\n\r");
        }

        LOOP_FOREVER();
    }
 
    printf("Device is configured in default state \n\r");

    //
    // Assumption is that the device is configured in station mode already
    // and it is in its default state
    //
    
    
    lRetVal = sl_Start(0, 0, 0);
    if (lRetVal < 0 || ROLE_STA != lRetVal)
    {
        printf("Failed to start the device \n\r");
        LOOP_FOREVER();
    }

    printf("Device started as STATION \n\r");

    //
    //Connecting to WLAN AP
    //
    lRetVal = WlanConnect();
    if(lRetVal < 0)
    {
        printf("Failed to establish connection w/ an AP \n\r");
        LOOP_FOREVER();
    }

    printf("Connection established w/ AP and IP is aquired \n\r");
    
    // get profiles
    ReadProfiles();
    
    lRetVal = osi_TaskCreate( BsdUdpServer, \
                                (const signed char*)"UDP Sever Task", \
                                OSI_STACK_SIZE, (void*)UDP_PORT_NUM, 2, NULL );
    if(lRetVal < 0)
    {
        printf("%d",lRetVal);
        LOOP_FOREVER();
    }
    printf("UDP task has created \n\r");
    //
    // Start the TcpSever task
    //
    lRetVal = osi_TaskCreate( BsdTcpServer, \
                                (const signed char*)"TCP Sever Task", \
                                OSI_STACK_SIZE, NULL, 2, NULL );
    if(lRetVal < 0)
    {
        printf("%d",lRetVal);
        LOOP_FOREVER();
    }

    printf("TCP task has created \n\r");
    


}



//****************************************************************************
//
//! \brief Opening a UDP server side socket and receiving data
//!
//! This function opens a UDP socket in Listen mode and waits for an incoming
//! UDP connection.
//!    If a socket connection is established then the function will try to
//!    read 1000 UDP packets from the connected client.
//!
//! \param[in]          port number on which the server will be listening on
//!
//! \return             0 on success, Negative value on Error.
//
//****************************************************************************
void BsdUdpServer(void *pvParameters)
{

    SlSockAddrIn_t  sLocalAddr;
    int             iCounter;
    int             iAddrSize;

    int             iStatus; 
    long            lNonBlocking = 0; //Blocking mode
    Packet_Header_t     sPacketHeader;
    uint8_t             received_buffer[BUF_SIZE];

    // filling the buffer
    for (iCounter=0 ; iCounter<BUF_SIZE ; iCounter++)
    {
        received_buffer[iCounter] = (char)(iCounter % 10);
    }

    //filling the UDP server socket address
    sLocalAddr.sin_family = SL_AF_INET;
    sLocalAddr.sin_port = sl_Htons(UDP_PORT_NUM);
    sLocalAddr.sin_addr.s_addr = 0;

    iAddrSize = sizeof(SlSockAddrIn_t);

    // creating a UDP socket
    m_UdpSock = sl_Socket(SL_AF_INET,SL_SOCK_DGRAM, 0);
    if( m_UdpSock < 0 )
    {
        // error
        //ASSERT_ON_ERROR(SOCKET_CREATE_ERROR);
    	printf("SOCKET_CREATE_ERROR");
    }

    // binding the UDP socket to the UDP server address
    iStatus = sl_Bind(m_UdpSock, (SlSockAddr_t *)&sLocalAddr, iAddrSize);
    if( iStatus < 0 )
    {
        // error
        sl_Close(m_UdpSock);
        //ASSERT_ON_ERROR(BIND_ERROR);
        printf("BIND_ERROR.\r\n");
    }

    // setting socket option to make the socket as blocking
    iStatus = sl_SetSockOpt(m_UdpSock, SL_SOL_SOCKET, SL_SO_NONBLOCKING,
                            &lNonBlocking, sizeof(lNonBlocking));
    
    // no listen or accept is required as UDP is connectionless protocol
    /// waits for 1000 packets from a UDP client
    
                
    //red led on
    led_display(254,235,254);
  
    while (1)
    {
        iStatus = sl_RecvFrom(m_UdpSock, &sPacketHeader, PACKET_HEADER_LENGTH, 0,
                     ( SlSockAddr_t *)&m_sUdpAddr, (SlSocklen_t*)&iAddrSize );
        if( iStatus <= 0 )
        {
                // error
                sl_Close(m_UdpSock);
                //ASSERT_ON_ERROR(RECV_ERROR);
                printf("RECV_ERROR.\r\n");
        }
        else if( iStatus == PACKET_HEADER_LENGTH && sPacketHeader.version == PROTOCAL_VERSION)
        {
                printf("UDP packet receive: \r\n");
                if(sPacketHeader.length > 0)
                {
                    iStatus = sl_RecvFrom(m_UdpSock, received_buffer, sPacketHeader.length, 0,
                                ( SlSockAddr_t *)&m_sUdpAddr, (SlSocklen_t*)&iAddrSize );
                }
                ParsePacket(received_buffer, sPacketHeader.length, sPacketHeader.key);
        }
        else
        {
          printf("Packet header error!\r\n");
        }
    }

}

//****************************************************************************
//
//! \brief Opening a TCP server side socket and receiving data
//!
//! This function opens a TCP socket in Listen mode and waits for an incoming
//!    TCP connection.
//! If a socket connection is established then the function will try to read
//!    1000 TCP packets from the connected client.
//!
//! \param[in] port number on which the server will be listening on
//!
//! \return     0 on success, -1 on error.
//!
//! \note   This function will wait for an incoming connection till
//!                     one is established
//
//****************************************************************************
void BsdTcpServer(void *pvParameters)
{

    SlSockAddrIn_t  sAddr;
    SlSockAddrIn_t  sLocalAddr;
    int             iCounter;
    int             iAddrSize;

    int             iStatus;

    long            lNonBlocking = 0; //Blocking mode

    Packet_Header_t     sPacketHeader;
    uint8_t             received_buffer[BUF_SIZE];
    //delete the WlanStation task
//    osi_TaskDelete(&TCPtaskHandle);

    // filling the buffer
    for (iCounter=0 ; iCounter<BUF_SIZE ; iCounter++)
    {
        received_buffer[iCounter] = (char)(iCounter % 10);
    }


    //filling the TCP server socket address
    sLocalAddr.sin_family = SL_AF_INET;
    sLocalAddr.sin_port = sl_Htons(TCP_PORT_NUM);
    sLocalAddr.sin_addr.s_addr = 0;

    // creating a TCP socket
    m_TcpListenSock = sl_Socket(SL_AF_INET,SL_SOCK_STREAM, 0);
    if( m_TcpListenSock < 0 )
    {
        // error
    	printf("SOCKET_CREATE_ERROR.\r\n");
    }

    iAddrSize = sizeof(SlSockAddrIn_t);

    // binding the TCP socket to the TCP server address
    iStatus = sl_Bind(m_TcpListenSock, (SlSockAddr_t *)&sLocalAddr, iAddrSize);
    if( iStatus < 0 )
    {
        // error
        sl_Close(m_TcpListenSock);
        printf("BIND_ERROR.\r\n");
    }

    // putting the socket for listening to the incoming TCP connection
    iStatus = sl_Listen(m_TcpListenSock, 0);
    if( iStatus < 0 )
    {
        sl_Close(m_TcpListenSock);
        printf("LISTEN_ERROR.\r\n");
    }

    // setting socket option to make the socket as blocking
    iStatus = sl_SetSockOpt(m_TcpListenSock, SL_SOL_SOCKET, SL_SO_NONBLOCKING,
                            &lNonBlocking, sizeof(lNonBlocking));
    if( iStatus < 0 )
    {
        sl_Close(m_TcpListenSock);
        printf("SOCKET_OPT_ERROR.\r\n");
    }
    m_TcpSock = SL_EAGAIN;

    // waiting for an incoming TCP connection
    printf("waiting for an incoming TCP connection.\n\r");
    while( m_TcpSock < 0 )
    {
        // accepts a connection form a TCP client, if there is any
        // otherwise returns SL_EAGAIN
        m_TcpSock = sl_Accept(m_TcpListenSock, ( struct SlSockAddr_t *)&sAddr,
                                (SlSocklen_t*)&iAddrSize);
        if( m_TcpSock == SL_EAGAIN )
        {
           Delay_us(750);
        }
        else if( m_TcpSock < 0 )
        {
            // error
            sl_Close(m_TcpSock);
            sl_Close(m_TcpListenSock);
            printf("ACCEPT_ERROR.\n\r");
        }
    }
    printf("connect succeed, sockedID is %d.\n\r",m_TcpSock);

    while (1)
    {
        iStatus = sl_Recv(m_TcpSock, &sPacketHeader, PACKET_HEADER_LENGTH, 1);
        if( iStatus < 0 && iStatus != SL_EAGAIN)
        {
          // error
          sl_Close(m_TcpSock);
          sl_Close(m_TcpListenSock);
          printf("RECV_ERROR.\r\n");
        }
        else if( iStatus == PACKET_HEADER_LENGTH && sPacketHeader.version == PROTOCAL_VERSION)
        {
            printf("TCP packet receive: \r\n");

            if(sPacketHeader.length != 0)
                iStatus = sl_Recv(m_TcpSock, received_buffer, sPacketHeader.length, 0);

            ParsePacket(received_buffer, sPacketHeader.length, sPacketHeader.key);
        }
        else
        {
          printf("Packet header error!\r\n");
        }
    }

    printf("Close the Socket\n\r");

    // close the connected socket after receiving from connected TCP client
    iStatus = sl_Close(m_TcpSock);
    printf("%d.\r\n", iStatus);
    // close the listening socket
    iStatus = sl_Close(m_TcpListenSock);
    printf("%d.\r\n", iStatus);
}

//****************************************************************************
//
//! \brief	This send message by udp.
//!
//!
//! \param[in] packet is the point to the data packet head.
//!
//! \param[in] len is the length of the data packet.
//!
//! \return     1 on success, error code on error.
//!

//
//****************************************************************************
int SendMsgByUdp(uint8_t* content, uint16_t length, uint8_t key)
{
    int iStatus;
    Packet_Header_t     sPacketHeader;
    //fill Header
    sPacketHeader.version = PROTOCAL_VERSION;
    sPacketHeader.key = key;
    sPacketHeader.length = length;

    //fill send buffer
    memcpy(m_SendBuf, &sPacketHeader, PACKET_HEADER_LENGTH);
    memcpy(m_SendBuf + PACKET_HEADER_LENGTH, content, length);
    
    iStatus = sl_SendTo(m_UdpSock, m_SendBuf, sPacketHeader.length+PACKET_HEADER_LENGTH, 0,
                        (SlSockAddr_t *)&m_sUdpAddr, sizeof(SlSockAddrIn_t));
    if( iStatus <= 0 )
    {
            // error
            sl_Close(m_UdpSock);
            ASSERT_ON_ERROR(SEND_ERROR);
    }
    return 0;
}

//****************************************************************************
//
//! \brief	This send message by TCP.
//!
//!
//! \param[in] packet is the point to the data packet head.
//!
//! \param[in] len is the length of the data packet.
//!
//! \return     1 on success, error code on error.
//!

//
//****************************************************************************
int SendMsgByTcp(const uint8_t* content, uint16_t len, uint8_t key)
{
    // need add thread lock
    int iStatus;
    Packet_Header_t     sPacketHeader;
    //fill Header
    sPacketHeader.version = PROTOCAL_VERSION;
    sPacketHeader.length = len + PACKET_HEADER_LENGTH;
    sPacketHeader.key = key;
    
    if(len == 0)       //payload is void
    {
        iStatus = sl_Send(m_TcpSock, &sPacketHeader, PACKET_HEADER_LENGTH, 0);
    }
    else
    {
        //fill send buffer
        memcpy(m_SendBuf, &sPacketHeader, PACKET_HEADER_LENGTH);
        memcpy(m_SendBuf + PACKET_HEADER_LENGTH, content, len);
        
        iStatus = sl_Send(m_TcpSock, m_SendBuf, sPacketHeader.length, 0);
    }
    
    if( iStatus <= 0 )
    {
            // error
            sl_Close(m_TcpSock);
            ASSERT_ON_ERROR(SEND_ERROR);
    }
    return 0;
}
