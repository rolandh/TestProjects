/*
 *   Flash routines for the dsPIC
 *   Copyright (C) 2006, 2007, 2008 by Temperature Technology
 *   Written by Roland Harrison <rollie@internode.on.net>
 */

#include "globals.h"

/** Error Codes for OW functions */
enum FLASH_CODES {
  /*! Used when device is busy */
  FLASH_BUSY = 1, 
  /*! Used when device is available */
  FLASH_AVAILABLE = 0,

};

void flash_enable();
void flash_disable();
int flash_check_busy();
void flash_write(uint32_t address, const uint8_t volatile * data, uint16_t size);
void flash_read(uint32_t address, uint8_t volatile * data, uint16_t size);
void flash_read_cbuf(uint32_t address, uint16_t size);
void flash_set_device(int device);
void flash_cs_lat(int cs);
void flash_low_power();
void host_comms();
void flash_set_device(int device);
void flash_cs_lat(int cs);
