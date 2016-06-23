﻿using System.Net;
using System.Web;
using Lunggo.ApCommon.Identity.User;
using Lunggo.WebAPI.ApiSrc.Account.Model;
using Lunggo.WebAPI.ApiSrc.Common.Model;
using Microsoft.AspNet.Identity;

namespace Lunggo.WebAPI.ApiSrc.Account.Logic
{
    public static partial class AccountLogic
    {
        public static ApiResponseBase ChangeProfile(ChangeProfileApiRequest request, ApplicationUserManager userManager)
        {
            var user = HttpContext.Current.User;
            var updatedUser = user.Identity.GetUser();
            string first, last;
            if (request.Name == null)
            {
                first = updatedUser.FirstName;
                last = updatedUser.LastName;
            }
            else
            {
                var splittedName = request.Name.Split(' ');
                if (splittedName.Length == 1)
                {
                    first = request.Name;
                    last = request.Name;
                }
                else
                {
                    first = request.Name.Substring(0, request.Name.LastIndexOf(' '));
                    last = splittedName[splittedName.Length - 1];
                }
            }
            updatedUser.FirstName = first;
            updatedUser.LastName = last;
            updatedUser.CountryCd = request.CountryCallingCd ?? updatedUser.CountryCd;
            updatedUser.PhoneNumber = request.PhoneNumber ?? updatedUser.PhoneNumber;
            var result = userManager.Update(updatedUser);
            if (result.Succeeded)
            {
                return new ApiResponseBase
                {
                    StatusCode = HttpStatusCode.OK
                };
            }
            return new ApiResponseBase
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorCode = "ERRGEN99"
            };
        }
    }
}