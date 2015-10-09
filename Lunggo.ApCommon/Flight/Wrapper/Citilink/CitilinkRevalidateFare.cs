﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CsQuery;
using Lunggo.ApCommon.Flight.Constant;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Flight.Service;

namespace Lunggo.ApCommon.Flight.Wrapper.Citilink
{
    internal partial class CitilinkWrapper
    {
        internal override RevalidateFareResult RevalidateFare(RevalidateConditions conditions)
        {
            conditions.FareId =
                "KNO.SUB.17.11.2015.3.2.1.QG.100.984000.1~N~~N~RGFR~~1~^2~P~~P~RGFR~~1~X|QG~ 831~ ~~KNO~11/17/2015 08:40~CGK~11/17/2015 10:55~^QG~ 805~ ~~CGK~11/17/2015 13:40~SUB~11/17/2015 15:00~";
            return Client.RevalidateFare(conditions);
        }

        private partial class CitilinkClientHandler
        {
            internal RevalidateFareResult RevalidateFare(RevalidateConditions conditions)
            {
                var splittedFareId = conditions.FareId.Split('.').ToList();
                var origin = splittedFareId[0];
                var dest = splittedFareId[1];
                var date = new DateTime(int.Parse(splittedFareId[4]), int.Parse(splittedFareId[3]),
                    int.Parse(splittedFareId[2]));
                var adultCount = int.Parse(splittedFareId[5]);
                var childCount = int.Parse(splittedFareId[6]);
                var infantCount = int.Parse(splittedFareId[7]);
                //var cabinClass = FlightService.ParseCabinClass(splittedFareId[8]);
                var AirlineCode = splittedFareId[8];
                var FlightNumber = splittedFareId[9];
                var price = decimal.Parse(splittedFareId[10]);
                var coreFareId = splittedFareId[11];

                var URL = @"https://book.citilink.co.id/Search.aspx?" +
                          @"DropDownListCurrency=IDR" +
                          @"&DropDownListMarketDay1=" + date.Day +
                          @"&DropDownListMarketDay2=" +
                          @"&DropDownListMarketMonth1=" + date.ToString("yyyy MMMM") +
                          @"&DropDownListMarketMonth2=" +
                          @"&DropDownListPassengerType_ADT=" + adultCount +
                          @"&DropDownListPassengerType_CHD=" + childCount +
                          @"&DropDownListPassengerType_INFANT=" + infantCount +
                          @"&OrganizationCode=QG" +
                          @"&Page=Select" +
                          @"&RadioButtonMarketStructure=OneWay" +
                          @"&TextBoxMarketDestination1=" + dest +
                          @"&TextBoxMarketOrigin1=" + origin +
                          @"&culture=id-ID";

                Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                //Headers["Accept-Encoding"] = "gzip, deflate";
                Headers["Accept-Language"] = "en-GB,en-US;q=0.8,en;q=0.6";
                Headers["Upgrade-Insecure-Requests"] = "1";
                Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
                Headers["Referer"] = "https://www.citilink.co.id/";

                var htmlRespon = DownloadString(URL);
                var cobaAmbilTable = (CQ)htmlRespon;
                var tunjukkode = cobaAmbilTable[".flight-info>span"];
                var ambilTanggal = cobaAmbilTable[".dayHeaderTodayImage>a"];
                var isi = cobaAmbilTable[".w99>tbody>tr:not([class^='trSSR'])"];
                //FareID
                var FID = coreFareId.Split('|').Last();
                var CekFID = cobaAmbilTable["[value$=" + FID + "]"];
                
                var tunjuk = CekFID.Parent().Parent().Parent();
                

                if (CekFID.Count() != 0 )
                {
                
                    //newFID
                    var newFID = CekFID.Select(x => x.Cq().Attr("value")).FirstOrDefault();
                    var ParseFID1 = newFID.Split('|').ToList();
                    var ParseFID2 = ParseFID1[1].Split('~').ToList();

                    //Airline
                    
                    var foundFareId = CekFID.Attr("value");
                    var fareIdPrefix = origin + "." + dest + "." + date.ToString("dd.MM.yyyy") + "." + adultCount + "." +
                                       childCount + "." + infantCount + "." + AirlineCode + "." + FlightNumber +"." + price;

                    Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                    //Headers["Accept-Encoding"] = "gzip, deflate";
                    Headers["Accept-Language"] = "en-GB,en-US;q=0.8,en;q=0.6";
                    Headers["Upgrade-Insecure-Requests"] = "1";
                    Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
                    Headers["Referer"] = "https://www.citilink.co.id/";
                    Headers["X-Requested-With"] = "XMLHttpRequest";
                    Headers["Host"] = "book.citilink.co.id";

                    var url =
                         "https://book.citilink.co.id/TaxAndFeeInclusiveDisplayAjax-resource.aspx?" +
                                               "flightKeys=" + newFID +
                         "&numberOfMarkets=1" +
                         "&keyDelimeter=%2C" +
                         "&ssrs=FLEX";

                    var responAjax = DownloadString(url);
                    CQ ambilDataAjax = (CQ)responAjax;
                    
                    //Price
                    var tunjukHarga = ambilDataAjax["#taxAndFeeInclusiveTotal"];
                    var ambilharga = tunjukHarga.Select(x => x.Cq().Text()).FirstOrDefault();
                    var harga = ambilharga.Split('.');
                    var newPrice = decimal.Parse(harga[1]);

                    var prefix =
                        "" + origin + "" +
                        "." + dest + "" +
                        "." + date.Day + "" +
                        "." + date.Month + "" +
                        "." + date.Year + "" +
                        "." + adultCount + "" +
                        "." + childCount + "" +
                        "." + infantCount + "" +
                        "." + ParseFID2[0] + "" +
                        "." + ParseFID2[1] + "" +
                        "." + decimal.Parse(harga[1]) + "" +
                        ".";

                    //for (int l = 0; l < ACpisah1.Count; l++)
                    //{
                    var segments = new List<FlightSegment>();
                    string Acode;
                    string Fnumber;

                    if (ParseFID2.Count > 7)
                    {
                        int i = ((ParseFID2.Count) - 1) / 8;
                        int j = 4;
                        for (int l = 0; l <i; l++)
                        {
                            if (ParseFID2[(8 * l)].Length > 2)
                            {
                                Acode = ParseFID2[(8 * l)].Substring(1, 2);

                            }
                            else
                                {
                                    Acode = ParseFID2[(8 * l)];
                                }

                            if (ParseFID2[(8 * l) + 1].Trim().Length > 3)
                            {
                                Fnumber = ParseFID2[(8 * l) + 1].Substring(0, 4).Trim();
                            }
                            else
                                {
                                    Fnumber = ParseFID2[(8 * l) + 1].Trim();
                                }
                        segments.Add(new FlightSegment
                        {
                            AirlineCode = Acode,
                            FlightNumber = Fnumber,
                            CabinClass = CabinClass.Economy,
                            Rbd = FID,
                            DepartureAirport = ParseFID2[j],
                            DepartureTime = DateTime.Parse(ParseFID2[j+1]),
                            ArrivalAirport = ParseFID2[j+2],
                            ArrivalTime = DateTime.Parse(ParseFID2[j + 3]),
                            OperatingAirlineCode = Fnumber,
                            StopQuantity = 0
                        });
                        j = j + 8;
                        }
                    }

                    //}

                    var itin = new FlightItinerary
                    {
                        AdultCount = adultCount,
                        ChildCount = childCount,
                        InfantCount = infantCount,
                        CanHold = true,
                        FareType = FareType.Published,
                        RequireBirthDate = true,
                        RequirePassport = false,
                        RequireSameCheckIn = false,
                        RequireNationality = true,
                        RequestedCabinClass = CabinClass.Economy,
                        TripType = TripType.OneWay,
                        Supplier = FlightSupplier.AirAsia,
                        SupplierCurrency = "IDR",
                        SupplierRate = 1,
                        SupplierPrice = newPrice,
                        FareId = fareIdPrefix + foundFareId,
                        FlightTrips = new List<FlightTrip>
                        {
                            new FlightTrip
                            {
                                OriginAirport = origin,
                                DestinationAirport = dest,
                                DepartureDate = date,
                                Segments = segments
                            }
                        }
                    };
                    
                    return new RevalidateFareResult
                    {
                        IsSuccess = true,
                        IsValid = price == newPrice,
                        Itinerary = itin
                    };
               }    
               else
               {
                   return new RevalidateFareResult
                   {
                       IsSuccess = true,
                       IsValid = false
                   };
               }
            }
        }
    }
}
