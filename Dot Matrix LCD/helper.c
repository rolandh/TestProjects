/*
 *   Helper functions
 *   Copyright (C) 2008 by Temperature Technology
 *   Written by Roland Harrison <rollie@internode.on.net>
 */

#include "helper.h"

/** Helper function to convert BCD to Binary
    Takes 8 bit BCD and converts to 8bit binary
    \return The input value as a decimal. */
int bcd_to_binary(int bcd)
{
  int tens, number;
  tens = ((bcd >> 4) * 10);
  number = bcd & 0xF;
  return tens + number;
}

/** Helper function to convert binary to BCD
    \param bin A number between 0 and 99.
    \return The input value as binary coded decimal. */
int binary_to_bcd(int bin)
{
  int temp_bin = bin;
  int i = 0;
 
  for (i = bin; i >= 10; i -= 10) {
    temp_bin += 6;
  }
  return temp_bin;
}
