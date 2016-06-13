; A function for writing to the dsPIC OSCCON SFR.
; Copyright (C) 2005 by Temperature Technology
; Written by John Steele Scott <john@t-tec.com.au>

 .include "p24FJ256GB106.inc"
 .global _set_osccon
 
; Write the word in W0 to OSCCON.
; The high byte should be written first; this allows the clock source to be 
; changed with only one call to this function.
_set_osccon:
 ; disable interrupts until we have written the low byte
 DISI   #12
 ; address of OSCCON
 MOV.W  #OSCCON, W3

 ; shift high byte into position in W4
 LSR    W0, #8, W4

 ; magic bytes for writing high byte
 MOV.B  #0x78, W1
 MOV.B  #0x9A, W2
 MOV.B  W1, [W3+1]
 MOV.B  W2, [W3+1]

 ; write high byte
 MOV.B  W4, [W3+1]

 ; magic bytes for writing low byte
 MOV.B  #0x46, W1
 MOV.B  #0x57, W2
 MOV.B  W1, [W3+0]
 MOV.B  W2, [W3+0]
 ; write low byte
 MOV.B  W0, [W3+0]
 
 RETURN
