﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Lunggo.ApCommon.Flight.Service;
using Lunggo.Framework.Config;
using Lunggo.Framework.Database;
using Lunggo.Repository.TableRecord;
using Lunggo.Repository.TableRepository;

namespace Lunggo.Webjob.MystiflyQueueHandler
{
    class Program
    {
        static void Main()
        {
            Init();

            var flightService = FlightService.GetInstance();
            List<string> ticketedBookingIds;
            List<string> scheduleChangedBookingIds;
            flightService.GetAndUpdateBookingStatus(out ticketedBookingIds, out scheduleChangedBookingIds);
        }

        private static void Init()
        {
            InitConfigurationManager();
            InitFlightService();
        }

        private static void InitConfigurationManager()
        {
            var configManager = ConfigManager.GetInstance();
            configManager.Init(@"Config\");
        }

        private static void InitFlightService()
        {
            var flight = FlightService.GetInstance();
            flight.Init();
        }
    }
}
