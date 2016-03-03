 /* 
 * File Name          : cc_hal_ads129x.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 7/11/2015         | Initial Revision                | 
 
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

#include "cc_utils.h"
#include "cc_hal_gpio.h"
#include "mc_hal_ads129x.h"

#define MASTER_MODE      0
#define SPI_IF_BIT_RATE  100000
#define My_SPI_IF_BIT_RATE  20000000 //2 divide  CLKD 1H

#define Dummy_Byte	 0x5a

#define ADS_REG_ID   0x00
#define ADS_REG_CFG1 0x01
#define ADS_REG_CFG2 0x02
#define ADS_REG_CFG3 0x03
#define ADS_REG_CFG4 0x17

#define ADS_REG_CH1SET  0x05
#define ADS_REG_CH2SET  0x06
#define ADS_REG_CH3SET  0x07
#define ADS_REG_CH4SET  0x08
#define ADS_REG_CH5SET  0x09
#define ADS_REG_CH6SET  0x0a
#define ADS_REG_CH7SET  0x0b
#define ADS_REG_CH8SET  0x0c

#define ADS_REG_PACE    0x15
#define ADS_REG_RESP    0x16
#define ADS_REG_CONFIG4 0x17
#define ADS_REG_CONFIG2 0x02

#define ADS_REG_RLD_SENSP 0x0D
#define ADS_REG_RLD_SENSN 0x0E

typedef struct _Emg_Data_Struct
{
	uint8_t ch1[3];	//ch1
	uint8_t ch2[3];
	uint8_t ch3[3];
	uint8_t ch4[3];

}Emg_Data_Struct;

Emg_Data_Struct EmgData = {0x00,0x00,0x00,0x11,0x11,0x11,0x22,0x22,0x22,0x33,0x33,0x33};

#define START_GPIO_NUM	17
#define CS_GPIO_NUM 	3
#define RESET_GPIO_NUM  5
#define PWDN_GPIO_NUM	6
#define DRDY_GPIO_NUM   7

static Gpio_Type m_GpioStart;
static Gpio_Type m_GpioCs;
static Gpio_Type m_GpioReset;
static Gpio_Type m_GpioPwdn;
static Gpio_Type m_GpioDrdy;

static void ADS129x_Spi_Init();
static unsigned char ADS_SPI_RByte();
static void ADS_SPI_WByte(unsigned char byte);
static unsigned char ADS_SPI_RREG(unsigned char reg_address );
static int ADS_SPI_WREG(unsigned char reg_address, unsigned char data);

void ADS129x_Init()
{
    GPIO_IF_GetPortNPin(START_GPIO_NUM, &m_GpioStart);
    GPIO_IF_GetPortNPin(CS_GPIO_NUM, &m_GpioCs);
    GPIO_IF_GetPortNPin(RESET_GPIO_NUM, &m_GpioReset);
    GPIO_IF_GetPortNPin(PWDN_GPIO_NUM, &m_GpioPwdn);
    GPIO_IF_GetPortNPin(DRDY_GPIO_NUM, &m_GpioDrdy);
    
    Ads129x_Pinmux();
    GPIO_IF_INTCLR(m_GpioDrdy);
    GPIO_IF_INTINI(m_GpioDrdy,GPIO_FALLING_EDGE, EMG_INT_Handler);
    
    ADS129x_Spi_Init();
}
//****************************************************************************
//
//! \brief Spi Initail
//!
//! This Function Initial the Spi interface, spi rate 2 divide CLKD, Master Mode,
//! 3 Pin Mode, CS active_LOW
//!
//! \param[in]  None
//!
//! \return  None
//
//****************************************************************************
static void ADS129x_Spi_Init()
{
    //
    // Reset SPI
    //
    MAP_SPIReset(GSPI_BASE);

    //
    // Configure SPI interface
    //
    MAP_SPIConfigSetExpClk(GSPI_BASE,MAP_PRCMPeripheralClockGet(PRCM_GSPI),
    				My_SPI_IF_BIT_RATE,SPI_MODE_MASTER,SPI_SUB_MODE_1,
                     (SPI_SW_CTRL_CS |
                     SPI_3PIN_MODE |
                     SPI_TURBO_OFF |
                     SPI_CS_ACTIVELOW |
                     SPI_WL_8));


    //
    // Enable SPI for communication
    //
    MAP_SPIEnable(GSPI_BASE);

   // SPICSEnable(GSPI_BASE);
}

//****************************************************************************
//
//! \brief Write a byte to ads129x by spi
//!
//! This function use to write a byte to ads129x
//!
//!
//! \param[in]  byte is a byte to write
//!
//! \return  None
//
//****************************************************************************
static void ADS_SPI_WByte(unsigned char byte)
{
    //SPIDataPut(GSPI_BASE, byte);
    MAP_SPITransfer(GSPI_BASE,&byte,0,1,
                    SPI_CS_ENABLE|SPI_CS_DISABLE);
}

//****************************************************************************
//
//! \brief Read a byte to ads129x by spi
//!
//! This function use to read a byte to ads129x
//!
//!
//! \param[in]
//!
//! \return  a byte
//
//****************************************************************************
static unsigned char ADS_SPI_RByte()
{
    unsigned char byte;
    MAP_SPITransfer(GSPI_BASE,0,&byte,1,
                    SPI_CS_ENABLE|SPI_CS_DISABLE);
    return byte;
}

//****************************************************************************
//
//! \brief Read a a regidster from ads129x by spi.
//!
//! This function use to read a register to ads129x.
//!
//!
//! \param[in] reg_address is the register address.
//!
//! \return  a byte in register.
//
//****************************************************************************
static unsigned char ADS_SPI_RREG(unsigned char reg_address )
{
    uint8_t result;
    //unsigned int RREG_OPCODE1;
    //unsigned int RREG_OPCODE2;
    //#define number_of_bytes 1;
    //send two opcodes of RREG
    /*
    RREG_OPCODE1 = 0x20 + reg_address;//opcode1:0010,rrrr;rrrr is the starting reg addr.
    ADS_SPI_Write_Byte(RREG_OPCODE1);// define RDATAC 0x10;
    */
    ADS_SPI_WByte(0x20 + reg_address);//the same as upside
    /*
    RREG_OPCODE2 = number_of_bytes-1;//opcode2:000n,nnnn;n,nnnn is the number of bytes to read -1
    ADS_SPI_Write_Byte(RREG_OPCODE2);// define RDATAC 0x10;
    */
    ADS_SPI_WByte(0x00);

    result = ADS_SPI_RByte();//
    return result;
}
//****************************************************************************
//
//! \brief Write a byte to regidster in ads129x by spi.
//!
//! This function use to write a register to ads129x.
//!
//!
//! \param[in] reg_address is the register address.
//!
//! \param[in] data is a byte to write.
//!
//! \return  1 is write success, 0 is error
//
//****************************************************************************
static int ADS_SPI_WREG(unsigned char reg_address, unsigned char data)
{
  unsigned char WREG_OPCODE1;
  unsigned char WREG_OPCODE2;
  unsigned char rdback;
  //#define number_of_bytes 1;//default, read/write only one register
  //send two opcodes of WREG
  WREG_OPCODE1=0x40+reg_address;//opcode1:0100,rrrr;rrrr is the starting reg addr.
  ADS_SPI_WByte(WREG_OPCODE1);
  WREG_OPCODE2=0x00;//opcode2:000n,nnnn;n,nnnn is the number of bytes to read -1
  ADS_SPI_WByte(WREG_OPCODE2);

  ADS_SPI_WByte(data);//
  //read back
  rdback = ADS_SPI_RREG(reg_address);
   if(data == rdback)
     return 1;
   else
     return 0;
}
//****************************************************************************
//
//! \brief Read the ADS129x ID
//!
//! This function use to confirm the ads129x is worked.
//!
//! \param[in] None
//!
//! \return  the ADS129x ID
//!				- ADS 1294R 0xD0
//!				- ADS 1294	0x90
//!				- ADS 1296R 0xD1
//!				- ADS 1296  0x91
//!				- ADS 1298R 0xD2
//!				- ADS 1298  0x92
//
//****************************************************************************

