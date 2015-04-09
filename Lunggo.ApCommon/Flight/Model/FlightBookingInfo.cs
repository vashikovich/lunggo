﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Lunggo.ApCommon.Flight.Model
{
    public class FlightBookingInfo
    {
        public string FareId { get; set; }
        public List<PassengerFareInfo> PassengerFareInfos { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
    }
}
