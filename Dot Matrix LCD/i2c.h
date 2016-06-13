/*
 *   I2C master routines for the dsPIC.
 *   Copyright (C) 2006, 2008 by Temperature Technology
 *   Written by John Steele Scott <john@t-tec.com.au>
 */

#ifndef I2C_H
#define I2C_H

#include "globals.h"


void i2c_enable();
void i2c_disable();
void i2c_start();
void i2c_rstart();
void i2c_stop();
int i2c_sbyte(const uint8_t data);
int i2c_rbyte(uint8_t *data, enum i2c_ack ack_or_nack);


#endif