unsigned char ADS_read_id()
{
	unsigned char DeviceID;
 	GPIO_IF_Set(m_GpioCs, 1);;
 	DeviceID = 0;
 	GPIO_IF_Set(m_GpioCs, 0);;
 	DeviceID = ADS_SPI_RREG(ADS_REG_ID);
	GPIO_IF_Set(m_GpioCs, 1);;
	switch(DeviceID)
	{
	case 0xD0:
		printf("The Device is 1294R.\n\r");
		return DeviceID;;
	case 0x90:
		printf("The Device is 1294.\n\r");
		return DeviceID;;
	case 0xD1:
		printf("The Device is 1296R.\n\r");
		return DeviceID;;
	case 0x91:
		printf("The Device is 1296.\n\r");
		return DeviceID;;
	case 0xD2:
		printf("The Device is 1298R.\n\r");
		return DeviceID;
	case 0x92:
		printf("The Device is 1286.\n\r");
		return DeviceID;
	default:
		return -1;
	}

}

//****************************************************************************
//
//! \brief Stop the read data continue mode
//!
//! This function use to stop to read.
//!
//! \param[in] None
//!
//! \return  None
//
//****************************************************************************

void ADS_stop_rdatac_mode()
{
    //SDATAC COMMAND, first byte opcode, 0001,0001(11h)
    GPIO_IF_Set(m_GpioCs, 0);;
    ADS_SPI_WByte(0x11);
    //wait for at least 4 clk
    Delay_us(2);	//tCLK = 0.5us.
    GPIO_IF_Set(m_GpioCs, 1);;
}

