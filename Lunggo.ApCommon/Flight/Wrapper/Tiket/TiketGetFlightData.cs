﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Flight.Wrapper.Tiket.Model;
using Lunggo.Framework.Extension;
using RestSharp;

namespace Lunggo.ApCommon.Flight.Wrapper.Tiket
{
    internal partial class TiketWrapper
    {
        internal GetFlightDataResponse SelectFlight(string flightId, DateTime date)
        {
            return Client.GetFlightData(flightId, date);
        }

        private partial class TiketClientHandler
        {
            internal GetFlightDataResponse GetFlightData(string flightId, DateTime date)
            {
                GetToken();
                var client = CreateTiketClient();
                var url = "/flight_api/get_flight_data?flight_id="+flightId+"&token="+token+"&date="+date.ToString("yyyy-MM-dd")+"&output=json";
                var request = new RestRequest(url, Method.GET);
                var response = client.Execute(request);
                var flightData = JsonExtension.Deserialize<GetFlightDataResponse>(response.Content);
                var temp = flightData;
                if (flightData.Diagnostic.Status != "200")
                    return null;
                return flightData;
            }
        }
    }

}