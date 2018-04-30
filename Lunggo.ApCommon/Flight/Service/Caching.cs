﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Lunggo.ApCommon.Constant;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Product.Model;
using Lunggo.ApCommon.Sequence;
using Lunggo.ApCommon.Payment.Model;
using Lunggo.Framework.Config;
using Lunggo.Framework.Extension;
using Lunggo.Framework.Redis;
using StackExchange.Redis;

namespace Lunggo.ApCommon.Flight.Service
{
    public partial class FlightService
    {
        private const int SupplierIndexCap = 333;

        public List<Pax> GetSavedPassengers(string contactEmail)
        {
            return GetSavedPassengersFromDb(contactEmail);
        }

        public void SetLowestPriceToCache(List<FlightItineraryForDisplay> itins, string origin, string destination)
        {
            var keyRoute = SetRoute(origin, destination);
            var keyDate = SetDate(itins);
            var lowestvalue = GetLowestPrice(itins);
            Console.WriteLine("Lowest value for route: " + keyRoute + keyDate + " is " + lowestvalue);
            var redis = RedisService.GetInstance();
            var redisDb = redis.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {
                
                    redisDb.HashSet(keyRoute, keyDate, Convert.ToString(lowestvalue));
                    break;
                
            }

        }

        private void SetLowestPriceToCache(List<FlightItineraryForDisplay> itins, string origin, string destination,
            DateTime date)
        {
            var keyRoute = SetRoute(origin, destination);
            var keyDate = SetDate(date);
            var lowestvalue = GetLowestPrice(itins);
            var redis = RedisService.GetInstance();
            var redisDb = redis.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {
               
                    redisDb.HashSet(keyRoute, keyDate, Convert.ToString(lowestvalue));
                    break;
                
                
            }

        }


        //public decimal GetLowestPriceFromCache(List<FlightItinerary> itins)
        //{
        //    var keyRoute = SetRoute(itins);
        //    var keyDate = SetDate(itins);
        //    var redis = RedisService.GetInstance();
        //    var redisDb = redis.GetDatabase(ApConstant.SearchResultCacheName);
        //    return (decimal)redisDb.HashGet(keyRoute, keyDate);
        //}

        public decimal GetLowestPriceFromCache(string origin, string destination, DateTime date)
        {
            var keyRoute = SetRoute(origin, destination);
            var keyDate = SetDate(date);
            var redis = RedisService.GetInstance();
            var redisDb = redis.GetDatabase(ApConstant.SearchResultCacheName);
            Decimal dec = -1;
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {
                
                    dec = Convert.ToDecimal(redisDb.HashGet(keyRoute, keyDate));
                    return dec;
                
            }
            return dec;
        }

        public List<decimal> GetLowestPricesForRangeOfDate(string origin, string destination, DateTime startingTime,
            DateTime endTime)
        {
            var keyRoute = SetRoute(origin, destination);
            var listofDates = new List<string>();
            for (var date = startingTime.Date; date <= endTime; date = date.AddDays(1))
            {
                listofDates.Add(date.ToString("ddMMyy", CultureInfo.InvariantCulture));
            }
            var redis = RedisService.GetInstance();
            var redisDb = redis.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {
                
                    var values = redisDb.HashGet(keyRoute, Array.ConvertAll(listofDates.ToArray(), item => (RedisValue)item)).ToList();
                    return values.Select(value => Convert.ToDecimal(value)).ToList();
                
            }
            return new List<decimal>();

        }

        public LowestPrice GetLowestPriceInRangeOfDate(string origin, string destination, DateTime startDate,
            DateTime endDate)
        {
            var keyRoute = SetRoute(origin, destination);
            var listofDates = new List<string>();
            for (var date = startDate.Date; date <= endDate; date = date.AddDays(1))
            {
                listofDates.Add(date.ToString("ddMMyy", CultureInfo.InvariantCulture));
            }
            var redis = RedisService.GetInstance();
            var redisDb = redis.GetDatabase(ApConstant.SearchResultCacheName);
            var values = new List<RedisValue>();
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {
                
                    values = redisDb.HashGet(keyRoute, Array.ConvertAll(listofDates.ToArray(), item => (RedisValue)item)).ToList();
                    break;
                
            }

            if (values.Count == 0)
            {
                return new LowestPrice();
            }

            var listOfPrices = values.Select(value => Convert.ToDecimal(value)).ToList();
            var minPrice = listOfPrices.ElementAt(0);
            var minDate = listofDates.ElementAt(0);
            for (var ind = 1; ind < listOfPrices.Count; ind++)
            {
                if (listOfPrices.ElementAt(ind) >= minPrice) continue;
                minPrice = listOfPrices.ElementAt(ind);
                minDate = listofDates.ElementAt(ind);
            }

            return new LowestPrice
            {
                CheapestDate = minDate,
                CheapestPrice = minPrice
            };
        }

