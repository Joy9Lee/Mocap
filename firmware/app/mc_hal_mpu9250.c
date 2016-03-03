 /* 
 * File Name          : cc_hal_ads129x.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 9/12/2015         | Initial Revision                | 
 
 */

// Driverlib includes
#include "hw_types.h"
#include "hw_memmap.h"
//#include "hw_common_reg.h"
#include "spi.h"
#include "gpio.h"
#include "prcm.h"
//#include "rom.h"
#include "rom_map.h"
#include "utils.h"
//#include "uart.h"
//#include "interrupt.h"

#include "cc_pinmux.h"
#include "cc_utils.h"
#include "cc_hal_gpio.h"
#include "cc_hal_spi.h"
#include "cc_hal_timer.h"
#include "mc_protocol.h"
#include "mc_hal_mpu9250.h"
#include "mc_profiles.h"

//REG ADDRESS
#define	INT_PIN_CFG		0x37
#define	PWR_MGMT_1		0x6B
#define	PWR_MGMT_2		0x6C
#define	ACCEL_CONFIG    	0x1C
#define	GYRO_CONFIG		0x1B
#define	CONFIG			0x1A
#define	SMPRT_DIV		0x19
#define	CNTL			0x0A

#define ACC_REG			0x3B
#define	GYO_REG			0x43
#define MEG_REG			0x03

#define WHO_AM_I                0x75
//GPIO PIN
#define CS_GPIO_NUM	17
#define INT_GPIO_NUM 	22

#define MAG_READ_DELAY  256

static uint8_t tmpRead[3];
static uint8_t AK8963_ASA[3];

static Gpio_Type m_GpioCs;
static Gpio_Type m_GpioInt;

static void MPU_INT_Handler();
static uint8_t Mpu_RdREG(uint8_t reg_address);
static uint8_t Mpu_WtREG(uint8_t reg_address, uint8_t data);
static void Mpu_RdDATA(uint8_t reg_address, uint8_t *data);
static void MPU_Timer_Init(void);

static uint8_t MPU9250_Check( void );
static void MPU9250_Mag_WriteReg( uint8_t writeAddr, uint8_t writeData );
static void MPU9250_Mag_ReadRegs( uint8_t readAddr, uint8_t *readData, uint8_t lens );

void MPU_Start();

void Mpu9250_Init()
{
    GPIO_IF_GetPortNPin(INT_GPIO_NUM, &m_GpioInt);
    GPIO_IF_GetPortNPin(CS_GPIO_NUM, &m_GpioCs);
    
    mpu9250_pinmux();
    GPIO_IF_INTCLR(m_GpioInt);
    //GPIO_IF_INTINI(m_GpioInt,GPIO_FALLING_EDGE, MPU_INT_Handler);
    Spi_Init();
    

        
    
    int ret = 1;
    //ret &= I2C_IF_Write(0x68,&a[0],0x02,0x01);

    ret = Mpu_RdREG(WHO_AM_I);
    ret = Mpu_WtREG(INT_PIN_CFG, 0x02);
    ret = Mpu_WtREG(PWR_MGMT_1, 0x01);	//PLL with X axis gyroscope reference.
    ret = Mpu_WtREG(PWR_MGMT_2, 0x00);
    ret = Mpu_WtREG(ACCEL_CONFIG, 0x18);	//+-16g scale
    ret = Mpu_WtREG(GYRO_CONFIG, 0x18);	//+-2000 scale
    ret = Mpu_WtREG(CONFIG, 0x00);
    //Sample Rate = Gyroscope Output Rate / (1 + SMPLRT_DIV),
    //where Gyroscope Output Rate = 8kHz when the DLPF is disabled (DLPF_CFG = 0 or 7),
    //and 1kHz when the DLPF is enabled
    ret = Mpu_WtREG(SMPRT_DIV, 0x00);
    //ret &= MPU_Reg_Write(AK8975, CNTL, 0x0F);		//Fuse ROM access mode
    ret = Mpu_WtREG(CNTL, 0x01);		//Single measurement mode
    
    
  /*
    //Mpu_WtREG(MPU6500_PWR_MGMT_1, 0x80);        // [0]  Reset Device
    //Delay_us(10000);
    Mpu_WtREG(MPU6500_PWR_MGMT_1, 0x04);        // [1]  Clock Source
    Mpu_WtREG(MPU6500_INT_PIN_CFG, 0x10);       // [2]  Set INT_ANYRD_2CLEAR
    Mpu_WtREG(MPU6500_INT_ENABLE, 0x01);        // [3]  Set RAW_RDY_EN
    Mpu_WtREG(MPU6500_PWR_MGMT_2, 0x00);        // [4]  Enable Acc & Gyro
    Mpu_WtREG(MPU6500_SMPLRT_DIV, 0x00);        // [5]  Sample Rate Divider
    Mpu_WtREG(MPU6500_GYRO_CONFIG, 0x18);       // [6]  default : +-2000dps
    //Mpu_WtREG(MPU6500_ACCEL_CONFIG, 0x08);      // [7]  default : +-4G
    Mpu_WtREG(MPU6500_ACCEL_CONFIG, 0x18);      // [7]  default : +-16G
    Mpu_WtREG(MPU6500_CONFIG, 0x07);            // [8]  default : LPS_41Hz
    Mpu_WtREG(MPU6500_ACCEL_CONFIG_2, 0x03);    // [9]  default : LPS_41Hz
    Mpu_WtREG(MPU6500_USER_CTRL, 0x30);         // [10] Set I2C_MST_EN, I2C_IF_DIS
   */ 
    MPU9250_Check();
        
    MPU9250_Mag_WriteReg(AK8963_CNTL2, 0x01);       // Reset Device
    MPU9250_Mag_WriteReg(AK8963_CNTL1, 0x10);       // Power-down mode
    MPU9250_Mag_WriteReg(AK8963_CNTL1, 0x1F);       // Fuse ROM access mode
   // MPU9250_Mag_ReadRegs(AK8963_ASAX, tmpRead, 3);  // Read sensitivity adjustment values
    MPU9250_Mag_WriteReg(AK8963_CNTL1, 0x10);       // Power-down mode
    
  	AK8963_ASA[0] = (int16_t)(tmpRead[0]) + 128;
	AK8963_ASA[1] = (int16_t)(tmpRead[1]) + 128;
	AK8963_ASA[2] = (int16_t)(tmpRead[2]) + 128;

    Mpu_WtREG(MPU6500_I2C_MST_CTRL, 0x5D);
    Mpu_WtREG(MPU6500_I2C_SLV0_ADDR, AK8963_I2C_ADDR | 0x80);
    Mpu_WtREG(MPU6500_I2C_SLV0_REG, AK8963_ST1);
    Mpu_WtREG(MPU6500_I2C_SLV0_CTRL, MPU6500_I2C_SLVx_EN | 8);

    MPU9250_Mag_WriteReg(AK8963_CNTL1, 0x16);       // Continuous measurement mode 2

	Mpu_WtREG(MPU6500_I2C_SLV4_CTRL, 0x09);
	Mpu_WtREG(MPU6500_I2C_MST_DELAY_CTRL, 0x81);
        
        
    MPU_Timer_Init();
}

