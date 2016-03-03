 /* 
 * File Name          : cc_hal_ads129x.h
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 7/11/2015         | Initial Revision                | 
 
 */
#ifndef __ADS129X_H__
#define __ADS129X_H__

#define CHANNEL_INFO 0xf0

void SpiInit();
void ADS_SPI_WByte(unsigned char byte);
unsigned char ADS_SPI_RByte();
unsigned char ADS_SPI_RREG(unsigned char reg_address );
int ADS_SPI_WREG(unsigned char reg_address, unsigned char data);
unsigned char ADS_read_id();
void ADS_stop_rdatac_mode();
unsigned char Init_ADS129x();
unsigned char ADS129x_CONFIG(unsigned char DeviceID);
void EMG_INT_Handler();
void ADS_start_rdatac_mode();

#endif // __ADS129X_H__