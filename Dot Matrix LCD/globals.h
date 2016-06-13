/*
 *   A program to communicate with an Agena LCD via a pf30f3013
 *   Copyright (C) 2005, 2007, 2008 by Temperature Technology
 *   Written by Roland Harrison <roland@t-tec>
 */

#ifndef GLOBALS_H
#define GLOBALS_H

#include "p24FJ256GB106.h"

/** The \p ack_or_nack argument for i2c_byte().
    The values of this enum currently match the value we pass to ACKDT. */
enum i2c_ack {
  /** Don't acknowledge. */
  I2C_NACK = 1,
  /** Do acknowledge. */
  I2C_ACK = 0 
  };

/* LCD pin assignments for PIC24f */
#define LCD_VDD 		_LATG8
#define LCD_VDD_TRIS 	_TRISG8
#define LCD_E 			_LATF1
#define LCD_E_TRIS 		_TRISF1
#define LCD_RW			_LATF0
#define LCD_RW_TRIS		_TRISF0
#define LCD_RS			_LATD7
#define LCD_RS_TRIS		_TRISD7
#define LCD_RST			_LATD6
#define LCD_RST_TRIS	_TRISD6
#define LCD_CS			_LATD5
#define LCD_CS_TRIS		_TRISD5
#define LCD_BL			_LATD4
#define LCD_BL_TRIS		_TRISD4

//new board is 15
#define FLASH_CS_LAT _LATB13
#define FLASH_CS_PORT _RB13
#define FLASH_CS_TRIS _TRISB13

#define GREEN_LED _LATD1
#define YELLOW_LED _LATD2
#define RED_LED _LATD3

#define GREEN_LED_TRIS _TRISD1
#define YELLOW_LED_TRIS _TRISD2
#define RED_LED_TRIS _TRISD3


/* Definitions of some standard integer types. The number indicates how many bits long */
typedef signed char int8_t;
typedef unsigned char uint8_t;
typedef unsigned int uint16_t;
typedef unsigned long uint32_t;



#endif
