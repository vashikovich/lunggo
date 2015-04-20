﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunggo.ApCommon.Hotel.Model
{
    public class RoomDetailForBooking
    {

        public String RoomId { get; set; }
        public String RoomBasis { get; set; }
        public String RoomClass { get; set; }
        public String RoomType { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public bool Available { get; set; }
    }

}