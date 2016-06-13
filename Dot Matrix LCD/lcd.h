/*
 *   Routines to communicate with a Samsung S6B1713 Dot matix LCD controller via a Pic24
 *   Copyright (C) Roland Harrison
 *   Written by Roland Harrison <rollie@internode.on.net>
 */

#ifndef LCD_H_
#define LCD_H_


int lcd_power_on();
int lcd_power_off();
int lcd_print_line(int line, const char * text);
int lcd_print_text(const char * text);
int lcd_change_font_size(int size);
int lcd_clear(int line);
int lcd_set_battery_meter(int value);
void lcd_print_integers(int line, uint8_t * integers, int size);
void lcd_set_date_and_time(uint8_t month, uint8_t day, uint8_t hour, uint8_t minute);

enum font_size {
  DOUBLE_TOP = 0,
  DOUBLE_BOTTOM= 1,
  SINGLE_ALL = 2
};

enum lcd_position{
     lcd_TOP = 0x0,
	 lcd_MIDDLE = 0x10,
     lcd_BOTTOM = 0x20,
	 lcd_ALL = 0x3
};


#endif /*lcd_H_*/
