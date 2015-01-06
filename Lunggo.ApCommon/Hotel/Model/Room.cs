﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunggo.ApCommon.Model;

namespace Lunggo.ApCommon.Hotel.Model
{

    public abstract class RoomPackageBase
    {
        public long PackageId { get; set; }
        public Price FinalPackagePrice { get; set; }
    }

    public abstract class RoomBase
    {
        public String RoomName { get; set; }
        public long RoomId { get; set; }
        public int AdultCount { get; set; }
        public int ChildrenCount { get; set; }
        public String RoomDescription { get; set; }
        public Price FinalRoomPrice { get; set; }
    }

    public class RoomPackage : RoomPackageBase
    {
        public IEnumerable<Room> RoomList { get; set; }
    }

    public class RoomPackageExcerpt : RoomPackageBase
    {
        public IEnumerable<RoomExcerpt> RoomList { get; set; }
    }

    public class Room : RoomBase
    {
        
    }

    public class RoomExcerpt : RoomBase
    {
        
    }
}