﻿using Lunggo.ApCommon.Activity.Model;
using Lunggo.ApCommon.Activity.Service;
using Lunggo.WebAPI.ApiSrc.Activity.Model;
using Lunggo.WebAPI.ApiSrc.Common.Model;
using System.Linq;
using System.Net;

namespace Lunggo.WebAPI.ApiSrc.Activity.Logic
{
    public static partial class ActivityLogic
    {
        public static ApiResponseBase Search(ActivitySearchApiRequest request)
        {
            if (!IsValid(request))
                return new ActivitySearchApiResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = "ERASEA01"
                };
            var searchServiceRequest = PreprocessServiceRequest(request);
            var searchServiceResponse = ActivityService.GetInstance().Search(searchServiceRequest);
            var apiResponse = AssembleApiResponse(searchServiceResponse);

            return apiResponse;
        }

        private static bool IsValid(ActivitySearchApiRequest request)
        {
            if (request == null)
                return false;
            if (request.SearchId != null)
            {
                return (request.Page > 0 && request.PerPage > 0);
            }
            else
            {
                return true;
            }
        }

        private static SearchActivityInput PreprocessServiceRequest(ActivitySearchApiRequest request)
        {
            var searchServiceRequest = new SearchActivityInput
            {
                SearchId = request.SearchId,
                ActivityFilter = request.Filter,
                Page = request.Page,
                PerPage = request.PerPage
                //SearchHotelType = request.SearchType,
                //SearchId = request.SearchId,
                //CheckIn = request.CheckinDate,
                //Checkout = request.CheckoutDate,
                //AdultCount = request.AdultCount,
                //ChildCount = request.ChildCount,
                //Nights = request.NightCount,
                //Occupancies = request.Occupancies,
                //Rooms = request.RoomCount,
                //Location = request.Location,
                //Page = request.Page,
                //PerPage = request.PerPage,
                //FilterParam = request.Filter,
                //SortingParam = request.Sorting,
                //HotelCode = request.HotelCode,
                //RegsId = request.RegsId
            };
            return searchServiceRequest;
        }

        private static ActivitySearchApiResponse AssembleApiResponse(SearchActivityOutput searchServiceResponse)
        {
            var apiResponse = new ActivitySearchApiResponse
            {
                ActivityList = searchServiceResponse.ActivityList.Select(actList => new ActivityDetailForDisplay()
                {
                    Name = actList.Name,
                    City = actList.City,
                    Country = actList.Country,
                    Description = actList.Description,
                    OperationTime = actList.OperationTime,
                    Price = actList.Price
                }).ToList()
            };
            return apiResponse;
            
        }
    }
}