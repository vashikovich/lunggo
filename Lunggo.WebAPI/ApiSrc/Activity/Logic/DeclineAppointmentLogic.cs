﻿using System;
using Lunggo.ApCommon.Activity.Model;
using Lunggo.ApCommon.Activity.Service;
using Lunggo.WebAPI.ApiSrc.Activity.Model;
using Lunggo.WebAPI.ApiSrc.Common.Model;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Lunggo.ApCommon.Activity.Model.Logic;
using Lunggo.ApCommon.Product.Constant;
using System.Web;
using Microsoft.AspNet.Identity;
using Lunggo.ApCommon.Identity.Users;

namespace Lunggo.WebAPI.ApiSrc.Activity.Logic
{
    public static partial class ActivityLogic
    {
        public static ApiResponseBase DeclineAppointment(string rsvNo, ApplicationUserManager userManager)
        {
            var user = HttpContext.Current.User;
            if (string.IsNullOrEmpty(user.Identity.Name))
            {
                return new ApiResponseBase
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ErrorCode = "ERR_UNDEFINED_USER" //ERAGPR01
                };
            }
            var role = userManager.GetRoles(user.Identity.GetUser().Id).FirstOrDefault();
            if (role != "Operator")
            {
                return new ApiResponseBase
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                    ErrorCode = "ERR_NOT_OPERATOR"
                };
            }

            if (string.IsNullOrEmpty(rsvNo))
            {
                return new ApiResponseBase
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorCode = "ERR_INVALID_REQUEST"
                };
            }

            var serviceRequest = PreprocessServiceRequest(rsvNo);
            var serviceResponse = ActivityService.GetInstance().DenyAppointment(serviceRequest);
            var apiResponse = AssembleApiResponse(serviceResponse);

            return apiResponse;
        }
    }
}