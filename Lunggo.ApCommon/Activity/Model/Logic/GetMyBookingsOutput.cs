﻿using System.Collections.Generic;

namespace Lunggo.ApCommon.Activity.Model.Logic
{
    public class GetMyBookingsOutput
    {
        public List<CartList> MyBookings { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
    }
}
