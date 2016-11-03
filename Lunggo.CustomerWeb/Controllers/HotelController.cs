﻿using Lunggo.CustomerWeb.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Lunggo.CustomerWeb.Controllers
{
    public class HotelController : Controller
    {
        // GET: Hotel
        public ActionResult Search()
        {
            try
            {
                NameValueCollection query = Request.QueryString;
                if (query.Count > 0)
                {
                    HotelSearchApiRequest model = new HotelSearchApiRequest(query[0]);

                    return View(model); 
                }

                return View();

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }

        }
        //public ActionResult DetailHotel()
        //{
        //    return View();
        //}
        public ActionResult DetailHotel(string searchId, int hotelCd)
        {
            return View(new { searchId, hotelCd });
        }
        public ActionResult Checkout()
        {
            return View();
        }
        public ActionResult Thankyou()
        {
            return View();
        }
        public ActionResult OrderHotelHistoryDetail()
        {
            return View();
        }
        public ActionResult BankTransferHotel()
        {
            return View();
        }
        public ActionResult VirtualAccountHotel()
        {
            return View();
        }
        public ActionResult EmailVoucher()
        {
            return View();
        }
        public ActionResult VoucherHotel()
        {
            return View();
        }
        public ActionResult SorryEmailHotel()
        {
            return View();
        }
    }
}