﻿using System.Text;
using Lunggo.Framework.Database;

namespace Lunggo.ApCommon.Flight.Query
{
    class GetRsvNoByBookingIdQuery : DbQueryBase<GetRsvNoByBookingIdQuery, string>
    {
        protected override string GetQuery(dynamic condition = null)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append(CreateSelectClause());
            queryBuilder.Append(CreateWhereClause());
            return queryBuilder.ToString();
        }

        private static string CreateSelectClause()
        {
            var clauseBuilder = new StringBuilder();
            clauseBuilder.Append("SELECT RsvNo ");
            clauseBuilder.Append("FROM FlightItinerary ");
            return clauseBuilder.ToString();
        }

        private static string CreateWhereClause()
        {
            var clauseBuilder = new StringBuilder();
            clauseBuilder.Append("WHERE BookingId IN @BookingIds");
            return clauseBuilder.ToString();
        }
    }
}