//****************************************************************************
//
//! \brief Start the read data continue mode
//!
//! This function use to Start to read mode.
//!
//! \param[in] None
//!
//! \return  None
//
//****************************************************************************

void ADS_start_rdatac_mode()
{
    int i;
    GPIO_IF_Set(m_GpioCs, 0);;
    ADS_SPI_WByte(0x10);
    //wait for at least 4 clk
    for (i=0; i<10; i++);
    GPIO_IF_Set(m_GpioCs, 1);;
}

//****************************************************************************
//
//! \brief Powerup the ADS129x.
//!
//! This function is used to Power up ADS129x register.
//!
//! \param[in] None
//!
//! \return  DeviceID the ID ofADS129x
//
//****************************************************************************
unsigned char Init_ADS129x()
{
    //Tclk = 1/2.048 M = 0.5us
    int DeviceID = -1;
    //need time counting
    GPIO_IF_Set(m_GpioPwdn, 1);;
    Delay_us(5);
    GPIO_IF_Set(m_GpioReset, 1);;
    Delay_us(130000);	//tPOR 2^18 tCLK
    GPIO_IF_Set(m_GpioReset, 0);;
    Delay_us(1);		//tRST 2tCLK
    GPIO_IF_Set(m_GpioReset, 1);;
    Delay_us(9);		//18 tCLK
    GPIO_IF_Set(m_GpioStart, 0);;
    ADS_stop_rdatac_mode();
    DeviceID=ADS_read_id();
    return DeviceID;
}


