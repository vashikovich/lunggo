﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls.WebParts;
using Lunggo.ApCommon.Constant;
using Lunggo.ApCommon.Flight.Constant;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Flight.Model.Logic;
using Lunggo.ApCommon.Flight.Query;
using Lunggo.ApCommon.Flight.Query.Logic;
using Lunggo.ApCommon.Flight.Query.Model;
using Lunggo.ApCommon.Sequence;
using Lunggo.Framework.Database;
using Lunggo.Repository.TableRecord;
using Lunggo.Repository.TableRepository;

namespace Lunggo.ApCommon.Flight.Service
{
    public partial class FlightService
    {
        public FlightReservation GetReservation(string rsvNo)
        {
            return GetFlightDb.Reservation(rsvNo);
        }

        public List<FlightReservation> GetReservations(string contactEmail)
        {
            return GetFlightDb.Reservations(contactEmail).ToList();
        }
    }
}
