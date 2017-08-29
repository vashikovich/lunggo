﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunggo.ApCommon.Flight.Wrapper.Tiket.Model;
using Newtonsoft.Json;

namespace Lunggo.ApCommon.Hotel.Wrapper.Tiket.Model
{
    public  class TiketHotelOrderResponse :TiketBaseResponse
    {

        [JsonProperty("myorder", NullValueHandling = NullValueHandling.Ignore)]
        public MyOrder MyOrder { get; set; }
    }

    public class MyOrder
    {
        [JsonProperty("order_id", NullValueHandling = NullValueHandling.Ignore)]
        public string OrderId { get; set; }
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public List<Data> Data { get; set; }
        [JsonProperty("total", NullValueHandling = NullValueHandling.Ignore)]
        public decimal Total { get; set; }
        [JsonProperty("discount", NullValueHandling = NullValueHandling.Ignore)]
        public decimal Discount { get; set; }
        [JsonProperty("discount_amount", NullValueHandling = NullValueHandling.Ignore)]
        public decimal DiscountAmount { get; set; }
        [JsonProperty("total_tax", NullValueHandling = NullValueHandling.Ignore)]
        public decimal TotalTax { get; set; }
        [JsonProperty("total_without_tax", NullValueHandling = NullValueHandling.Ignore)]
        public decimal TotalWithoutTax { get; set; }
        [JsonProperty("count_installment", NullValueHandling = NullValueHandling.Ignore)]
        public decimal CountInstallment { get; set; }
    }

    public class Data
    {
        [JsonProperty("business_id", NullValueHandling = NullValueHandling.Ignore)]
        public string BusinessId { get; set; }
        [JsonProperty("expire", NullValueHandling = NullValueHandling.Ignore)]
        public int Expire { get; set; }
        [JsonProperty("order_detail_id", NullValueHandling = NullValueHandling.Ignore)]
        public string OrderDetailId { get; set; }
        [JsonProperty("order_detail_status", NullValueHandling = NullValueHandling.Ignore)]
        public string OrderDetailStatus { get; set; }
        [JsonProperty("order_expire_datetime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime OrderExpireDatetime{ get; set; }
        [JsonProperty("order_type", NullValueHandling = NullValueHandling.Ignore)]
        public string OrderType { get; set; }
        [JsonProperty("order_name", NullValueHandling = NullValueHandling.Ignore)]
        public string OrderName { get; set; }
        [JsonProperty("order_name_detail", NullValueHandling = NullValueHandling.Ignore)]
        public string OrderNameDetail { get; set; }
        [JsonProperty("tenor", NullValueHandling = NullValueHandling.Ignore)]
        public string Tenor { get; set; }
        [JsonProperty("detail", NullValueHandling = NullValueHandling.Ignore)]
        public OrderDetail Detail { get; set; }
        [JsonProperty("order_photo", NullValueHandling = NullValueHandling.Ignore)]
        public string OrderPhoto { get; set; }
        [JsonProperty("tax", NullValueHandling = NullValueHandling.Ignore)]
        public decimal Tax { get; set; }
        [JsonProperty("item_charge", NullValueHandling = NullValueHandling.Ignore)]
        public decimal ItemCharge { get; set; }
        [JsonProperty("subtotal_and_charge", NullValueHandling = NullValueHandling.Ignore)]
        public decimal SubtotalCharge { get; set; }
        [JsonProperty("delete_uri", NullValueHandling = NullValueHandling.Ignore)]
        public string DeleteUri { get; set; }
    }

    public class OrderDetail
    {
        [JsonProperty("order_detail_id", NullValueHandling = NullValueHandling.Ignore)]
        public string OrderDetailId { get; set; }
        [JsonProperty("room_id", NullValueHandling = NullValueHandling.Ignore)]
        public string RoomId { get; set; }
        [JsonProperty("rooms", NullValueHandling = NullValueHandling.Ignore)]
        public int Rooms { get; set; }
        [JsonProperty("adult", NullValueHandling = NullValueHandling.Ignore)]
        public int Adult { get; set; }
        [JsonProperty("child", NullValueHandling = NullValueHandling.Ignore)]
        public int Child { get; set; }
        [JsonProperty("startdate", NullValueHandling = NullValueHandling.Ignore)]
        public string StartDate { get; set; }
        [JsonProperty("enddate", NullValueHandling = NullValueHandling.Ignore)]
        public string EndDate { get; set; }
        [JsonProperty("nights", NullValueHandling = NullValueHandling.Ignore)]
        public int Nights { get; set; }
        [JsonProperty("price", NullValueHandling = NullValueHandling.Ignore)]
        public decimal Price { get; set; }
        [JsonProperty("price_per_night", NullValueHandling = NullValueHandling.Ignore)]
        public decimal PricePerNight { get; set; }
    }
}