//****************************************************************************
//
//! \brief configure the ADS129x.
//!
//! This function is used to configure the ADS129x register.
//!
//! \param[in] DeviceID is ADS129x series tag, which is used to figure out which type it is.
//!
//! \return  DeviceID the ID ofADS129x
//
//****************************************************************************
unsigned char ADS129x_CONFIG(unsigned char DeviceID)
{
    unsigned char ret = 1;
    GPIO_IF_Set(m_GpioCs, 0);;
    //  ADS_SPI_WREG(ADS_REG_CFG1, 0x01,0xA6);        	//CONFIG1:1010,0110(26h), Highresolution mode, Daisy-chain mode, Oscillator clock output enabled, ADC sample rate 500 for high-resolution mode
    // DR2-0: 101:500,  110: 250                                            	//Daisy chaining multiple ADCs enables the use of a single data receiver or a small FPGA,
    //	ADS_SPI_WREG(ADS_REG_CFG1, 0x85);        	//CONFIG1:1000,0101(85h), High-Resolution mode, Daisy-chain mode, Oscillator clock output disabled, ADC sample rate 1k for High-Resolution mode
    //	ADS_SPI_WREG(ADS_REG_CFG2, 0x13); 			//test DC                                              //Daisy chaining multiple ADCs enables the use of a single data receiver or a small FPGA,
    ret &= ADS_SPI_WREG(ADS_REG_CFG1, 0x85);			//11000101 High-Resolution mode,Multiple readback mode,Oscillator clock output disabled,1K
    ret &= ADS_SPI_WREG(ADS_REG_CFG2, 0x32);		   //00110010 Chopping frequency constant at fMOD/16,Test signals are generated internally,1 ?¨¢ ¡§C(VREFP ¡§C VREFN)/2.4mV,Not used
    //Enable internal reference buffer, Turn on RLD amplifier, set internal RLDREF voltage ,RLD sense is disabled
    ret &= ADS_SPI_WREG(ADS_REG_CFG3, 0xDE);        //CONFIG3:1100,1100(CCh)
    //RLD settings
    ret &= ADS_SPI_WREG(ADS_REG_RLD_SENSP, 0x02);   	//RLD_SENSP:0000,0010(02h), Select channel 2  , P-side for RLD sensing
    ret &= ADS_SPI_WREG(ADS_REG_RLD_SENSN, 0x02);   	//RLD_SENSN:0000,0010(02h), Select channel 2  , N-side for RLD sensing

    //Respiration settings
    ret &= ADS_SPI_WREG(ADS_REG_RESP, 0xE2);        //RESP:1110,0010(E2h), respiration demodulation/modulation on, phase=22.5,  Internal respiration with internal signals
    //ADS_SPI_WREG(ADS_REG_RESP, 0x20);
    ret &= ADS_SPI_WREG(ADS_REG_CONFIG4, 0x20);     //CONFIG4:0010,0100(24h), 32Hz modulation clock, continuous conversion mode, disconnect WCT to RLD, disable LEADOFF
    //ADS_SPI_WREG(ADS_REG_CONFIG2, 0X01,0x10);     //inner test signal

    //channel settings
    ret &= ADS_SPI_WREG(ADS_REG_CH1SET, 0x00);      //gain 3 for respiration(datasheetP74: when fmod=32Hz, gain 3/4 & phase 112.5/135 is recommended)
    ret &= ADS_SPI_WREG(ADS_REG_CH2SET, 0x00);      //gain 6 for single lead EMG   //inner test signal
    ret &= ADS_SPI_WREG(ADS_REG_CH3SET, 0x00);
    ret &= ADS_SPI_WREG(ADS_REG_CH4SET, 0x00);      //gain 3 for chest lead
    if((DeviceID & 0x03) != 0)			//only ads1296, ads1296R or ads1298, ads1298R
    {
        ret &= ADS_SPI_WREG(ADS_REG_CH5SET, 0x80);
        ret &= ADS_SPI_WREG(ADS_REG_CH6SET, 0x80);
        if((DeviceID & 0x02) != 0)		//only ads12968 or ads1298R
        {
            ret &= ADS_SPI_WREG(ADS_REG_CH7SET, 0x80);
            ret &= ADS_SPI_WREG(ADS_REG_CH8SET, 0x80);
        }
    }

    //ADS_SPI_WREG(ADS_REG_PACE, 0x01,0x01);        //turn off the output of PGA

    GPIO_IF_Set(m_GpioCs, 1);;
    return ret;
}


//****************************************************************************
//
//! \brief EMG_INT_Handler.
//!
//! This function read 152bits for one time. used together with RDATA opcode.
//! each has channel three bytes, MSB first
//!
//! \param[in] None.
//!
//! \return  None.
//
//****************************************************************************
void EMG_INT_Handler()
{
    uint8_t status_buffer[3];
    uint8_t i =	0;
    uint8_t* p = (uint8_t*)&EmgData;
    
    GPIO_IF_INTCLR(m_GpioDrdy);
    //EMG_buffer: to store 24bits, Breath_buffer: to store 24bitss

    GPIO_IF_Set(m_GpioCs, 0);;

    for(i=0;i<27;i++)					//3bytes + 8*3bytes = 27
    {
        if(i<3)       					// status registers
            status_buffer[i] = ADS_SPI_RByte();
        else
        {
            if((CHANNEL_INFO>>(8-i/3) & 0x01) == 1)     //judge the channal enabled or disable
            {
                switch (i%3)
                {
                case 0	: //EmgData[j++] = i/3;		//fill the channal number
                    *p	= ADS_SPI_RByte();		//fill the data
                    p++;
                    break;
                case 1	:	*p	= ADS_SPI_RByte();
                    p++;
                    break;
                case 2	:	*p	= ADS_SPI_RByte();
                    p++;
                    break;
                }
            }
            else
            {
                ADS_SPI_RByte();        //dump
            }

        }
    }
    GPIO_IF_Set(m_GpioCs, 1);;
    EmgQueueSend(&EmgData);
}
