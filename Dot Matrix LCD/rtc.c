/*
 *   Routines to provide a RTC (Real time clock) via a pf30f3013
 *   Copyright (C) 2005, 2007, 2008 by Temperature Technology
 *   Written by Roland Harrison <rollie@internode.on.net>
 */

#include "rtc.h"
#include "helper.h"
#include "globals.h"
#include "MDD File System/FSIO.h"
#ifdef C30

#else
# include <stdint.h>
#endif



/** Sets the time.

    \bug The documentation for this function does not specify how to
    the values are interpreted.

    \par Example: 2007, 12/30 24:59:59
    \code
    rtc_set_time(2007, 5, 30, 23, 59, 40);
    \endcode
 */
void rtc_set_time(int years, int months, int days,
                  int hours, int mins, int secs)
{
  int second, minute, hour, day, month, year = {0};

  year = binary_to_bcd(years - 2000);
  month = binary_to_bcd(months);
  day = binary_to_bcd(days);
  hour = binary_to_bcd(hours);
  minute = binary_to_bcd(mins);
  second = binary_to_bcd(secs);
  
  /* Allow writes to RTCVAL register */
  __builtin_write_OSCCONL(2);
  // __builtin_write_RTCWEN(); 
    asm volatile("disi #5"); // Disable interrupts for 5 instructions
    asm volatile("mov #0x55, w7");
    asm volatile("mov w7,_NVMKEY");
    asm volatile("mov #0xAA, w8");
    asm volatile("mov w8,_NVMKEY");
    asm volatile("bset _RCFGCAL, #13");
    asm volatile("nop");
    asm volatile("nop"); 

  RCFGCALbits.RTCPTR = 3;
   /*RTCVAL = 0x0007;  // Example Set to 2007 
   RTCVAL = 0x1012;  // Example Set to Oct 12th 
   RTCVAL = 0x0115;  // Example Set to Tuesday 15 hours 
   RTCVAL = 0x0600;  // Example Set to 0 min 0 sec */ 
//RTCVAL = ((year & 0xF) | (year << 8));
  RTCVAL = year & 0xFF;
  RTCVAL = (day | (month << 8));
  RTCVAL = (hour );
  RTCVAL = (second | (minute << 8));
  /* Enable RTC */
  RCFGCALbits.RTCEN = 1;

  /* Lock RTCVAL register */
  _RTCWREN = 0;

}


/** Gets the time.

    Stores time in addresses passed to it.

    None of the arguments may be NULL.

    \bug The documentation for this function does not specify how to
    interpret the values.

    \par Example:
    \code
    int second, minute, hour, day, month, year;
    rtc_get_time(&year, &month, &day, &hour, &minute, &second);
    \endcode
*/
void rtc_get_time(int * year_out, int * month_out, int * day_out,
                  int * hour_out, int * min_out, int * sec_out)
{

  uint16_t second, minute, hour, day, month, year;
  uint16_t temp_rtcval = 0;
  /* Allow writes to RTCVAL register */
  __builtin_write_OSCCONL(2);
  //__builtin_write_RTCWEN(); 
  
    asm volatile("disi #5"); // Disable interrupts for 5 instructions
    asm volatile("mov #0x55, w7");
    asm volatile("mov w7,_NVMKEY");
    asm volatile("mov #0xAA, w8");
    asm volatile("mov w8,_NVMKEY");
    asm volatile("bset _RCFGCAL, #13");
    asm volatile("nop");
    asm volatile("nop"); 

  Nop();
  Nop();
  RCFGCALbits.RTCPTR = 3; 
  /* Pointer to RTCVAL decrements on each read */
  temp_rtcval = RTCVAL;
  year = bcd_to_binary(temp_rtcval);
  
  temp_rtcval = RTCVAL;
  month = bcd_to_binary(temp_rtcval >> 8);
  day = bcd_to_binary(temp_rtcval & 0xFF);

  temp_rtcval = RTCVAL;
  hour = bcd_to_binary(temp_rtcval & 0xFF);

  temp_rtcval = RTCVAL;
  minute = bcd_to_binary(temp_rtcval >> 8);
  second = bcd_to_binary(temp_rtcval & 0xFF);
  
  /* Lock RTCVAL register */
  _RTCWREN = 0;

  *year_out = year;
  *month_out = month;
  *day_out = day;
  *hour_out = hour;
  *min_out = minute;
  *sec_out = second;          
}










