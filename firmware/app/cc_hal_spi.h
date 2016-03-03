 /* 
 * File Name          : cc_hal_spi.h
 * Author             : Joy
 * Version            : $Revision:$
 * Date               : $Date:$
 * Description        : 
 *                      
 * HISTORY:
 * Date               | Modification                    | Author
 * 28/09/2015         | Initial Revision                | 
 
 */

#include <stdint.h>

//int SPI_WREG(unsigned char reg_address, unsigned char data);
//unsigned char SPI_RREG(unsigned char reg_address );
void Spi_Init();
unsigned char SPI_RByte();
void SPI_WByte(unsigned char byte);