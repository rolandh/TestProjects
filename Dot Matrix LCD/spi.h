/*
 *   SPI routines for the dsPIC.
 *   Copyright (C) 2006, 2007, 2008 by Temperature Technology
 *   Written by John Steele Scott <john@t-tec.com.au>
 */

#ifndef SPI_H
#define SPI_H

#include "globals.h"

void spi_enable();
void spi_disable();
uint8_t spi_touch(uint8_t data);
void spi_send(uint8_t data);
void spi_setup(int sdi, int sdo, int sck);
void spi_set_mode(int mode);
void spi_set_clock(int clock);

enum spi_functions{
     SLAVE = 0,
	 MASTER = 1,
	 EXTERNAL = 0,
	 INTERNAL = 1
};

#endif
