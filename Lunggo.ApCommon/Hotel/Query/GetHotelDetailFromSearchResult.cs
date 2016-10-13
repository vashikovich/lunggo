﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunggo.Framework.Documents;

namespace Lunggo.ApCommon.Hotel.Query
{
    internal class GetHotelDetailFromSearchResult : DocQueryBase
    {
        public override string GetQueryString(dynamic condition = null)
        {
            return "SELECT c.hotel FROM c WHERE c.searchId = @SearchId AND c.hotelCd = @HotelCode";
        }
    }
}