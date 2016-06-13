/*
 *   Delay module implementation.
 *   Copyright (C) 2006, 2007 by Temperature Technology
 *   Written by John Steele Scott <john@t-tec.com.au>
 */

#include <p24fxxxx.h>
#include <timer.h>
#include "osccon.h"
#include "globals.h"

/** Semaphore shared between timer2_delay and T2Interrupt. */
static volatile unsigned char timer2_interrupt_semaphore = 0;

int postscaler = 16;
/** Timer 2 Interrupt.
    This ISR operates in conjunction with timer2_delay. */
void __attribute__((interrupt, no_auto_psv)) _T2Interrupt(void)
{
  postscaler--;
  if(postscaler == 0){
     timer2_interrupt_semaphore = 1;
     postscaler = 16;  
  }
  
  IFS0bits.T2IF = 0;
}

void delay_ticks(int ticks){

  int cosc = OSCCONbits.COSC;

  //Factor to decide whether to divide the ticks in half
  //as we are running at 8mhz when cosc = 3 and 4mhz when cosc = 1
  int factor = 1;

  if(cosc == 1){
  	factor = 1;
  } else if (cosc == 3){
  	factor = 2;
  }
  
  /* Turn off timer, clear counter, set period register, clear semaphore. */
  T2CON = 0;
  TMR2 = 0;
  timer2_interrupt_semaphore = 0;
  PR2 = ticks*factor;

  /* Enable Timer2 interrupt */
  IPC1bits.T2IP = 6;
  IEC0bits.T2IE = 1;
  IFS0bits.T2IF = 0;


  /* Timer2 on and running from Tcy with specified prescaler. */
  T2CONbits.TCKPS = 3;
  T2CONbits.TCS = 0;	//Use internal clock / 2 = 4Mhz
  T2CONbits.T32 = 0;
  T2CONbits.TSIDL = 0;
  T2CONbits.TON = 1;

  while (timer2_interrupt_semaphore == 0){
    Idle();
  }
  
  //Turn the timer and interrupts off
  IFS0bits.T2IF = 0;
  IEC0bits.T2IE = 0;
  T2CONbits.TON = 0;

}

/** Special delay that will break if the ibutton */
void delay_ticks_ibutton(int ticks){

  int cosc = OSCCONbits.COSC;

  //Factor to decide whether to divide the ticks in half
  //as we are running at 8mhz when cosc = 3 and 4mhz when cosc = 1
  int factor = 1;

  if(cosc == 1){
  	factor = 1;
  } else if (cosc == 3){
  	factor = 2;
  }
  
  /* Turn off timer, clear counter, set period register, clear semaphore. */
  T2CON = 0;
  TMR2 = 0;
  timer2_interrupt_semaphore = 0;
  PR2 = ticks*factor;

  /* Enable Timer2 interrupt */
  IPC1bits.T2IP = 6;
  IEC0bits.T2IE = 1;
  IFS0bits.T2IF = 0;


  /* Timer2 on and running from Tcy with specified prescaler. */
  T2CONbits.TCKPS = 3;
  T2CONbits.TCS = 0;	//Use internal clock / 2 = 4Mhz
  T2CONbits.T32 = 0;
  T2CONbits.TSIDL = 0;
  T2CONbits.TON = 1;

  //If an ibutton is present quit the try again delay to make pressing the button
  //onto the ibutton plug not sit there saying try again when you are pressing it on
  while ((timer2_interrupt_semaphore == 0) && (OW_PORT == 1)){
    Idle();
  }
  
  //Turn the timer and interrupts off
  IFS0bits.T2IF = 0;
  IEC0bits.T2IE = 0;
  T2CONbits.TON = 0;
}

/** Idle wait for \p count beats on Timer2.
    \param nosc the clock to switch to while we wait.
    \param count the number of ticks of timer2 to wait for.
    \param tckps the prescaler setting to set in T2CON. */
void timer2_delay(unsigned int nosc, unsigned int count, unsigned int tckps)
{
  OSCCONBITS original_osccon, new_osccon;
  CLKDIVBITS original_clkdiv;

  original_osccon = OSCCONbits; /* save old OSCCON */
  original_clkdiv = CLKDIVbits;  

  new_osccon = original_osccon;    /* make copy to modify */

  /* Perform oscillator switch */
  new_osccon.NOSC = nosc;
  new_osccon.OSWEN = 1;

  set_osccon(new_osccon);
  while(OSCCONbits.OSWEN != 0)
    continue;

  /* Turn off timer, clear counter, set period register, clear semaphore. */
  T2CON = 0;
  TMR2 = 0;
  timer2_interrupt_semaphore = 0;
  PR2 = count;

  /* Enable Timer2 interrupt */
  //ConfigIntTimer2(T2_INT_PRIOR_6 & T2_INT_ON);
  IPC1bits.T2IP = 6;
  IEC0bits.T2IE = 1;
  IFS0bits.T2IF = 0;


  /* Timer2 on and running from Tcy with specified prescaler. */
  T2CONbits.TCKPS = tckps;
  T2CONbits.T32 = 0;
  T2CONbits.TSIDL = 0;
  T2CONbits.TON = 1;

  while (timer2_interrupt_semaphore == 0)
    Idle();

  /* semaphore has been set by interrupt handler, stop timer and return */
  T2CON = 0;

  /* switch back to original oscillator */
  original_osccon.OSWEN = 1;
  set_osccon(original_osccon);
  while(OSCCONbits.OSWEN != 0)
    continue;

  CLKDIVbits = original_clkdiv;

}
