/*
 *   I2C master routines for the dsPIC
 *   Copyright (C) 2006, 2007, 2008 by Temperature Technology
 *   Written by John Steele Scott <john@t-tec.com.au>
 *   Modified by Roland Harrison to run on a PIC24 <rollie@internode.
 */

#include "i2c.h"
#include "osccon.h"

static OSCCONBITS old_osc;
static int i2c_enabled = 0;

/** Enable the I2C module for master mode.
    \note While I2C is enabled, the microcontroller will run from the
    FRC oscillator. Call i2c_disable() to restore the previous
    oscillator configuration. */
void i2c_enable()
{
  if (i2c_enabled == 1){
	  return;
  }
  i2c_enabled = 1;
  OSCCONBITS new_osc;

  /* Switch to FRC oscillator */
  old_osc = new_osc = OSCCONbits;
  new_osc.NOSC = 0;
  new_osc.OSWEN = 1;
  set_osccon(new_osc);

  /* Set I2C baud rate generator. Using FRC, F = 7.37 MHz, Fcy=1.8425
     MHz, want Fscl=400kHZ, using equation from page 21-4 of dsPIC
     family refman: I2CBRG = INT(Fcy/Fscl) - 1 */
  //I2C1BRG = 4;                  /* gives Fscl=368.5 kHz */
  I2C1BRG = 0x4E;				//sets Fscl ~100khz or lower

  I2C1STAT = 0;
  I2C1CONbits.I2CEN = 1;
  I2C1CONbits.A10M = 1;
  _MI2C1IE = 1;
}


/** Disable the I2C module for master mode. */
void i2c_disable()
{
	i2c_enabled = 0;

  if (_SI2C1IE == 1) {
  /* Leave slave enabled */
  /* I2C1CONbits.A10M = 0; */
  I2C1CONbits.I2CEN = 1;
  } else {
    I2C1CONbits.I2CEN = 0;
  }

  /* Restore previous oscillator configuration. */
  old_osc.OSWEN = 1;
  set_osccon(old_osc);

}

/** Send an I2C START condition.
    \note Do not use this for a repeated start, use i2c_rstart() instead. */
void i2c_start()
{
  I2C1CONbits.SEN = 1;
  /* FIXME why is this commented out */
  //if((I2C1STATbits.S == 0) && (I2C1STATbits.P == 1)){
  /* wait until START is complete */
  while (I2C1CONbits.SEN == 1)
    Idle();
  // }
}

/** Send an I2C repeated START condition. */
void i2c_rstart()
{
  I2C1CONbits.RSEN = 1;
  /* wait until repeated START is complete */
  while (I2C1CONbits.RSEN == 1)
    Idle();
}

/** Send an I2C STOP condition. */
void i2c_stop()
{
  I2C1CONbits.PEN = 1;
  /* wait until STOP is complete */
  while (I2C1CONbits.PEN == 1)
    Idle();
}

/** Send a byte on the I2C bus. 
    \param data The byte to send. */
int i2c_sbyte(const uint8_t data)
{
  /* put data in transmit buffer */
  I2C1TRN = data;

  /* wait until transmit + ack timeslots have been used */
  while (I2C1STATbits.TRSTAT == 1)
    Idle();

  /* return 0 if slave doesn't ack */
  return !I2C1STATbits.ACKSTAT;
}

/** Receive a byte from the I2C bus.
    \param data The location to store the received byte.
    \param ack_or_nack See enum i2c_ack. */
int i2c_rbyte(uint8_t *data, enum i2c_ack ack_or_nack)
{
  /* enable receive */
  I2C1CONbits.RCEN = 1;

  /* wait for receive to finish */
  while (I2C1CONbits.RCEN == 1)
    Idle();

  /* save data */
  *data = I2C1RCV;

  /* send ack or nack */
  I2C1CONbits.ACKDT = ack_or_nack;
  I2C1CONbits.ACKEN = 1;
  
  /* wait for ack/nack to be sent */
  while (I2C1CONbits.ACKEN == 1)
    Idle();

  /* there's no error status that would be useful to return */
  return 1;
}

/** Interrupt handler for master I2C, only used to get us out of Idle. */
void __attribute__((interrupt, no_auto_psv)) _MI2C1Interrupt(void)
{
  _MI2C1IF = 0;
}


