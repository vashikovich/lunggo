﻿using Lunggo.WebAPI.ApiSrc.Common.Model;
using System.Collections.Generic;
using Lunggo.ApCommon.Activity.Model;
using Newtonsoft.Json;
using System;

namespace Lunggo.WebAPI.ApiSrc.Activity.Model
{
    public class GetAppointmentListActiveApiResponse : ApiResponseBase
    {
        [JsonProperty("appointments", NullValueHandling = NullValueHandling.Ignore)]
        public List<AppointmentListForDisplay> Appointments { get; set; }
        [JsonProperty("mustUpdate", NullValueHandling = NullValueHandling.Ignore)]
        public bool MustUpdate { get; set; }
        [JsonProperty("lastUpdate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime LastUpdate { get; set; }
    }
}