﻿using System;
using System.Diagnostics;
using Lunggo.ApCommon.Flight.Service;
using Lunggo.Framework.Mail;
using Microsoft.Azure.WebJobs;

namespace Lunggo.WebJob.EmailQueueHandler.Function
{
    public partial class ProcessEmailQueue
    {
        public static void FlightInstantPaymentReservationNotifEmail([QueueTrigger("flightinstantpaymentreservationnotifemail")] string rsvNo)
        {
            var flightService = FlightService.GetInstance();
            var sw = new Stopwatch();
            Console.WriteLine("Processing Flight Instant Payment Reservation Notif Email for RsvNo " + rsvNo + "...");

            Console.WriteLine("Getting Required Data...");
            sw.Start();
            var reservation = flightService.GetOverviewReservation(rsvNo);
            sw.Stop();
            Console.WriteLine("Done Getting Required Data. (" + sw.Elapsed.TotalSeconds + "s)");
            sw.Reset();

            var mailService = MailService.GetInstance();
            var mailModel = new MailModel
            {
                RecipientList = new[] {reservation.Contact.Email},
                Subject = "[Travorama] Harap Selesaikan Pembayaran Anda - No. Pemesanan " + reservation.RsvNo,
                FromMail = "booking@travorama.com",
                FromName = "Travorama"
            };
            Console.WriteLine("Sending Notification Email...");
            mailService.SendEmail(reservation, mailModel, "FlightPendingPaymentNotifEmail");
            
            Console.WriteLine("Done Processing Flight Instant Payment Reservation Notif Email for RsvNo " + rsvNo);
        }
    }
}