/*
 *   RTC (Real time clock) module via a PF30f3013
 *   Copyright (C) 2005, 2007, 2008 by Temperature Technology
 *   Written by Roland Harrison <rollie@internode.on.net>
 */

#ifndef RTC_H
#define RTC_H

void rtc_get_time(int * year_out, int * month_out, int * day_out,
                  int * hour_out, int * min_out, int * sec_out);
void rtc_set_time(int years, int months, int days,
                  int hours, int mins, int secs);

#endif
