﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.Framework.Database;
using Lunggo.Repository.TableRecord;

namespace Lunggo.ApCommon.Flight.Query
{
    internal class GetFlightTripSummaryQuery : QueryBase<GetFlightTripSummaryQuery, FlightTripTableRecord>
    {
        protected override string GetQuery(dynamic condition = null)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(CreateSelectClause());
            if (condition != null)
                queryBuilder.Append(CreateWhereClause(condition));
            return queryBuilder.ToString();
        }

        private static string CreateSelectClause()
        {
            var clauseBuilder = new StringBuilder();
            clauseBuilder.Append(@"SELECT t.TripId, t.OriginAirportCd, t.DestinationAirportCd ");
            clauseBuilder.Append(@"FROM FlightTrip AS t");
            clauseBuilder.Append(@"INNER JOIN FlightItinerary AS i ON t.ItineraryId = i.ItineraryId ");
            return clauseBuilder.ToString();
        }

        private static string CreateWhereClause(dynamic condition)
        {
            var clauseBuilder = new StringBuilder();
            clauseBuilder.Append(@"WHERE i.RsvNo = @RsvNo");
            return clauseBuilder.ToString();
        }
    }
}