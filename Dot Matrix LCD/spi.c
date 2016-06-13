/*
 *   SPI routines for the dsPIC.
 *   Copyright (C) 2006, 2007, 2008 by Temperature Technology
 *   Written by Roland Harrison <roland.c.harrison@gmail.com>
 */

#include "globals.h"
#include "spi.h"
#include "hlcd.h"
#include "spi_mp.h"

int spi_enabled = 0;
static volatile double timer5_counter = 0;

/** Sets the clock to internal/external mode
	0 internal clock is disabled
	1 internal clock is enabled 
	/parameter EXTERNAL, INTERNAL */
void spi_set_clock(int clock){
	SPI1CON1bits.DISSCK = clock;
}

/** Sets the SPI module into slave/master
	mode SPI must be set up and running 
	for this to have any effect 
	/parameter mode SLAVE, MASTER
	0 = Slave mode */
void spi_set_mode(int mode){
	/* 0 = Slave, 1 = Master */
	SPI1CON1bits.MSTEN = mode;
}


/** Enable the SPI module. */
void spi_enable()
{
	if (spi_enabled == 1){
		return;
	}
	spi_enabled = 1;


  /* Idle SPI clock high, active low */
  SPI1CON1bits.CKP = 1;
  /* Change data on idle->active clock transition. */
  SPI1CON1bits.CKE = 0;
  /* Sample data at middle of output time */
  SPI1CON1bits.SMP = 0;
  /* Enable master mode. */
  SPI1CON1bits.MSTEN = 1;
  /* Disable SS pin */
  SPI1CON1bits.SSEN = 0;

  /* Clear SPIROV as recommended in dsPIC family reference section 20.3.2.1 */
  SPI1STATbits.SPIROV = 0;

  /* Set primary prescaler at 1:1, secondary at 2:1, gives an SPI
     clock of about Fsck=921kHz with Fcy=1.8425 MHz */
  SPI1CON1bits.PPRE = 3;
  SPI1CON1bits.SPRE = 7;

  

  /* Enable SPI module. */
  SPI1STATbits.SPIEN = 1;

//	ConfigIntSPI1(SPI_INT_DIS);
//
//	OpenSPI1(
//		SPI_MODE16_OFF & ENABLE_SCK_PIN & ENABLE_SDO_PIN & SPI_SMP_ON & CLK_POL_ACTIVE_LOW & SPI_CKE_OFF & MASTER_ENABLE_ON & SEC_PRESCAL_2_1 & PRI_PRESCAL_1_1,
//		0x00,
//		SPI_ENABLE
//	);

}

/** Disable the SPI module. */
void spi_disable()
{
  spi_enabled = 0;
  SPI1STATbits.SPIEN = 0;

}


int timer3_timeout = 0;

/** Enables timer3, interupts approximately every 
	interrupt_ms milliseconds 
	/note 1000ms max interrupt interval */
void start_timer3(int interrupt_ms)
{
  
  T3CON = 0;  /* Turn off timer 3 */
  TMR3 = 0;   /* Clear count register */
  PR3 = 0;    /* Clear period register */
  
  /* set Timer3 interrupt priority level to 4 */
  IPC2bits.T3IP = 4;
  
  IEC0bits.T3IE = 1;
  T2CONbits.T32 = 0; 
  T3CONbits.TCS = 0; 
  T3CONbits.TCKPS0 = 1; 		/* Set prescaler to 1:1 */
  T3CONbits.TCKPS1 = 0;
  PR3 = (interrupt_ms * 3);      /* Set period register 
								//	this is quite inaccurate */
  T3CONbits.TSIDL = 0; 			/* Keep counting when idle */
  T3CONbits.TGATE = 0;
  T3CONbits.TON = 1;  			 /* turn on Timer 3 */
}

/** Stops timer 3 from running */
void stop_timer3()
{
  timer3_timeout = 0;
  TMR3 = 0;   /* Clear count register */
  PR3 = 0;    /* Clear period register */
  IEC0bits.T3IE = 0;
  T3CONbits.TON = 0;
}

/** Used when sending data in slave mode */
void spi_send(uint8_t data){

	/* Dummy Read */
	uint8_t temp = SPI1BUF;
	
	/* Initiate transmission */
  	SPI1BUF = data;

	/* wait until transmit buffer is empty 
	   eg. when the PC has toggled the clock */
  	while (SPI1STATbits.SPITBF){
    	continue;
  	}


}


/** Send a byte over SPI, and return the received byte.
    In SPI, send and receive happens at the same time. If you don't
    want to send anything, use 0 as the \p data parameter.*/
uint8_t spi_touch(uint8_t data)
{

  //start_timer3(200);
  timer3_timeout = 0;

  /* wait until transmit buffer is empty */
  while (SPI1STATbits.SPITBF && !timer3_timeout){
    continue;
  }

  /* Initiate transmission */
  SPI1BUF = data;

  /* wait until received data has been shifted in */
  //FIXME possible infinite loop, needs a timer incase spi breaks
   while (!SPI1STATbits.SPIRBF && !timer3_timeout){
    continue;
  }
  
  //stop_timer3();

  /* return received data */
    return SPI1BUF;
}

void spi_setup(int sdi, int sdo, int sck)
{
  uint8_t *rpo_reg = (uint8_t*)&RPOR0;
  rpo_reg += sdo;
  *rpo_reg = 7;
  
  uint8_t *rpo_reg1 = (uint8_t*)&RPOR0;
  rpo_reg1 += sck;
  *rpo_reg1 = 8;

  _SDI1R = sdi;

  //_RP9R = 14; //set SSEN pin to RB14
  //RPINR21 |= 14;



}
