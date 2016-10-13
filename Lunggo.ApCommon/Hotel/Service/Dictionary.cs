﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using Lunggo.Repository.TableRecord;

namespace Lunggo.ApCommon.Hotel.Service
{
    public partial class HotelService
    {
        public class Country
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string IsoCode { get; set; }
            public List<Destination> Destinations { get; set; }
        }

        public class Destination
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public List<Zone> Zones { get; set; }
        }

        public class Zone
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public List<string> Hotel { get; set; } 
        }

        public class FacilityGroup
        {
            public int Code { get; set; }
            public string NameId { get; set; }
            public string NameEn { get; set; }
            public List<Facility> Facilities { get; set; } 

        }

        public class Facility
        {
            public int Code { get; set; }
            public string NameId { get; set; }
            public string NameEn { get; set; }

        }

        public class HotelRoomType
        {
            public string Type { get; set; }
            public string DescId { get; set; }
            public string DescEn { get; set; }
        }

        public class Room
        {
            public RoomCharacteristic RoomCharacteristic { get; set; }
            public HotelRoomType RoomType { get; set; }
            public string RoomCd { get; set; }
            public string RoomDescId { get; set; }
            public string RoomDescEn { get; set; }
            public int MinPax { get; set; }
            public int MaxPax { get; set; }
            public int MinAdult { get; set; }
            public int MaxAdult { get; set; }
            public int MinChild { get; set; }
            public int MaxChild { get; set; }

        }

        public class RoomCharacteristic
        {
            public string CharacteristicCd { get; set; }
            public string CharacteristicDescId { get; set; }
            public string CharacteristicDescEn { get; set; }
        }
        public static Dictionary<string, string> HotelSegmentDictId;
        public static Dictionary<string, string> HotelSegmentDictEng;
        public static Dictionary<int, string> HotelFacilityDictId;
        public static Dictionary<int, string> HotelFacilityDictEng;
        public static Dictionary<int, string> HotelFacilityGroupDictId;
        public static Dictionary<int, string> HotelFacilityGroupDictEng;
        public static Dictionary<int, Facility> HotelRoomFacility;


        public static Dictionary<string, Room> HotelRoomDict;
        public static Dictionary<string, HotelRoomType> HotelRoomTypeDict;
        public static Dictionary<string, RoomCharacteristic> HotelRoomCharacteristicDict;
        
        
        public static Dictionary<string, string> HotelRoomRateClassDictId;
        public static Dictionary<string, string> HotelRoomRateClassDictEng;
        public static Dictionary<string, string> HotelRoomRateTypeDictId;
        public static Dictionary<string, string> HotelRoomRateTypeDictEng;
        public static Dictionary<string, string> HotelRoomPaymentTypeDictId;
        public static Dictionary<string, string> HotelRoomPaymentTypeDictEng;
        public static Dictionary<string, string> HotelCountry;
        public static Dictionary<string, string> HotelCountryIso;
        public static Dictionary<string, string> HotelCountryIsoName;
        public static Dictionary<string, Destination> HotelDestinationDict;
        public static Dictionary<string, Country> HotelDestinationCountryDict;
        public static Dictionary<string, Zone> HotelDestinationZoneDict;

        public static List<Country> Countries;
        public static List<FacilityGroup> FacilityGroups;
        public static List<Room> Rooms; 

        private const string HotelSegmentFileName = @"HotelSegment.csv";
        private const string HotelFacilityFileName = @"HotelFacilities.csv";
        private const string HotelFacilityGroupFileName = @"HotelFacilityGroup.csv";
        private const string HotelRoomFileName = @"HotelRoom.csv";
        private const string HotelRoomRateClassFileName = @"HotelRoomRateClass.csv";
        private const string HotelRoomRateTypeFileName = @"HotelRoomRateType.csv";
        private const string HotelRoomPaymentTypeFileName = @"HotelRoomPaymentType.csv";
        private const string HotelCountryFileName = @"HotelCountries.csv";
        private const string HotelDestinationFileName = @"HotelDestinations.csv";

        private static string _hotelSegmentFilePath;
        private static string _hotelFacilitiesFilePath;
        private static string _hotelFacilityGroupFilePath;
        private static string _hotelRoomFilePath;
        private static string _hotelRoomRateClassFilePath;
        private static string _hotelRoomRateTypeFilePath;
        private static string _hotelRoomPaymentTypeFilePath;
        private static string _hotelCountriesFilePath;
        private static string _hotelDestinationsFilePath;
        private static string _configPath;

        public void InitDictionary(string folderName)
        {
            _configPath = HttpContext.Current != null
                ? HttpContext.Current.Server.MapPath(@"~/" + folderName + @"/")
                : string.IsNullOrEmpty(folderName)
                    ? ""
                    : folderName + @"\";

            _hotelSegmentFilePath = Path.Combine(_configPath, HotelSegmentFileName);
            _hotelFacilitiesFilePath = Path.Combine(_configPath, HotelFacilityFileName);
            _hotelFacilityGroupFilePath = Path.Combine(_configPath, HotelFacilityGroupFileName);
            _hotelRoomFilePath = Path.Combine(_configPath, HotelRoomFileName);
            _hotelRoomRateClassFilePath = Path.Combine(_configPath, HotelRoomRateClassFileName);
            _hotelRoomRateTypeFilePath = Path.Combine(_configPath, HotelRoomRateTypeFileName);
            _hotelRoomPaymentTypeFilePath = Path.Combine(_configPath, HotelRoomPaymentTypeFileName);
            _hotelCountriesFilePath = Path.Combine(_configPath, HotelCountryFileName);
            _hotelDestinationsFilePath = Path.Combine(_configPath, HotelDestinationFileName);

            PopulateHotelSegmentDict(_hotelSegmentFilePath);

            PopulateHotelFacilityGroupDict(_hotelFacilityGroupFilePath);
            PopulateHotelFacilityGroupList(_hotelFacilitiesFilePath);
            PopulateHotelRoomFacilityDict(FacilityGroups);

            PopulateHotelRoomList(_hotelRoomFilePath);
            PopulateHotelRoomDict(Rooms);
            PopulateHotelRoomTypeDict(Rooms);
            PopulateHotelRoomCharacteristicDict(Rooms);

            PopulateHotelRoomRateClassDict(_hotelRoomRateClassFilePath);
            PopulateHotelRoomRateTypeDict(_hotelRoomRateTypeFilePath);
            PopulateHotelRoomPaymentTypeDict(_hotelRoomPaymentTypeFilePath);
            
            PopulateHotelCountriesDict(_hotelCountriesFilePath);

            PopulateHotelDestinationList(_hotelDestinationsFilePath);
            PopulateHotelDestinationCountryDict(Countries);
            PopulateHotelDestinationDict(Countries);
            PopulateHotelZoneDict(Countries);
        }

        //POPULATE METHODS REGARDING HOTEL SEGMENT
        private static void PopulateHotelSegmentDict(String hotelSegmentFilePath)
        {
            HotelSegmentDictEng = new Dictionary<string, string>();
            HotelSegmentDictId = new Dictionary<string, string>();

            using (var file = new StreamReader(hotelSegmentFilePath))
            {
                var line = file.ReadLine();
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    var splittedLine = line.Split('|');
                    HotelSegmentDictEng.Add(splittedLine[0],splittedLine[1]);
                    HotelSegmentDictId.Add(splittedLine[0], splittedLine[2]);
                }
            }
            
        }

        //POPULATE METHODS REGARDING FACILITY
        private static void PopulateHotelFacilityGroupList(string hotelFacilitiesFilePath)
        {
            FacilityGroups = new List<FacilityGroup>();

            using (var file = new StreamReader(hotelFacilitiesFilePath))
            {
                var line = file.ReadLine();
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    var splittedLine = line.Split('|');
                    var foundFacilityGroup = FacilityGroups.Where(g => g.Code == Convert.ToInt32(splittedLine[0])/1000).ToList();
                    if (foundFacilityGroup.Count == 0)
                    {
                        var newFacilityGroup = new FacilityGroup
                        {
                            Code = Convert.ToInt32(splittedLine[0]) / 1000,
                            NameEn =
                                HotelService.GetInstance()
                                    .GetHotelFacilityGroupEng(Convert.ToInt32(splittedLine[0]) / 1000),
                            NameId =
                                HotelService.GetInstance()
                                    .GetHotelFacilityGroupId(Convert.ToInt32(splittedLine[0]) / 1000),
                            Facilities = new List<Facility>
                            {
                                new Facility
                                {
                                    Code = Convert.ToInt32(splittedLine[0]) % 1000,
                                    NameEn = splittedLine[1],
                                    NameId = splittedLine[2],
                                }
                            }
                        };

                        FacilityGroups.Add(newFacilityGroup);
                    }
                    else
                    {
                        var foundFacility =
                            foundFacilityGroup[0].Facilities.Where(f => f.Code == Convert.ToInt32(splittedLine[0])/1000)
                                .ToList();
                        if (foundFacility.Count == 0)
                        {
                            var newFacility = new Facility
                            {
                                Code = Convert.ToInt32(splittedLine[0]) % 1000,
                                NameEn = splittedLine[1],
                                NameId = splittedLine[2],
                            };
                            FacilityGroups.Where(g => g.Code == Convert.ToInt32(splittedLine[0])/1000).
                                ToList()[0].Facilities.Add(newFacility);
                        }
                    }
                }
            }
        }

        private static void PopulateHotelFacilityGroupDict(String hotelFacilityGroupFilePath)
        {
            HotelFacilityGroupDictEng = new Dictionary<int, string>();
            HotelFacilityGroupDictId = new Dictionary<int, string>();

            using (var file = new StreamReader(hotelFacilityGroupFilePath))
            {
                var line = file.ReadLine();
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    var splittedLine = line.Split('|');
                    HotelFacilityGroupDictEng.Add(Convert.ToInt32(splittedLine[0]), splittedLine[1]);
                    HotelFacilityGroupDictId.Add(Convert.ToInt32(splittedLine[0]), splittedLine[2]);
                }
            }
        }

        private static void PopulateHotelRoomFacilityDict(List<FacilityGroup> facilityGroups)
        {
            HotelRoomFacility = new Dictionary<int, Facility>();

            foreach (var facilityGroup in facilityGroups.Where(f => f.Code == 60).ToList())
            {
                foreach (var fac in facilityGroup.Facilities)
                {
                    HotelRoomFacility.Add(fac.Code, fac);
                }
            }
        }

        //POPULATE METHODS REGARDING HOTEL ROOM

        private static void PopulateHotelRoomList(string hotelRoomFilePath)
        {
            Rooms = new List<Room>();
            using (var file = new StreamReader(hotelRoomFilePath))
            {
                var line = file.ReadLine();
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    var splittedLine = line.Split('|');
                    var newHotelRoom = new Room
                    {
                        RoomCharacteristic = new RoomCharacteristic
                        {
                            CharacteristicCd = splittedLine[2],
                            CharacteristicDescEn = splittedLine[10],
                            CharacteristicDescId = splittedLine[13]
                        },
                        RoomType = new HotelRoomType
                        {
                            Type = splittedLine[1],
                            DescEn = splittedLine[9],
                            DescId = splittedLine[12]
                        },
                        MinPax = Convert.ToInt32(splittedLine[3]),
                        MaxPax = Convert.ToInt32(splittedLine[4]),
                        MaxAdult = Convert.ToInt32(splittedLine[5]),
                        MaxChild = Convert.ToInt32(splittedLine[6]),
                        MinAdult = Convert.ToInt32(splittedLine[7]),
                        RoomCd = splittedLine[0],
                        RoomDescEn = splittedLine[8],
                        RoomDescId = splittedLine[11]
                    };
                    Rooms.Add(newHotelRoom);
                }
            }
        }


        private static void PopulateHotelRoomDict(List<Room> rooms )
        {
            HotelRoomDict = new Dictionary<string, Room>();

            foreach (var room in rooms)
            {
                HotelRoomDict.Add(room.RoomCd, room);
            }
            
        }

        private static void PopulateHotelRoomTypeDict(List<Room> rooms )
        {
            HotelRoomTypeDict = new Dictionary<string, HotelRoomType>();

            foreach (var room in rooms)
            {
                HotelRoomType x;
                if (!HotelRoomTypeDict.TryGetValue(room.RoomType.Type, out x))
                {
                    HotelRoomTypeDict.Add(room.RoomType.Type, room.RoomType);
                }
            }
        }

        private static void PopulateHotelRoomCharacteristicDict(List<Room> rooms)
        {
            HotelRoomCharacteristicDict= new Dictionary<string, RoomCharacteristic>();
            foreach (var room in rooms)
            {
                RoomCharacteristic x;
                if (!HotelRoomCharacteristicDict.TryGetValue(room.RoomCharacteristic.CharacteristicCd, out x))
                {
                    HotelRoomCharacteristicDict.Add(room.RoomCharacteristic.CharacteristicCd, room.RoomCharacteristic);
                }
            }
            
        }

        private static void PopulateHotelRoomRateClassDict(String hotelRoomRateClassFilePath)
        {
            HotelRoomRateClassDictEng = new Dictionary<string, string>();
            HotelRoomRateClassDictId = new Dictionary<string, string>();

            using (var file = new StreamReader(hotelRoomRateClassFilePath))
            {
                var line = file.ReadLine();
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    var splittedLine = line.Split('|');
                    
                    HotelRoomRateClassDictEng.Add(splittedLine[0], splittedLine[1]);
                    HotelRoomRateClassDictId.Add(splittedLine[0], splittedLine[2]);
                }
            }
        }

        private static void PopulateHotelRoomRateTypeDict(String hotelRoomRateTypeFilePath)
        {
            HotelRoomRateTypeDictEng = new Dictionary<string, string>();
            HotelRoomRateTypeDictId = new Dictionary<string, string>();

            using (var file = new StreamReader(hotelRoomRateTypeFilePath))
            {
                var line = file.ReadLine();
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    var splittedLine = line.Split('|');

                    HotelRoomRateTypeDictEng.Add(splittedLine[0], splittedLine[1]);
                    HotelRoomRateTypeDictId.Add(splittedLine[0], splittedLine[2]);
                }
            }
        }

        private static void PopulateHotelRoomPaymentTypeDict(String hotelRoomPaymentTypeFilePath)
        {
            HotelRoomPaymentTypeDictEng = new Dictionary<string, string>();
            HotelRoomPaymentTypeDictId = new Dictionary<string, string>();

            using (var file = new StreamReader(hotelRoomPaymentTypeFilePath))
            {
                var line = file.ReadLine();
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    var splittedLine = line.Split('|');

                    HotelRoomPaymentTypeDictEng.Add(splittedLine[0], splittedLine[1]);
                    HotelRoomPaymentTypeDictId.Add(splittedLine[0], splittedLine[2]);
                }
            }
        }

        private static void PopulateHotelCountriesDict(String hotelCountriesFilePath)
        {
            HotelCountry = new Dictionary<string, string>();
            HotelCountryIso = new Dictionary<string, string>();
            HotelCountryIsoName = new Dictionary<string, string>();

            using (var file = new StreamReader(hotelCountriesFilePath))
            {
                var line = file.ReadLine();
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    var splittedLine = line.Split('|');
                    string x;
                    HotelCountryIso.Add(splittedLine[0], splittedLine[1]);
                    HotelCountry.Add(splittedLine[0], splittedLine[2]);
                    if (!HotelCountryIsoName.TryGetValue(splittedLine[1], out x))
                    {
                        HotelCountryIsoName.Add(splittedLine[1], splittedLine[2]);
                    }
                }
            }
        }

        //POPULATE METHODS REGARDING DESTINATION AND ZONE
        private static void PopulateHotelDestinationList(String hotelDestinationsFilePath)
        {
            Countries = new List<Country>();
            using (var file = new StreamReader(hotelDestinationsFilePath))
            {
                var line = file.ReadLine();
                while (!file.EndOfStream)
                {
                    line = file.ReadLine();
                    var splittedLine = line.Split('|');
                    var foundCountry = Countries.Where(c => c.Code == splittedLine[2]).ToList();
                    if (foundCountry.Count == 0)
                    {
                        var newCountry = new Country
                        {
                            Code = splittedLine[2],
                            Name = HotelService.GetInstance().GetHotelCountryNameByCode(splittedLine[2]),
                            IsoCode = HotelService.GetInstance().GetHotelCountryIsoCode(splittedLine[2]),
                            Destinations = new List<Destination>
                            {
                                new Destination
                                {
                                    Code = splittedLine[0],
                                    Name = splittedLine[1],
                                    Zones = new List<Zone>
                                    {
                                        new Zone
                                        {
                                            Code = splittedLine[0] + "-" + splittedLine[4],
                                            Name = splittedLine[5]
                                        }
                                    }
                                }
                            }
                        };
                        Countries.Add(newCountry);
                        
                    }
                    else
                    {
                        var foundDestination = foundCountry[0].Destinations.Where(d => d.Code == splittedLine[0]).ToList();
                        if (foundDestination.Count == 0)
                        {
                            var newDestination = new Destination
                            {
                                Code = splittedLine[0],
                                Name = splittedLine[1],
                                Zones = new List<Zone>
                                {
                                    new Zone
                                    {
                                        Code = splittedLine[0] + "-" + splittedLine[4],
                                        Name = splittedLine[5]
                                    }
                                }
                            };
                            Countries.Where(c => c.Code == splittedLine[2]).ToList()[0].Destinations.Add(newDestination);
                        }
                        else
                        {
                            var foundZone = foundDestination.Where(d => d.Code == splittedLine[0] + "-" + splittedLine[4]).ToList();
                            if (foundZone.Count == 0)
                            {
                                var newZone = new Zone
                                {
                                    Code = splittedLine[0] + "-" + splittedLine[4],
                                    Name = splittedLine[5]
                                };
                                Countries.Where(c => c.Code == splittedLine[2]).ToList()[0].Destinations.Where(d => d.Code == splittedLine[0]).ToList()[0].Zones.Add(newZone);
                            }
                        }

                        //Countries.Add(foundCountry[0]);
                    }

                    
                }
            }
        }

        private static void PopulateHotelDestinationCountryDict(List<Country> countries)
        {
            HotelDestinationCountryDict = new Dictionary<string, Country>();
            foreach (var country in countries)
            {
                HotelDestinationCountryDict.Add(country.Code, country);
            }
        }

        private static void PopulateHotelDestinationDict(List<Country> countries)
        {
            HotelDestinationDict = new Dictionary<string, Destination>();
            foreach (var destination in countries.SelectMany(country => country.Destinations))
            {
                HotelDestinationDict.Add(destination.Code, destination);
            }
        }

        private static void PopulateHotelZoneDict(List<Country> countries)
        {
            HotelDestinationZoneDict = new Dictionary<string, Zone>();
            foreach (var zone in countries.SelectMany(country => country.Destinations).SelectMany(destination => destination.Zones))
            {
                HotelDestinationZoneDict.Add(zone.Code, zone);
            }
        }

        //GET METHODS REGARDING SEGMENT
        public string GetHotelSegmentId(string code)
        {
            try
            {
                return HotelSegmentDictId[code];
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelSegmentEng(string code)
        {
            try
            {
                return HotelSegmentDictEng[code];
            }
            catch
            {
                return "";
            }
        }

        //GET METHODS REGARDING FACILITY
        public Facility GetHotelFacility(int code)
        {
            try
            {
                return FacilityGroups.Where(g => g.Code == code/1000).ToList()[0].
                    Facilities.Where(f => f.Code == code % 1000).ToList()[0];
            }
            catch
            {
                return new Facility();
            }
        }
        public string GetHotelFacilityDescId(int code)
        {
            try
            {
                return FacilityGroups.Where(g => g.Code == code/1000).ToList()[0].
                    Facilities.Where(f => f.Code == code%1000).ToList()[0].NameId;
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelFacilityDescEn(int code)
        {
            try
            {
                return FacilityGroups.Where(g => g.Code == code / 1000).ToList()[0].
                    Facilities.Where(f => f.Code == code % 1000).ToList()[0].NameEn;
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelFacilityGroupId(int code)
        {
            try
            {
                return HotelFacilityGroupDictId[code];
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelFacilityGroupEng(int code)
        {
            try
            {
                return HotelFacilityGroupDictEng[code];
            }
            catch
            {
                return "";
            }
        }
        public Facility GetHotelRoomFacility(int code)
        {
            try
            {
                return HotelRoomFacility[code];
            }
            catch
            {
                return new Facility();
            }
        }
        public string GetHotelRoomFacilityDescId(int roomFacilityCd)
        {
            try
            {
                return FacilityGroups.Where(g => g.Code == 60).ToList()[0].
                    Facilities.Where(f => f.Code == roomFacilityCd).ToList()[0].NameId;
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelRoomFacilityDescEn(int roomFacilityCd)
        {
            try
            {
                return FacilityGroups.Where(g => g.Code == 60).ToList()[0].
                    Facilities.Where(f => f.Code == roomFacilityCd).ToList()[0].NameEn;
            }
            catch
            {
                return "";
            }
        }
        
        //GET METHODS REGARDING HOTEL ROOM
        public Room GetHotelRoom(string code)
        {
            try
            {
                return Rooms.Where(r => r.RoomCd == code).ToList()[0];
            }
            catch
            {
                return new Room();
            }
        }
        public string GetHotelRoomDescEn(String cd)
        {
            try
            {
                return Rooms.Where(r => r.RoomCd == cd).ToList()[0].RoomDescEn;
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelRoomDescId(String cd)
        {
            try
            {
                return Rooms.Where(r => r.RoomCd == cd).ToList()[0].RoomDescId;
            }
            catch
            {
                return "";
            }
        }
        public HotelRoomType GetHotelRoomType(string code)
        {
            try
            {
                return HotelRoomTypeDict[code];
            }
            catch
            {
                return new HotelRoomType();
            }
        }
        public string GetHotelRoomTypeDescEn(String cd)
        {
            try
            {
                return HotelRoomTypeDict[cd].DescEn;
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelRoomTypeDescId(String cd)
        {
            try
            {
                return HotelRoomTypeDict[cd].DescId;
            }
            catch
            {
                return "";
            }
        }
        public RoomCharacteristic GetHotelRoomCharacteristic(string code)
        {
            try
            {
                return HotelRoomCharacteristicDict[code];
            }
            catch
            {
                return new RoomCharacteristic();
            }
        }
        public string GetHotelRoomCharacteristicDescEn(string code)
        {
            try
            {
                return HotelRoomCharacteristicDict[code].CharacteristicDescEn;
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelRoomCharacteristicDescId(string code)
        {
            try
            {
                return HotelRoomCharacteristicDict[code].CharacteristicDescId;
            }
            catch
            {
                return "";
            }
        }

        //
        public string GetHotelRoomRateClassId(string code)
        {
            try
            {
                return HotelRoomRateClassDictId[code];
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelRoomRateClassEng(string code)
        {
            try
            {
                return HotelRoomRateClassDictEng[code];
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelRoomRateTypeId(string code)
        {
            try
            {
                return HotelRoomRateTypeDictId[code];
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelRoomRateTypeEng(string code)
        {
            try
            {
                return HotelRoomRateTypeDictEng[code];
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelRoomPaymentTypeId(string code)
        {
            try
            {
                return HotelRoomPaymentTypeDictId[code];
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelRoomPaymentTypeEng(string code)
        {
            try
            {
                return HotelRoomPaymentTypeDictEng[code];
            }
            catch
            {
                return "";
            }
        }

        //
        public string GetHotelCountryNameByCode(string code)
        {
            try
            {
                return HotelCountry[code];
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelCountryIsoCode(string code)
        {
            try
            {
                return HotelCountryIso[code];
            }
            catch
            {
                return "";
            }
        }
        public string GetHotelCountryNameByIsoCode(string isoCode)
        {
            try
            {
                return HotelCountryIsoName[isoCode];
            }
            catch
            {
                return "";
            }
        }

        //GET METHODS REGARDING DESTINATION AND ZONE
        public Country GetHotelCountryFromMasterList(string countryCode)
        {
            try
            {
                return Countries.Where(c=> c.Code == countryCode).ToList()[0];
            }
            catch
            {
                return new Country();
            }
        }

        public Country GetHotelCountryFromDict(string countryCode)
        {
            try
            {
                return HotelDestinationCountryDict[countryCode];
            }
            catch
            {
                return new Country();
            }
        }

        public Destination GetHotelDestinationFromDict(string destinationCode)
        {
            try
            {
                return HotelDestinationDict[destinationCode];
            }
            catch
            {
                return new Destination();
            }
        }

        public Zone GetHotelZoneFromDict(string zoneCode)
        {
            try
            {
                return HotelDestinationZoneDict[zoneCode];
            }
            catch
            {
                return new Zone();
            }
        }
    }

    

    
}