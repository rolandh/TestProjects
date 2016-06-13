/*********************************************************************
 FileName:		main.c
 Dependencies:	See INCLUDES section
 Processor:		PIC18 or PIC24 USB Microcontrollers
 Hardware:		The code is natively intended to be used on the following
 				hardware platforms: PICDEM™ FS USB Demo Board, 
 				PIC18F87J50 FS USB Plug-In Module, or
 				Explorer 16 + PIC24 USB PIM.  The firmware may be
 				modified for use on other USB platforms by editing the
 				HardwareProfile.h file.
 Complier:  	Microchip C18 (for PIC18) or C30 (for PIC24)
 Company:		Microchip Technology, Inc.

 Software License Agreement:

 The software supplied herewith by Microchip Technology Incorporated
 (the “Company”) for its PIC® Microcontroller is intended and
 supplied to you, the Company’s customer, for use solely and
 exclusively on Microchip PIC Microcontroller products. The
 software is owned by the Company and/or its supplier, and is
 protected under applicable copyright laws. All rights are reserved.
 Any use in violation of the foregoing restrictions may subject the
 user to criminal sanctions under applicable laws, as well as to
 civil liability for the breach of the terms and conditions of this
 license.

 THIS SOFTWARE IS PROVIDED IN AN “AS IS” CONDITION. NO WARRANTIES,
 WHETHER EXPRESS, IMPLIED OR STATUTORY, INCLUDING, BUT NOT LIMITED
 TO, IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 PARTICULAR PURPOSE APPLY TO THIS SOFTWARE. THE COMPANY SHALL NOT,
 IN ANY CIRCUMSTANCES, BE LIABLE FOR SPECIAL, INCIDENTAL OR
 CONSEQUENTIAL DAMAGES, FOR ANY REASON WHATSOEVER.

********************************************************************
 File Description:

 Change History:
  Rev   Date         Description
  1.0   11/19/2004   Initial release
  2.1   02/26/2007   Updated for simplicity and to use common
                     coding style
********************************************************************/

/** I N C L U D E S **********************************************************/

#include "P24FJ256GB106.h"
#include <stdio.h>
#include <string.h>
#include "globals.h"
#include <timer.h>
#include "delay.h"
#include "rtc.h"
#include "flash.h"
#include "spi.h"
#include "osccon.h"
#include "helper.h"
#include "i2c.h"

/** Flag is set when iButton wakes PIC up */
int volatile ibutton_flag = 0;

/** Flag for timer1 interrupt */
int volatile timer1_flag = 0;

/** Flag is setwhen LCD is off */
static int volatile lcd_off_flag = 0;

/** Flag is set whenever the VBUS pin goes high (user plugs usb into pc) */
static int volatile VBUS_flag = 0;

/** This second flag is used for making sure we update the free space available
    value when the USB is unplugged. */
static int volatile VBUS_flag2 = 0;

/** Flag for when I2C2 interrupt wakes up PIC */
extern int volatile I2C2_flag;

const char FIRMWARE_REVISION = 'C';

	_CONFIG1( JTAGEN_OFF & GCP_OFF & GWRP_OFF & ICS_PGx1 & FWDTEN_OFF & WINDIS_OFF);
	_CONFIG2( PLLDIV_DIV2 & PLL_96MHZ_ON & FNOSC_PRIPLL & IESO_OFF & FCKSM_CSECMD & OSCIOFNC_OFF & DISUVREG_ON & IOL1WAY_OFF & POSCMOD_XT);
	_CONFIG3(WPCFG_WPCFGDIS & WPDIS_WPDIS);



/** This function grabs the time from the RTC module
    and updates the LCD with it.
    \note RTC/lcd module must be enabled/running for this to work. */
void update_time()
{
  if(lcd_off_flag != 1){   //Only update the screen if it is actually turned on
  	int second, minute, hour, day, month, year = {0};
  	rtc_get_time(&year, &month, &day, &hour, &minute, &second);
  
  	lcd_set_date_and_time(month, day, hour, minute);
  }
}




/** Bring auxillary modules out of low power mode and turn LCD back on */
void power_up(){
	if (lcd_off_flag == 1) {
		/* timeout was reached, start up LCD again */
	    lcd_power_on();
	    lcd_off_flag = 0;
		start_timer1();
	}
	start_timer1();
	i2c_enable();
	flash_enable();
}



/** Enables timer 1, interupts once per second
    \note Used for real time clock */
void start_timer1()
{
   
  T1CON = 0;  /* Turn off timer 1 */
  TMR1 = 0;   /* Clear count register */
  PR1 = 0;    /* Clear period register */
  
  /* set Timer1 interrupt priority level to 4 */
  IPC0bits.T1IP = 4;
  
  IEC0bits.T1IE = 1;
  IFS0bits.T1IF = 0;
  T1CONbits.TSYNC = 1; /* sync external clock */
  T1CONbits.TCS = 1;   /* Use external clock source */
  T1CONbits.TCKPS = 0; /* Set prescaler to 1:1 */
  PR1 = 32768;         /* Set period register to 1 sec */
  T1CONbits.TSIDL = 0; /* Keep counting when idle */
  T1CONbits.TON = 1;   /* turn on Timer 1 */
  timer1_flag = 0;
}

