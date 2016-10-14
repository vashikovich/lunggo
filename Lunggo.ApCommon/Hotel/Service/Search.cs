﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lunggo.ApCommon.Hotel.Model;
using Lunggo.ApCommon.Hotel.Model.Logic;
using Lunggo.ApCommon.Hotel.Query;
using Lunggo.ApCommon.Hotel.Wrapper.HotelBeds;
using Lunggo.Framework.Documents;
using Lunggo.Framework.SharedModel;

namespace Lunggo.ApCommon.Hotel.Service
{

    public partial class HotelService
    {
        public SearchHotelOutput Search(SearchHotelInput input)
        {
            if (input.SearchId != null)
            {
                ////Take search result from docDB
                //var searchResult = GetSearchHotelResultWithFilter(input);

                //Take search data from Redis
                var searchResult = GetSearchHotelResultFromCache(input.SearchId);

                var hotels = searchResult.HotelDetails;
                var hasil = searchResult.HotelDetails;
                
                //DO Filtering
                if (hotels != null && input.FilterParam != null)
                {
                    hotels = searchResult.HotelDetails.Where(p =>
                    (input.FilterParam.Area == null || input.FilterParam.Area.Contains(p.ZoneCode)) &&
                    (input.FilterParam.StarRating == null || input.FilterParam.StarRating.Contains(p.StarRating)) &&
                    (input.FilterParam.AccomodationType == null || input.FilterParam.AccomodationType.Contains(p.AccomodationType)) &&
                    (input.FilterParam.MaxPrice == null || input.FilterParam.MinPrice == null || (p.OriginalFare >= input.FilterParam.MinPrice && p.OriginalFare <= input.FilterParam.MaxPrice))
                    ).Select(p => new HotelDetail
                    {
                        HotelCode = p.HotelCode,
                        HotelName = p.HotelName,
                        Review = p.Review,
                        Address = p.Address,
                        PostalCode = p.PostalCode,
                        Chain = p.Chain,
                        Pois = p.Pois,
                        StarRating = p.StarRating,
                        Terminals = p.Terminals,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        PhonesNumbers = p.PhonesNumbers,
                        OriginalFare = p.OriginalFare,
                        ImageUrl = p.ImageUrl,
                        ZoneCode = p.ZoneCode,
                        SpecialRequest = p.SpecialRequest,
                        Email = p.Email,
                        Facilities = p.Facilities,
                        Segment = p.Segment,
                        Description = p.Description,
                        City = p.City,
                        CountryCode = p.CountryCode,
                        NightCount = p.NightCount,
                        Rooms = p.Rooms,
                        NetFare = p.NetFare,
                        DestinationCode = p.DestinationCode,
                        AccomodationType = p.AccomodationType,
                        Discount = p.Discount,
                    }).ToList();    
                }
                

                //Do Sorting
                if (hotels != null && input.SortingParam != null)
                {
                    if (input.SortingParam.AscendingPrice)
                    {
                        hotels = hotels.OrderBy(p => p.OriginalFare).ToList();
                    }

                    if (input.SortingParam.DescendingPrice)
                    {
                        hotels = hotels.OrderByDescending(p => p.OriginalFare).ToList();
                    }
                }

                List<HotelDetail> hotelList;
                if (input.StartPage != 0 && input.EndPage != 0)
                {
                    hotelList = hotels.Skip(input.StartPage).Take(input.EndPage).ToList();
                }
                else
                {
                    hotelList = hotels.Take(100).ToList();
                }

                return new SearchHotelOutput
                {
                    SearchId = searchResult.SearchId,
                    HotelDetailLists = ConvertToHotelDetailForDisplay(hotelList)
                };
            }
            else
            {
                //Do Call Availability
                //Save data to DocDB
                var hotelBedsClient = new HotelBedsSearchHotel();
                var result = hotelBedsClient.SearchHotel(new SearchHotelCondition
                {
                    CheckIn = input.CheckIn,
                    Checkout = input.Checkout,
                    Location = input.Location,
                    Zone = input.Zone,
                    AdultCount = input.AdultCount,
                    ChildCount = input.ChildCount,
                    Nights = input.Nights,
                    Rooms = input.Rooms
                });

                //remember to add searchId
                Guid generatedSearchId = Guid.NewGuid();
                result.SearchId = generatedSearchId.ToString();
                Debug.Print("Search Id : "+ result.SearchId);


                //Adding Additional Hotel Information
                //foreach (var hotel in result.HotelDetails)
                //{
                   //var detail = GetHotelDetailsFromDocument(hotel.HotelCode);
                //    hotel.PhonesNumbers = detail.PhonesNumbers;
                //    //hotel.Terminals = detail.Terminals != null ? detail.Terminals : null; //TODO Krena ada tambahan jadi masih ada kesalahan ya, di masukin data content ke docDB
                //    hotel.PostalCode = detail.PostalCode;
                //    hotel.Review = detail.Review;
                //    hotel.StarRating = detail.StarRating;
                //    hotel.Chain = detail.Chain;
                //    hotel.Pois = detail.Pois;
                //    hotel.Address = detail.Address;
                //    hotel.Segment = detail.Segment;
                //    hotel.PhonesNumbers = detail.PhonesNumbers;
                //    //hotel.ImageUrl = detail.ImageUrl != null ? detail.ImageUrl : null; //TODO Krena ada tambahan jadi masih ada kesalahan ya, di masukin data content ke docDB
                //    hotel.Email = detail.Email;
                //    hotel.City = detail.City;
                //    hotel.CountryCode = detail.CountryCode;
                //    hotel.ZoneCode = detail.ZoneCode;
                //    hotel.Longitude = detail.Longitude;
                //    hotel.Latitude = detail.Latitude;
                //    hotel.DestinationCode = detail.DestinationCode;
                //    hotel.Description = detail.Description;
                //    hotel.AccomodationType = detail.AccomodationType;
                //}

                if (result.HotelDetails != null)
                {
                    //save data to docDB
                    //SaveSearchResultToDocument(result);

                    //save searchResult to cache
                    SaveSearchResultintoDatabaseToCache(result.SearchId,result);


                    //return only 100 data for the first page
                    return new SearchHotelOutput
                    {
                        SearchId = result.SearchId,
                        HotelDetailLists = ConvertToHotelDetailForDisplay(result.HotelDetails).Take(100).ToList(),
                        StartPage = 1,
                        EndPage = 100,
                    };
                }
                else
                {
                    Console.WriteLine("Search result is empty");
                    return new SearchHotelOutput();
                }
            }
        }
    }
}