static uint8_t Mpu_RdREG(uint8_t reg_address)
{
    uint8_t result;
    GPIO_IF_Set(m_GpioCs, 0);
    SPI_WByte(0x80|reg_address);//the same as upside
    result = SPI_RByte();//
    GPIO_IF_Set(m_GpioCs, 1);
    return result;
}

static void Mpu_RdDATA(uint8_t reg_address, uint8_t *data)
{
    uint8_t  i;
    GPIO_IF_Set(m_GpioCs, 0);
    SPI_WByte(0x80|reg_address);//the same as upside
    for( i = 0; i < 6; i++)
    {
        *data = SPI_RByte();//
        data++;
    }
    GPIO_IF_Set(m_GpioCs, 1);
}

static uint8_t Mpu_WtREG(uint8_t reg_address, uint8_t data)
{
    uint8_t rdback;
    GPIO_IF_Set(m_GpioCs, 0);
    SPI_WByte(reg_address);
    SPI_WByte(data);//
    GPIO_IF_Set(m_GpioCs, 1);
  //read back
    rdback = Mpu_RdREG(reg_address);
    if(data == rdback)
        return 1;
    else
    {
        printf("mpu read reg error \r\n");
    return 0;
    }
}

static void MPU9250_Mag_WriteReg( uint8_t writeAddr, uint8_t writeData )
{
    uint8_t  status = 0;
    uint32_t timeout = MAG_READ_DELAY;

    Mpu_WtREG(MPU6500_I2C_SLV4_ADDR, AK8963_I2C_ADDR);
    Mpu_WtREG(MPU6500_I2C_SLV4_REG, writeAddr);
    Mpu_WtREG(MPU6500_I2C_SLV4_DO, writeData);
    Mpu_WtREG(MPU6500_I2C_SLV4_CTRL, MPU6500_I2C_SLVx_EN);
    do {
        status = Mpu_RdREG(MPU6500_I2C_MST_STATUS);
        Delay_us(100);
    } while(((status & MPU6500_I2C_SLV4_DONE) == 0) && (timeout--));
}

static void MPU9250_Mag_WriteRegs( uint8_t writeAddr, uint8_t *writeData, uint8_t lens )
{
    uint8_t  status = 0;
    uint32_t timeout = MAG_READ_DELAY;

    MPU9250_WriteReg(MPU6500_I2C_SLV4_ADDR, AK8963_I2C_ADDR);
    for(uint8_t i = 0; i < lens; i++) {
        Mpu_WtREG(MPU6500_I2C_SLV4_REG, writeAddr + i);
        Mpu_WtREG(MPU6500_I2C_SLV4_DO, writeData[i]);
        Mpu_WtREG(MPU6500_I2C_SLV4_CTRL, MPU6500_I2C_SLVx_EN);
        do {
            status = Mpu_RdREG(MPU6500_I2C_MST_STATUS);
        } while(((status & MPU6500_I2C_SLV4_DONE) == 0) && (timeout--));
    }
}

