﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Lunggo.ApCommon.Hotel.Model;
using Lunggo.ApCommon.Hotel.Model.Logic;
using Lunggo.Framework.Documents;
using Microsoft.Azure.Documents;

namespace Lunggo.ApCommon.Hotel.Service
{
    public partial class HotelService
    {
        private decimal NetFare = 0;
        private decimal OriginalPrice = 0;
        public GetHotelDetailOutput GetHotelDetail(GetHotelDetailInput input)
        {
            var hotelDetail = GetHotelDetailFromDb(input.HotelCode);
            SetHotelFullFacilityCode(hotelDetail);
            SetDetailFromSearchResult(hotelDetail, input.SearchId);
            hotelDetail.Rooms = SetRoomPerRate(hotelDetail.Rooms);
            return new GetHotelDetailOutput
            {
                HotelDetail = ConvertToHotelDetailsBaseForDisplay(hotelDetail,OriginalPrice,NetFare)
            };
        }

        public HotelDetailsBase GetHotelDetailFromDb (int hotelCode)
        {
            /*Technologies using DocDB*/
            //var document = DocumentService.GetInstance();
            //return document.Retrieve<HotelDetailsBase>("HotelDetail:"+hotelCode);

            /*Technpologies using TableStorage*/
            return GetHotelDetailFromTableStorage(hotelCode);
        }

        public void SetDetailFromSearchResult(HotelDetailsBase hotel ,string searchId)
        {
            var searchResultData = GetSearchHotelResultFromCache(searchId);
            var searchResulthotel = searchResultData.HotelDetails.SingleOrDefault(p => p.HotelCode == hotel.HotelCode);
            hotel.Rooms = searchResulthotel.Rooms;
            foreach (var room in hotel.Rooms)
            {
                room.Images = hotel.ImageUrl.Where(x=>x.Type == "HAB").Select(x => x.Path).ToList();
            }
            SetRegIdsAndTnc(hotel.Rooms, searchResulthotel.CheckInDate, hotel.HotelCode);
            OriginalPrice = searchResulthotel.OriginalFare;
            NetFare = searchResulthotel.NetFare;
        }

        public void SetHotelFullFacilityCode(HotelDetailsBase hotel)
        {
            foreach (var data in hotel.Facilities)
            {
                data.FullFacilityCode = data.FacilityGroupCode + "" + data.FacilityCode;
            }
        }
        public List<HotelRoom> SetRoomPerRate(List<HotelRoom> hotelRoom)
        {
            var roomList = new List<HotelRoom>();
            foreach (var room in hotelRoom)
            {
                foreach (var rate in room.Rates)
                {
                    var singleRoom = new HotelRoom
                    {
                        RoomCode = room.RoomCode,
                        RoomName = room.RoomName,
                        Type = room.Type,
                        TypeName = room.TypeName,
                        Images = room.Images,
                        Facilities = room.Facilities,
                        characteristicCd = room.characteristicCd,
                        SingleRate = rate
                    };
                    roomList.Add(singleRoom);
                }
            }
            return roomList;
        }

    }
}
