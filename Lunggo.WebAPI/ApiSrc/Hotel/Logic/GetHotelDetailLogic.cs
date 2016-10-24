﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Lunggo.ApCommon.Hotel.Model.Logic;
using Lunggo.ApCommon.Hotel.Service;
using Lunggo.Framework.Config;
using Lunggo.Framework.Log;
using Lunggo.WebAPI.ApiSrc.Common.Model;
using Lunggo.WebAPI.ApiSrc.Hotel.Model;

namespace Lunggo.WebAPI.ApiSrc.Hotel.Logic
{
    public static partial class HotelLogic
    {
        public static ApiResponseBase GetHotelDetailLogic(HotelDetailApiRequest request)
        {
            if (IsValid(request))
            {
                var searchServiceRequest = PreprocessServiceRequest(request);
                var searchServiceResponse = HotelService.GetInstance().GetHotelDetail(searchServiceRequest);
                var apiResponse = AssembleApiResponse(searchServiceResponse);
                if (apiResponse.StatusCode == HttpStatusCode.OK) return apiResponse;
                return apiResponse;
            }
            return new HotelDetailApiResponse
            {
                StatusCode = HttpStatusCode.BadRequest,
                ErrorCode = "ERHGHD01"
            };
        }

        private static bool IsValid(HotelDetailApiRequest request)
        {
            if (request == null)
                return false;
            return
                request.HotelCode != 0;

        }

        private static GetHotelDetailInput PreprocessServiceRequest(HotelDetailApiRequest request)
        {
            var searchServiceRequest = new GetHotelDetailInput
            {
                SearchId = request.SearchId,
                HotelCode = request.HotelCode,
            };
            return searchServiceRequest;
        }

        private static HotelDetailApiResponse AssembleApiResponse(GetHotelDetailOutput searchServiceResponse)
        {
            var apiResponse = new HotelDetailApiResponse
            {
                HotelDetails = searchServiceResponse.HotelDetail,
                StatusCode = HttpStatusCode.OK
            };
            return apiResponse;
        }
    }
}