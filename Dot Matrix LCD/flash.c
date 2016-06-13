/*
 *   Flash routines for the dsPIC
 *   Copyright (C) 2006, 2007, 2008 by Temperature Technology
 *   Written by Roland Harrison <rollie@internode.on.net>
 */


#include "globals.h"
#include "delay.h"
#include "hlcd.h"
#include <stdio.h>
#include "spi.h"
#include "flash.h"

/** A physical address on the flash.
    Logical addresses is contiguous from zero to (flash size - 1),
    while physical addresses go from byte 0 to byte 263 on each page.

    flash_logical_to_physical() can be used to get a physical address
    from a logical address. All the functions exported by this module
    take logical address, so users of this module do not need to worry
    about physical addresses. */
typedef struct {
  uint16_t page;                /**< The page number. */
  uint16_t addr;                /**< The byte address within the page. */
} phys_addr_t;

enum flash_cmd {
  WRITE_ENABLE = 0x06,
  PAGE_PROGRAM = 0x02,
  READ_STATUS = 0x05,
  FAST_READ = 0x0B,
  READ = 0x03,
  CMD_RDSR = 0xD7
};

static int low_power_enabled = 0;
static int flash_enabled = 0;

int flash_chip = 0; //Default to use the first chip

void flash_set_device(int device){
	flash_chip = device;
}

/** Triggers the cs latch of the currently selected flash chip */
void flash_cs_lat(int cs){
	if(flash_chip == 0){
		FLASH0_CS_LAT = cs;
	} else if(flash_chip == 1){
		FLASH1_CS_LAT = cs;
	}
}


/** This function forces the flash into low power mode for sleep */
void flash_low_power()
{
  spi_enable();
  flash_cs_lat(0);
  spi_touch(0xB9);
  flash_cs_lat(1);
  spi_disable();
  low_power_enabled = 1;
}

/** This function enables the flash module */
void flash_enable(){
  if (flash_enabled == 1){
		return;
	}

  flash_enabled = 1;
  spi_enable();
  /* Low power enabled bring out of deep sleep */
	flash_cs_lat(0);
  	spi_touch(0xAB);
  	flash_cs_lat(1);             /* Select device */
//	delay_us(500);
	delay_ticks(10);
	low_power_enabled = 0;
	flash_cs_lat(1);
}

/** This function disables the flash, flash_lower_power() should
    be called first. */
void flash_disable()
{
  flash_enabled = 0;
  flash_low_power();
  spi_disable();
}


long read_ident(){
	flash_cs_lat(0);
	spi_touch(0x9F);
	spi_touch(0x0);
	spi_touch(0x0);
	spi_touch(0x0);
	flash_cs_lat(1);
	while(1){}
}

/** Enables writing to the flash chip */
void write_enable(){

  flash_cs_lat(0);
  spi_touch(WRITE_ENABLE);
  flash_cs_lat(1);

}

/** Check if device is ready to accept a command. 

    \return Returns FLASH_BUSY or FLASH_AVAILABLE */
int flash_check_busy()
{
  int flash_status;
  /* Send - Read status register opcode */
  flash_cs_lat(0);
   
  spi_touch(CMD_RDSR);

  flash_status = (spi_touch(0x0) & 0x80);

  flash_cs_lat(1);


  /*  bit 7 of status register is device is busy register */
  if (flash_status == 0x80) {
    return FLASH_AVAILABLE;          /* Device is busy */
  } else {
    return FLASH_BUSY;     /* Device is available */
  }

}


/** Converts a memory address to a physical address.

    \param[in] log_addr A logical address.
    \param[out] pa The physical address within the flash which
    corresponds to log_addr. */
static void flash_logical_to_physical(uint32_t log_addr, phys_addr_t * pa) 
{
  /* Find out what page to start at */
  pa->page = log_addr / 256;
  
  /* Address within page to start at */
  pa->addr = log_addr - (pa->page * 256);

  /* Figure out which chip this address is on and set accordingly */
  

}

static void setup_command(enum flash_cmd flash_cmd, const phys_addr_t * pa)
{
    uint8_t high_byte = (pa->page >> 7);
    uint8_t mid_byte = (((pa->page << 1) & 0xFE) | (pa->addr >> 8));
    uint8_t low_byte = (pa->addr & 0xFF);

    spi_touch(flash_cmd); 
    spi_touch(high_byte);
    spi_touch(mid_byte);
    spi_touch(low_byte);
}

/** Write data to the flash.

    \param address The (logical) address in the flash to write data to.
    \param data An array of bytes to be written.
    \param size The number of bytes to write.

    \note Buffer 1 on the flash is used as the write buffer. Any data
    currently in this buffer will be destroyed.
    \fixme busy_lat can be removed on final build
 */
void flash_write(uint32_t address, const uint8_t volatile * data, uint16_t size)
{

  uint16_t data_index = 0;
  uint16_t buffer_address = 0;
  phys_addr_t pa;

  flash_logical_to_physical(address, &pa);

  
  while (data_index < size) {

    while (flash_check_busy() == FLASH_BUSY){
      continue;
	}
	
	//Enable write bit to allow Page Program command to be executed
	write_enable();

    flash_cs_lat(0);

	//Page Program command followed by 3 address bytes.
	setup_command(PAGE_PROGRAM, &pa);

    for (buffer_address = 0; 
         (buffer_address < (256 - pa.addr)) && (data_index < size);
         buffer_address++) {
      
      spi_touch(data[data_index]);
      data_index++;
    }
   
    flash_cs_lat(1); /* Buffer is full, commit */  
   
    pa.addr = 0;
    pa.page++;
  }
}


/** Read data from the flash
    \param address The (logical) address in the flash to read data from.
    \param data An array of bytes to write the received bytes to.
    \param size The number of bytes to read. */
void flash_read(uint32_t address, uint8_t volatile * data, uint16_t size)
{
  phys_addr_t pa;
  flash_logical_to_physical(address, &pa);

    while (flash_check_busy() == FLASH_BUSY){
      continue;
	}

  flash_cs_lat(0); /* Initiate command sequence by high - low transistion */
  setup_command(FAST_READ, &pa); /* Read directly from page in high speed */

  spi_touch(0x0); //Dummy byte for fast read, normal read does not require this

  while (size--) {
    *(data++) = spi_touch(0x0);
  }

  flash_cs_lat(1); /* stop receiving data */
}

