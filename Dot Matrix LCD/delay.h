/*
 *   Delay module header.
 *   Copyright (C) 2006, 2008 by Temperature Technology
 *   Written by John Steele Scott <john@t-tec.com.au>
 *   Modified to run on PIC24 by Roland Harrison <rollie@internode.on.net>
 */

#ifndef DELAY_H
#define DELAY_H

void timer2_delay(unsigned int nosc, unsigned int count, unsigned int tckps);

/** Use Timer2 to create a delay of 1024 ticks per second.
    \note The clock is switched to LPOSC oscillator for this function,
    and switched back before it returns.
    Delay must be greater then 15 ticks or may hang.  */
__inline__ static void delay_1024_ticks_per_second(int ticks)
{
  /* switch to SOSC and use a 1:8 timer prescaler */
	//This use to use nosc = 4;
	timer2_delay(1, ticks, 3);
}

void delay_ticks(int ticks);
void delay_ticks_ibutton(int ticks);
/** Use Timer2 to create a delay of 8192 ticks per second.
    \note The clock is switched to LPRC oscillator for this function,
    and switched back before it returns.
    Delay must be greater then 15 ticks or may hang. */
__inline__ static void delay_8192_ticks_per_second(int ticks)
{
  /* switch to LPRC and use a 1:1 timer prescaler */
//This use to use nosc = 4;
	timer2_delay(0, ticks, 0);
}

/** Use Timer2 to create a delay of 1000000 ticks per second.
    \note The clock is switched to FRC oscillator for this function,
    and switched back before it returns. 
    ~32us overhead */
__inline__ static void delay_1000000_ticks_per_second(int ticks)
{
  /* switch to FRC and use a 1:1 timer prescaler */
	timer2_delay(0, ticks, 0);
}

#endif
