 /* 
 * File Name          : cc_hal_spi.c
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 28/09/2015         | Initial Revision                | 
 
 */

// Driverlib includes
#include "hw_types.h"
#include "hw_memmap.h"
#include "spi.h"
#include "prcm.h"
#include "rom_map.h"
#include "utils.h"

#include "cc_utils.h"
#include "cc_hal_spi.h"

#define My_SPI_IF_BIT_RATE  20000000 //100000



void Spi_Init()
{
    MAP_PRCMPeripheralClkEnable(PRCM_GSPI, PRCM_RUN_MODE_CLK);
    //
    // Reset SPI
    //
    MAP_SPIReset(GSPI_BASE);

    //
    // Configure SPI interface
    //
    MAP_SPIConfigSetExpClk(GSPI_BASE,MAP_PRCMPeripheralClockGet(PRCM_GSPI),
    				My_SPI_IF_BIT_RATE,SPI_MODE_MASTER,SPI_SUB_MODE_0,
                     (SPI_SW_CTRL_CS |
                     SPI_3PIN_MODE |
                     SPI_TURBO_OFF |
                     SPI_CS_ACTIVELOW |
                     SPI_WL_8));


    //
    // Enable SPI for communication
    //
    MAP_SPIEnable(GSPI_BASE);
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
void SPI_WByte(unsigned char byte)
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
unsigned char SPI_RByte()
{
    unsigned char byte;
    MAP_SPITransfer(GSPI_BASE,0,&byte,1,
                    SPI_CS_ENABLE|SPI_CS_DISABLE);
    return byte;
}
/*
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
unsigned char SPI_RREG(unsigned char reg_address )
{
    uint8_t result;

    SPI_WByte(0x80|reg_address);//the same as upside

    result = SPI_RByte();//
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
int SPI_WREG(unsigned char reg_address, unsigned char data)
{
  unsigned char rdback;
  SPI_WByte(reg_address);
  SPI_WByte(data);//
  //read back
  rdback = SPI_RREG(reg_address);
   if(data == rdback)
     return 1;
   else
     return 0;
}
*/