static uint8_t MPU9250_Mag_ReadReg( uint8_t readAddr )
{
    uint8_t status = 0;
    uint8_t readData = 0;
    uint32_t timeout = MAG_READ_DELAY;

    Mpu_WtREG(MPU6500_I2C_SLV4_ADDR, AK8963_I2C_ADDR | 0x80);
    Mpu_WtREG(MPU6500_I2C_SLV4_REG, readAddr);
    Mpu_WtREG(MPU6500_I2C_SLV4_CTRL, MPU6500_I2C_SLVx_EN);

    do {
        status = Mpu_RdREG(MPU6500_I2C_MST_STATUS);
        Delay_us(100);
    } while(((status & MPU6500_I2C_SLV4_DONE) == 0) && (timeout--));

    readData = Mpu_RdREG(MPU6500_I2C_SLV4_DI);

    return readData;
}

static void MPU9250_Mag_ReadRegs( uint8_t readAddr, uint8_t *readData, uint8_t lens )
{
    uint8_t status = 0;
    uint32_t timeout = MAG_READ_DELAY;

    Mpu_WtREG(MPU6500_I2C_SLV4_ADDR, AK8963_I2C_ADDR | 0x80);
    for(uint8_t i = 0; i< lens; i++) {
        Mpu_WtREG(MPU6500_I2C_SLV4_REG, readAddr + i);
        Mpu_WtREG(MPU6500_I2C_SLV4_CTRL, MPU6500_I2C_SLVx_EN);

        do {
            status = Mpu_RdREG(MPU6500_I2C_MST_STATUS);
        } while(((status & MPU6500_I2C_SLV4_DONE) == 0) && (timeout--));
    
        readData[i] = Mpu_RdREG(MPU6500_I2C_SLV4_DI);
    }
}

static uint8_t MPU9250_Check( void )
{
    uint8_t deviceID = 0x00;

    deviceID = Mpu_RdREG(MPU6500_WHO_AM_I);
    if(deviceID != MPU6500_Device_ID)
    {
        printf("MPU6500 ID error! \r\n");
        return 0;
    }
     
    deviceID = MPU9250_Mag_ReadReg(AK8963_WIA);
    if(deviceID != AK8963_Device_ID)
    {
        printf("AK8963 ID error! \r\n");
        return 0;
    }
  
    return 1;
}

//*****************************************************************************
//
//! \brief  Timer Initialization & Configuration
//!
//! \param  None
//!
//! \return None
//
//*****************************************************************************
static void MPU_Timer_Init(void)
{
    //
    // Configuring the timers
    //
    MAP_PRCMPeripheralClkEnable(PRCM_TIMERA1, PRCM_RUN_MODE_CLK);
    Timer_IF_Init(PRCM_TIMERA1, TIMERA1_BASE, TIMER_CFG_PERIODIC, TIMER_A, 0);

    //
    // Setup the interrupts for the timer timeouts.
    //
    Timer_IF_IntSetup(TIMERA1_BASE, TIMER_A, MPU_INT_Handler);
    //Initial clocker, the period is n ms.  for time-record
    //Timer_IF_Start(TIMERA1_BASE, TIMER_A, 10);
    

}

void MPU_Start()
{
    int lRetVal = -1;

    //
    // Turn on the timers feeding values in mSec
    //
    MotionQueueInit();

//
// Start the data sending task
//
    //if(g_profiles.MOTION_EN != 0)
    {
        MotionSendTaskCreat();
        //start collection
        //Timer_IF_Start(TIMERA1_BASE, TIMER_A, 1000/g_profiles.MOTION_RATE); //n ms
        Timer_IF_Start(TIMERA1_BASE, TIMER_A, 10);
    }
/*
    if(g_profiles.EMG_CHL != 0x00)
    {
        lRetVal = osi_TaskCreate( EmgWifiSend, \
                                    (const signed char*)"Emg Wifi Send Task", \
                                    OSI_STACK_SIZE, NULL, 1, &EmgTaskHandle );
        if(lRetVal < 0)
        {
            printf("%d",lRetVal);
            while(1);
        }
        ADS_start_rdatac_mode();
        FE_START_SET(1);
    }
*/
    printf("Start Collection.\r\n");

}

void MPU_Stop()
{
    Timer_IF_Stop(TIMERA1_BASE, TIMER_A);
}
static void MPU_INT_Handler()
{
    Motion_Data_Struct MotionData;
    //Clocker();
    //
    // Clear the timer interrupt.
    //
    Timer_IF_InterruptClear(TIMERA1_BASE);
    Mpu_RdDATA(ACC_REG, (unsigned char *)&MotionData.Acc);
  //  Mpu_RdDATA(MEG_REG, (unsigned char *)&MotionData.Meg);
    Mpu_RdDATA(GYO_REG, (unsigned char *)&MotionData.Gyo);
    //Mpu_WtREG(CNTL, 0x01);
    //MotionQueueSend(&MotionData);
    MotionQueueWrite((uint8_t *)&(MotionData));
    
}