void stop_timer1(){
	IFS0bits.T1IF = 0;
	T1CONbits.TON = 0;
	IFS0bits.T1IF = 0;
	timer1_flag = 0;
}

/** Timer 1 Interrupt. */
void __attribute__((interrupt, no_auto_psv)) _T1Interrupt(void)
{
  timer1_flag = 1;
  IFS0bits.T1IF = 0;            /* Clear timer 1 interrupt */
}


/** Turns off LCD and powers down all auxillery modules for low power sleep mode */
void power_down(){
	if (lcd_off_flag == 0){
		//checks if lcd is already off
	    lcd_clear(lcd_ALL);
		lcd_set_battery_meter(-1);
	    lcd_power_off();
		flash_disable();
		i2c_disable();
	    lcd_off_flag = 1;
		lcd_VDD = 0;
		stop_timer1();
	}
}

/* Decrements the timeout */
void decrement_timer(){
	if (timeout > 0){
		timeout--;
	}
}


void __attribute__((__interrupt__,no_auto_psv)) _DefaultInterrupt(void)
{
	Nop();
	Nop(); //Place 2nd breakpoint here
	lcd_power_on();
	lcd_print_line(lcd_TOP, "ERROR: Trap Error");
	lcd_print_line(lcd_MIDDLE, "Contact Manafacturer");
	while(1); //Trap
}

int main(void){


    InitializeSystem();


    while(1)
    {

		/* Were we woken up by ibutton or by timer? */
	    if (timer1_flag == 1) {
	    	/* Was just timer, decrement timeout */
			decrement_timer();
			timer1_flag = 0;

      	}

    }
}


static void InitializeSystem(void)
{

	//Set all pins to outputs and drive low initially
	TRISB = 0;
	TRISC = 0;
	TRISD = 0;
	TRISE = 0;
	TRISG = 0;
	PORTB = 0;
	PORTC = 0;
	PORTD = 0;
	PORTE = 0;
	PORTG = 0;
	
	_CLKLOCK = 0;
	
	//Set tris bits for LEDs and flash CS pins.
 	FLASH1_CS_TRIS = 0;
  	FLASH1_CS_LAT = 1;
	GREEN_LED_TRIS = 0;
	YELLOW_LED_TRIS = 0;
	RED_LED_TRIS = 0;
	lcd_VDD_TRIS = 0;
	FLASH0_CS_TRIS = 0;
	FLASH1_CS_TRIS = 0;
    FLASH0_CS_LAT = 1;
    FLASH1_CS_LAT = 1;
	OW_TRIS = 1;

    //Initialize the SPI
	AD1PCFGL = 0xFFFF;
	AD1PCFGH = 0xFFFF;

    RPINR20bits.SDI1R = 16;	//SDI RP_16   SHOULD BE RP_29
    RPOR8bits.RP17R = 7;	//SDO RP_29   SHOULD BE RP_10
    RPOR5bits.RP10R = 8; 	//SCK1 Out RP_14  CORRECT

	ConfigIntSPI1(SPI_INT_DIS);

	OpenSPI1(
		SPI_MODE16_OFF & ENABLE_SCK_PIN & ENABLE_SDO_PIN & SPI_SMP_ON & CLK_POL_ACTIVE_LOW 
		& SPI_CKE_OFF & MASTER_ENABLE_ON & SEC_PRESCAL_1_1 & PRI_PRESCAL_1_1, 0x00, SPI_ENABLE
	);
	
	spi_enable();

	io_unlock();

	lcd_power_on();
	lcd_change_font_size(SINGLE_ALL);


	//Enable the low power oscillator
  	OSCCONBITS new_osccon;
  	new_osccon = OSCCONbits;    /* save old OSCCON */
	new_osccon.SOSCEN = 1; 		//enable secondary oscillator
  	set_osccon(new_osccon);
    
 	io_unlock();
  	RCONbits.VREGS = 0;    /* Disable regulator in sleep mode */

    char revision[16] = {0};
    sprintf(revision, "Version: Rev. %c", FIRMWARE_REVISION);
    
	lcd_print_line(lcd_TOP,"Initialising");
	lcd_print_line(lcd_MIDDLE,revision);
	
	//long timer1;
	//De-passivation of battery ~10second delay at full power
    //when it has been in storage it will report low battery other wise
	//for(timer1 = 0; timer1 < 4000000;timer1++){
	//	Nop();
	//}
	
	lcd_print_line(lcd_TOP,"NavCompass  ");
	delay_ticks(1048);


	//start countdown timer to turn off LCD and put PIC into low power (37uA) mode
  	start_timer1();
 
	//Set up timeout
	timeout = timeout_ticks;

	//Enable Real Time Clock
	RCFGCALbits.RTCEN = 1;
	update_screen();	//Update the clock on the screen

	



}//end InitializeSystem





           
/** EOF main.c ***************************************************************/