        public void SaveTransacInquiryInCache(string mandiriCacheId, List<KeyValuePair<string, string>> transaction, TimeSpan timeout)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "mandiriTransactionPrice:" + mandiriCacheId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {
                
                    foreach (var pair in transaction)
                    {
                        redisDb.HashSet(redisKey, pair.Key, pair.Value);
                        redisDb.KeyExpire(redisKey, timeout);
                    }
                    return;
                
            }

        }

        public List<KeyValuePair<string, string>> GetTransacInquiryFromCache(string mandiriCacheId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "mandiriTransactionPrice:" + mandiriCacheId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {
                

                    var temp = redisDb.HashGetAll(redisKey).ToList();
                    return temp.Count != 0
                        ? temp.Select(hashEntry => new KeyValuePair<string, string>(hashEntry.Name, hashEntry.Value)).ToList()
                        : null;
                
                
            }

            return new List<KeyValuePair<string, string>>();
        }

        public TimeSpan GetRedisExpiry(string key)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "mandiriTransactionPrice:" + key;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {
                
                    var timeToLive = redisDb.KeyTimeToLive(redisKey).GetValueOrDefault();
                    return timeToLive;
                
                
            }
            return TimeSpan.Zero;
        }

        private static bool GetSearchingStatusInCache(string searchId, int supplierIndex)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "searchFlightStatus:" + searchId + ":" + supplierIndex;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {
                
                    var redisTransaction = redisDb.CreateTransaction();
                    redisTransaction.AddCondition(Condition.KeyNotExists(redisKey));
                    redisTransaction.StringSetAsync(redisKey, true, TimeSpan.FromMinutes(5));
                    var currentStatusTask = redisTransaction.StringGetAsync(redisKey);
                    redisTransaction.Execute();
                    return currentStatusTask.Status == TaskStatus.Canceled;
                
            }
            return false;
        }

        private static bool GetSearchingStatusInCache(string searchId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "searchFlightStatus:" + searchId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {
                
                    var redisTransaction = redisDb.CreateTransaction();
                    redisTransaction.AddCondition(Condition.KeyNotExists(redisKey));
                    redisTransaction.StringSetAsync(redisKey, true, TimeSpan.FromMinutes(5));
                    var currentStatusTask = redisTransaction.StringGetAsync(redisKey);
                    redisTransaction.Execute();
                    return currentStatusTask.Status == TaskStatus.Canceled;
                
            }
            return true;
        }

        private void InvalidateSearchingStatusInCache(string searchId, int supplierIndex)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "searchFlightStatus:" + searchId + ":" + supplierIndex;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var expiry = GetSearchedItinerariesExpiry(searchId, supplierIndex);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    redisDb.StringSet(redisKey, false, expiry - DateTime.UtcNow); return;

                
            }

        }

        public void InvalidateSearchingStatusInCache(string searchId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "searchFlightStatus:" + searchId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var expiry = GetSearchedItinerariesExpiry(searchId, 0);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {
                
                    redisDb.StringSet(redisKey, false, expiry - DateTime.UtcNow);
                    return;
            }
        }

        private void SaveSearchedItinerariesToCache(List<List<FlightItinerary>> itinLists, string searchId, int timeout, int supplierIndex,
            DateTime searchFlightTimeOut)
        {

            var timeSpan = (searchFlightTimeOut - DateTime.UtcNow);
            if (timeout == 0)
                timeout =
                    Int32.Parse(EnvVariables.Get("flight", "SearchResultCacheTimeout"));
            var redisService = RedisService.GetInstance();
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var x = 0; x < ApConstant.RedisMaxRetry; x++)
            {
                
                    var redisTransaction = redisDb.CreateTransaction();
                    for (var i = 0; i < itinLists.Count; i++)
                    {
                        var itinList = itinLists[i];
                        var redisKey = "searchedFlightItineraries:" + i + ":" + searchId + ":" + supplierIndex;
                        var cacheObject = itinList.ToCacheObject();
                        redisTransaction.StringSetAsync(redisKey, cacheObject, timeSpan);
                    }
                    redisTransaction.Execute();
                    return;
                
            }

        }

        private static void SaveSearchedPartialItinerariesToBufferCache(List<FlightItinerary> itineraryList, string searchId, int supplierIndex, int partNumber)
        {
            var timeout = Int32.Parse(EnvVariables.Get("flight", "SearchResultCacheTimeout"));
            var redisService = RedisService.GetInstance();
            var redisKey = "searchedPartialFlightItineraries:" + searchId + ":" + supplierIndex + ":" + partNumber;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var cacheObject = itineraryList.ToCacheObject();
            for (var x = 0; x < ApConstant.RedisMaxRetry; x++)
            {
                
                    redisDb.StringSet(redisKey, cacheObject, TimeSpan.FromMinutes(timeout));
                    return;
                
            }
        }

        private static void SaveSearchedSupplierIndexToCache(string searchId, int supplierIndex, int timeout)
        {
            if (timeout == 0)
                timeout =
                    Int32.Parse(EnvVariables.Get("flight", "SearchResultCacheTimeout"));
            var redisService = RedisService.GetInstance();
            var redisKey = "searchedSupplierIndices:" + searchId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var x = 0; x < ApConstant.RedisMaxRetry; x++)
            {
                
                    redisDb.ListRightPush(redisKey, supplierIndex);
                    redisDb.KeyExpire(redisKey, TimeSpan.FromMinutes(timeout));
                    return;
               
            }
        }

        private static void SaveCurrencyStatesToCache(string searchId, Dictionary<string, Currency> currencies, int timeout)
        {
            if (timeout == 0)
                timeout =
                    Int32.Parse(EnvVariables.Get("flight", "SearchResultCacheTimeout"));
            var redisService = RedisService.GetInstance();
            var redisKey = "currencies:" + searchId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var x = 0; x < ApConstant.RedisMaxRetry; x++)
            {
                
                    redisDb.StringSet(redisKey, currencies.Serialize(), TimeSpan.FromMinutes(timeout));
                    return;
                
            }
        }

        private static Dictionary<string, Currency> GetCurrencyStatesFromCache(string searchId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "currencies:" + searchId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);

            for (var x = 0; x < ApConstant.RedisMaxRetry; x++)
            {
                
                    var redisString = (string)redisDb.StringGet(redisKey);
                    return redisString.Deserialize<Dictionary<string, Currency>>();
                
            }
            return new Dictionary<string, Currency>();
        }

        private static List<int> GetSearchedSupplierIndicesFromCache(string searchId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "searchedSupplierIndices:" + searchId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var length = redisDb.ListLength(redisKey);
            for (var x = 0; x < ApConstant.RedisMaxRetry; x++)
            {

                    return redisDb.ListRange(redisKey, 0, length - 1).Select(val => (int)val).Distinct().ToList();

            }
            return new List<int>();
        }

        private List<List<FlightItinerary>> GetSearchedPartialItinerariesFromBufferCache(string searchId, int supplierIndex)
        {
            var partialItinsCount = DecodeSearchConditions(searchId).Trips.Count;
            var redisService = RedisService.GetInstance();
            var itineraryLists = new List<List<FlightItinerary>>();
            for (var i = 0; i <= partialItinsCount; i++)
            {
                var redisKey = "searchedPartialFlightItineraries:" + searchId + ":" + supplierIndex + ":" + i;
                var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
                var cacheObject = new RedisValue();
                for (var x = 0; x < ApConstant.RedisMaxRetry; x++)
                {

                        cacheObject = redisDb.StringGet(redisKey);
                        break;

                }
                if (cacheObject.IsNullOrEmpty)
                {
                    return new List<List<FlightItinerary>>();
                }

                var itins = cacheObject.DeconvertTo<List<FlightItinerary>>();
                itineraryLists.Add(itins);
                if (partialItinsCount == 1)
                    break;
            }
            return itineraryLists;
        }

        private static FlightItinerary GetItineraryFromSearchCache(string searchId, int registerNumber, int partNumber = 1)
        {
            var redisService = RedisService.GetInstance();
            var supplierIndex = registerNumber / SupplierIndexCap;
            var redisKey = "searchedFlightItineraries:" + partNumber + ":" + searchId + ":" + supplierIndex;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var cacheObject = new RedisValue();
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    cacheObject = redisDb.StringGet(redisKey);
                    break;


            }
            if (cacheObject.IsNullOrEmpty)
            {
                return null;
            }

            var itins = cacheObject.DeconvertTo<List<FlightItinerary>>();
            if (itins == null)
                return null;
            return itins.SingleOrDefault(itin => itin.RegisterNumber == registerNumber);
        }

        public DateTime? GetSearchedItinerariesExpiry(string searchId, int supplierIndex)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "searchedFlightItineraries:0:" + searchId + ":" + supplierIndex;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    var timeToLive = redisDb.KeyTimeToLive(redisKey);
                    var expiryTime = DateTime.UtcNow + timeToLive;
                    return expiryTime;


            }
            return null;
        }

        private Dictionary<int, List<List<FlightItinerary>>> GetSearchedSupplierItineraries(string searchId, List<int> requestedSupplierIds)
        {
            var conditions = DecodeSearchConditions(searchId);
            var redisService = RedisService.GetInstance();
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var searchedSupplierItins = new Dictionary<int, List<List<FlightItinerary>>>();
            foreach (var supplierId in requestedSupplierIds)
            {
                var cacheObjects = new List<RedisValue>();
                for (var i = 0; i <= conditions.Trips.Count; i++)
                {
                    var redisKey = "searchedFlightItineraries:" + i + ":" + searchId + ":" + supplierId;
                    var cacheObject = new RedisValue();
                    for (var x = 0; x < ApConstant.RedisMaxRetry; x++)
                    {

                            cacheObject = redisDb.StringGet(redisKey);
                            break;


                    }
                    cacheObjects.Add(!cacheObject.IsNullOrEmpty
                        ? cacheObject
                        : new List<FlightItinerary>().ToCacheObject());
                    if (conditions.Trips.Count == 1)
                        break;
                }
                var itinsList = cacheObjects.Select(obj => obj.DeconvertTo<List<FlightItinerary>>()).ToList();
                searchedSupplierItins.Add(supplierId, itinsList);
            }
            return searchedSupplierItins;
        }

        private void SaveItineraryToCache(FlightItinerary itin, string itinCacheId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "flightItinerary:" + itinCacheId;
            var cacheObject = itin.ToCacheObject();
            var timeout = int.Parse(EnvVariables.Get("flight", "ItineraryCacheTimeout"));
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    redisDb.StringSet(redisKey, cacheObject, TimeSpan.FromMinutes(timeout));
                    return;


            }

        }

        private void SaveCombosToCache(List<Combo> combos, string searchId, int supplierIndex)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "flightCombos:" + searchId + ":" + supplierIndex;
            var cacheObject = combos.ToCacheObject();
            var timeout = int.Parse(EnvVariables.Get("flight", "SearchResultCacheTimeout"));
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    redisDb.StringSet(redisKey, cacheObject, TimeSpan.FromMinutes(timeout));
                    return;


            }
        }

        private List<Combo> GetCombosFromCache(string searchId, int supplierIndex)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "flightCombos:" + searchId + ":" + supplierIndex;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var cacheObject = new RedisValue();
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    cacheObject = redisDb.StringGet(redisKey);

            }

            if (cacheObject.IsNullOrEmpty)
                return new List<Combo>();
            return cacheObject.DeconvertTo<List<Combo>>() ?? new List<Combo>();
        }

        private FlightItinerary GetItineraryFromCache(string itinCacheId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "flightItinerary:" + itinCacheId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    var cacheObject = redisDb.StringGet(redisKey);

                    if (cacheObject.IsNullOrEmpty)
                        return null;

                    var itinerary = cacheObject.DeconvertTo<FlightItinerary>();
                    return itinerary;


            }
            return null;
        }

        private void DeleteItineraryFromCache(string itinCacheId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "flightItinerary:" + itinCacheId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            //var cacheObject = redisDb.StringGet(redisKey);
            redisDb.KeyDelete(redisKey);
        }

        public DateTime? GetItineraryExpiry(string itinCacheId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "flightItineraries:" + itinCacheId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    var timeToLive = redisDb.KeyTimeToLive(redisKey).GetValueOrDefault();
                    var expiryTime = DateTime.UtcNow + timeToLive;
                    return expiryTime;

            }
            return DateTime.UtcNow;
        }

        private string SaveItinerariesToCache(List<FlightItinerary> itins)
        {
            var itinCacheId = FlightItineraryCacheIdSequence.GetInstance().GetNext().ToString(CultureInfo.InvariantCulture);
            SaveItinerariesToCache(itins, itinCacheId);
            return itinCacheId;
        }

        private void SaveItinerariesToCache(List<FlightItinerary> itins, string itinCacheId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "flightItineraries:" + itinCacheId;
            var setCacheObject = itins.ToCacheObject();
            var timeout = int.Parse(EnvVariables.Get("flight", "ItineraryCacheTimeout"));
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    redisDb.StringSet(redisKey, setCacheObject, TimeSpan.FromMinutes(timeout));
                    return;

            }

        }

        private List<FlightItinerary> GetItinerariesFromCache(string itinCacheId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "flightItineraries:" + itinCacheId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {


                    var cacheObject = redisDb.StringGet(redisKey);

                    if (cacheObject.IsNullOrEmpty)
                        return null;

                    var itinerary = cacheObject.DeconvertTo<List<FlightItinerary>>();
                    return itinerary;

            }
            return null;
        }

        private void DeleteItinerariesFromCache(string itinCacheId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "flightItineraries:" + itinCacheId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    redisDb.KeyDelete(redisKey);
                    return;

            }
        }

        private void SavePaymentRedirectionUrlInCache(string rsvNo, string paymentUrl, DateTime? timeLimit)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "paymentRedirectionUrl:" + rsvNo;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    redisDb.StringSet(redisKey, paymentUrl, timeLimit - DateTime.UtcNow);
                    return;

            }
        }

        private string GetPaymentRedirectionUrlInCache(string rsvNo)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "paymentRedirectionUrl:" + rsvNo;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    var redirectionUrl = redisDb.StringGet(redisKey);
                    return redirectionUrl;

            }
            return null;
        }

        private void SaveActiveMarginRulesToCache(List<FlightMarginRule> marginRules)
        {
            var redisService = RedisService.GetInstance();
            var redisMarginsKey = "activeFlightMargins";
            var redisRulesKey = "activeFlightMarginRules";
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var marginsCacheObject = marginRules.Select(mr => mr.Margin).ToCacheObject();
            var rulesCacheObject = marginRules.Select(mr => mr.Rule).ToCacheObject();
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    redisDb.StringSet(redisMarginsKey, marginsCacheObject);
                    redisDb.StringSet(redisRulesKey, rulesCacheObject);
                    return;

            }
        }

        private List<FlightMarginRule> GetAllActiveMarginRulesFromCache()
        {
            var redisService = RedisService.GetInstance();
            var redisMarginKey = "activeFlightMargins";
            var redisRuleKey = "activeFlightMarginRules";
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);

            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    var marginCacheObject = redisDb.StringGet(redisMarginKey);
                    var margins = marginCacheObject.DeconvertTo<List<Margin>>();
                    var ruleCacheObject = redisDb.StringGet(redisRuleKey);
                    var rules = ruleCacheObject.DeconvertTo<List<FlightItineraryRule>>();
                    var marginRules = margins.Zip(rules, (margin, rule) => new FlightMarginRule(margin, rule)).ToList();
                    return marginRules;


            }
            return new List<FlightMarginRule>();
        }

        private void SaveActiveMarginRulesInBufferCache(List<FlightMarginRule> marginRules)
        {
            var redisService = RedisService.GetInstance();
            var marginsRedisKey = "activeFlightMarginsBuffer";
            var rulesRedisKey = "activeFlightMarginRulesBuffer";
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var marginsCacheObject = marginRules.Select(mr => mr.Margin).ToCacheObject();
            var rulesCacheObject = marginRules.Select(mr => mr.Rule).ToCacheObject();
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    redisDb.StringSet(marginsRedisKey, marginsCacheObject);
                    redisDb.StringSet(rulesRedisKey, rulesCacheObject);
                    return;

            }
        }

        private void SaveDeletedMarginRulesInBufferCache(List<FlightMarginRule> deletedMarginRules)
        {
            var redisService = RedisService.GetInstance();
            var deletedMarginsRedisKey = "deletedFlightMarginsBuffer";
            var deletedRulesRedisKey = "deletedFlightMarginRulesBuffer";
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var deletedMarginsCacheObject = deletedMarginRules.Select(mr => mr.Margin).ToCacheObject();
            var deletedRulesCacheObject = deletedMarginRules.Select(mr => mr.Rule).ToCacheObject();
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    redisDb.StringSet(deletedMarginsRedisKey, deletedMarginsCacheObject);
                    redisDb.StringSet(deletedRulesRedisKey, deletedRulesCacheObject);
                    return;

            }
        }

        private List<FlightMarginRule> GetActiveMarginRulesFromBufferCache()
        {
            var redisService = RedisService.GetInstance();
            var redisMarginsKey = "activeFlightMarginsBuffer";
            var redisRulesKey = "activeFlightMarginRulesBuffer";
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);

            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    var marginsCacheObject = redisDb.StringGet(redisMarginsKey);
                    var rulesCacheObject = redisDb.StringGet(redisRulesKey);
                    var margins = marginsCacheObject.DeconvertTo<List<Margin>>();
                    var rules = rulesCacheObject.DeconvertTo<List<FlightItineraryRule>>();
                    var marginRules = margins.Zip(rules, (margin, rule) => new FlightMarginRule(margin, rule)).ToList();
                    return marginRules;

            }
            return new List<FlightMarginRule>();
        }

        private List<FlightMarginRule> GetDeletedMarginRulesFromBufferCache()
        {
            var redisService = RedisService.GetInstance();
            var redisMarginsKey = "deletedFlightMarginsBuffer";
            var redisRulesKey = "deletedFlightMarginRulesBuffer";
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);

            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    var marginCacheObject = redisDb.StringGet(redisMarginsKey);
                    var rulesCacheObject = redisDb.StringGet(redisRulesKey);
                    var margins = marginCacheObject.DeconvertTo<List<Margin>>();
                    var rules = rulesCacheObject.DeconvertTo<List<FlightItineraryRule>>();
                    var marginRules = margins.Zip(rules, (margin, rule) => new FlightMarginRule(margin, rule)).ToList();
                    return marginRules;

            }
            return new List<FlightMarginRule>();
        }

        public void SetFlightRequestTripType(string requestId, bool asReturn)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "flightRequestAsReturn:" + requestId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var timeout = Int32.Parse(EnvVariables.Get("flight", "SearchResultCacheTimeout"));
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    redisDb.StringSet(redisKey, asReturn, new TimeSpan(0, 2 * timeout, 0));
                    return;

            }
        }

        public bool? GetFlightRequestTripType(string requestId)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "flightRequestAsReturn:" + requestId;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    return (bool?)redisDb.StringGet(redisKey);

            }
            return null;
        }

        private static List<TopDestination> GetTopDestinationsFromCache()
        {
            var redisService = RedisService.GetInstance();
            var redisDb = redisService.GetDatabase(ApConstant.MasterDataCacheName);
            var cacheKey = EnvVariables.Get("flight", "topdestinationcachekey");
            var rawTopDestinationsListFromCache = new RedisValue();
            for (var i = 0; i < ApConstant.RedisMaxRetry; i++)
            {

                    rawTopDestinationsListFromCache = redisDb.StringGet(cacheKey);
                    break;

            }

            if (rawTopDestinationsListFromCache.IsNullOrEmpty)
            {
                return new List<TopDestination>();
            }
            var topDestinations = rawTopDestinationsListFromCache.DeconvertTo<List<TopDestination>>();
            return topDestinations;
        }
    }
}
