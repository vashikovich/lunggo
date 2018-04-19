﻿using Lunggo.ApCommon.Activity.Model.Logic;
using Lunggo.ApCommon.Identity.Users;
using Lunggo.WebAPI.ApiSrc.Activity.Model;
using Lunggo.WebAPI.ApiSrc.Common.Model;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Lunggo.WebAPI.ApiSrc.Activity.Logic
{
    public partial class ActivityLogic
    {
        //public ApiResponseBase GetReservationList(GetReservationListApiRequest apiRequest, ApplicationUserManager userManager)
        //{
        //    var user = HttpContext.Current.User;
        //    if (string.IsNullOrEmpty(user.Identity.Name))
        //    {
        //        return new GetAppointmentListApiResponse
        //        {
        //            StatusCode = HttpStatusCode.Unauthorized,
        //            ErrorCode = "ERR_UNDEFINED_USER" //ERAGPR01
        //        };
        //    }
        //    var role = userManager.GetRoles(user.Identity.GetUser().Id).FirstOrDefault();
        //    if (role != "Operator")
        //    {
        //        return new GetAppointmentListApiResponse
        //        {
        //            StatusCode = HttpStatusCode.Unauthorized,
        //            ErrorCode = "ERR_NOT_OPERATOR"
        //        };
        //    }
        //    GetReservationListInput serviceRequest;
        //    var succeed = TryPreprocess(request, out serviceRequest);
        //    if (!succeed)
        //    {
        //        return new GetAppointmentListApiResponse
        //        {
        //            StatusCode = HttpStatusCode.BadRequest,
        //            ErrorCode = "ERR_INVALID_REQUEST"
        //        };
        //    }
        //    var serviceResponse = ActivityService.GetInstance().GetAppointmentList(serviceRequest);
        //    var apiResponse = AssembleApiResponse(serviceResponse);
        //
        //    return apiResponse;
        //}
        //
        //public static bool TryPreprocess(GetReservationListApiRequest request, out GetAppointmentListInput serviceRequest)
        //{
        //    serviceRequest = new GetAppointmentListInput();
        //
        //    if (request == null)
        //    {
        //        return false;
        //    }
        //
        //    int pageValid;
        //    bool isPageNumeric = int.TryParse(request.Page, out pageValid);
        //    if (!isPageNumeric) { return false; }
        //
        //    int perPageValid;
        //    bool isPerPageNumeric = int.TryParse(request.PerPage, out perPageValid);
        //    if (!isPerPageNumeric) { return false; }
        //
        //    if (pageValid < 0 || perPageValid < 0)
        //    {
        //        return false;
        //    }
        //
        //
        //    serviceRequest.Page = pageValid;
        //    serviceRequest.PerPage = perPageValid;
        //    return true;
        //}
        //
        //public static GetAppointmentListApiResponse AssembleApiResponse(GetAppointmentListOutput serviceResponse)
        //{
        //    var apiResponse = new GetAppointmentListApiResponse()
        //    {
        //        Appointments = serviceResponse.Appointments.Select(AppointmentList => new AppointmentListForDisplay()
        //        {
        //            ActivityId = AppointmentList.ActivityId,
        //            Name = AppointmentList.Name,
        //            Date = AppointmentList.Date,
        //            Session = AppointmentList.Session,
        //            MediaSrc = AppointmentList.MediaSrc,
        //            AppointmentReservations = AppointmentList.AppointmentReservations
        //        }).ToList(),
        //        Page = serviceResponse.Page,
        //        PerPage = serviceResponse.PerPage,
        //        StatusCode = HttpStatusCode.OK
        //    };
        //
        //    return apiResponse;
        //
        //}
    }
}