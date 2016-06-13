/*
 *   Routines to communicate with a Samsung S6B1713 Dot matix LCD controller via a Pic24
 *   Copyright (C) Roland Harrison
 *   Written by Roland Harrison <rollie@internode.on.net>
 */

#include "globals.h"
#include "delay.h"
#include "i2c.h"
#include "lcd.h"
#include "helper.h"
#include <string.h>
#include <stdio.h>
#include "ow.h"
#include "measure.h"

/** Flag is set when LCD is off */
static int lcd_off_flag = 1;

/** Slave address = 3C however it always bitshifted by 1 to the left when we use it(<< 1) */
static int SLAVE_ADDRESS = 0x78;

static int font_size;

//As each byte can access 10 different icons we need to keep track
//of which ones are on/off so that we don't override already turned
//on icons.
static uint8_t icon_data[0xF] = {0};

enum DATA {
  //RS bits
  FUNCTION = 0x0,
  WRITE_TO_RAM = 0x40, //RS bit is bit 6 in control byte, eg (0x1 << 7 = 0x40)
  //RW bits
  WRITE = 0x0,
  READ = 0x1,
  //CO bits
  DATA_STREAM = 0x0,
  INTERLEAVE_DATA = 0x80,	//CO bit is bit 7 in control byte, eg (0x1 << 8 = 0x80)
};

/** Sends data to LCD. 
returns 0 if display did not ack back
returns 1 if display successfully acked data */
static int lcd_send_bytes(uint8_t * data, int size){
	
	int result = 0;

	//Send data
	int i;
	for(i = 0; i < size; i++){
		PORTD = data[i];
	}


	return result;
}

/** Powers off LCD */
//returns 0 if display did not ack back
//returns 1 if display successfully turned off
//returns 2 if display is already off
int lcd_power_off(){

    /* Check if LCD is already off */
    if (lcd_off_flag == 1) {
      return 2;
    }

	uint8_t off[3];
	off[0] = SLAVE_ADDRESS;
	off[1] = 0x00;
	off[2] = 0x0B;
	int result = lcd_send_bytes(off, 3);

	lcd_VDD = 0; //kill vdd to display

	lcd_off_flag = 1;
	
	return result;
}



void lcd_power_on()
{
	//Reference circuit with internal regulator resistors and all LCD power circuits are used
	//4-Time V/C: ON, V/R: ON, V/F: ON)
	
	RST = 0;
	VLCD = 1;			//Power on LCD VDD
	delay_ticks(50);	//Wait for power to stabilise
	RST = 1;			//pull reset pin high
	
	
	RS = 0;
	RW = 0;
	
	//Power save [Display OFF] [Entire Display ON]
	PORTD = 0xAE; //Display off
	PORTD = 0xA5; //entire display on
	
	PORTD = 0xA1; //ADC select (direction eg SEG1>SEG132
	PORTD = 0xC0; //SHL select
	PORTD = 0xA3; //LCD Bias select
	
	//Set the LCD Operating Voltage by Internal Instructions
	PORTD = 0x23; //[Regulator Resistor Select]
	PORTD = 0x15; //[Reference Voltage Register Set]
	
	//Release power save
	PORTD = 0xA4; //[Entire Display OFF]
	PORTD = 0xAF; //[Display ON]
	
	delay_ticks(100);

}



/** Clears either top line, bottom line or both lines of the LCD screen 
   \param line One of lcd_TOP, lcd_BOTTOM or lcd_ALL. */
int lcd_clear(int line)
{
	int result = 1;

	result &= lcd_print_line(line, "                ");

	if (line == lcd_ALL){
		result &= lcd_print_text("                                                ");
	}
	return result;
}



/** Prints a string of text up to 48 characters and
	wraps around.

    \param lcd_position, pointer to string
	\return 0 if the display did not ack.
	\return 1 if the display acked back. */
int lcd_print_text(const char *text)
{	
	int result;
    unsigned char i  = 15;

	i2c_enable();
	i2c_start();

	result &= i2c_sbyte(SLAVE_ADDRESS); 
	result &= i2c_sbyte(0x40);  //set RS for writing to DDRAM
	

	//Send first line of text
    
	for(i = 0; text[i] != '\0' && i <= 48; i++){
		result &= i2c_sbyte(*(text+i));
		delay_us(35);
	}

	delay_us(35);

	i2c_stop();
	i2c_disable();
	
	return result;
   
}

/

/** Reserved ICONRAM addresses for segment data. 
	These addresses work backwards, eg bit 1 is 24
	bit 2 is 23... 9 */
enum iconram_address {
 	ICONRAM_HOURS = 24, 	//38-32 for first seg, 31-24 for 2nd seg
  	ICONRAM__MINUTES = 9,	//02-16 for first seg, 15-9 for 2nd seg
  	ICONRAM__MONTH = 0x50,
  	ICONRAM__MDAY = 0x58,
  	ICONRAM__RSEG = 0x60
};

/* Segment pattern lookup table for each digit */
static uint8_t lcd_dig[] = {0x77, 0x03, 0x3E, 0x1F, 0x4B, 
							 0x5D, 0x7D, 0x13, 0x7F, 0x5F, 0, 0, 0, 0, 0, 0, 0, 0};



/** Prints a string of text up to 16 characters
	to a specific line.

    \param lcd_position, pointer to string
	\return 0 if the display did not ack.
	\return 1 if the display acked back. */
int lcd_print_line(int line, const char * text){
	int result;
    unsigned char i  = 15;

	i2c_enable();
	i2c_start();

	result &= i2c_sbyte(SLAVE_ADDRESS); 
	result &= i2c_sbyte(0x80);			//control byte
	result &= i2c_sbyte(0x80 | line );  //set ddram address
	result &= i2c_sbyte(0x40);

	//Send first line of text
    for(i = 0; text[i] != '\0' && i <= 16; i++){
		result &= i2c_sbyte(*(text+i));
		delay_us(35);
	}

	delay_us(35);

	i2c_stop();
	i2c_disable();
	
	return result;
 
}


