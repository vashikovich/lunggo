﻿using Newtonsoft.Json;

namespace Lunggo.WebAPI.ApiSrc.Account.Model
{
    public class LoginApiRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
        [JsonProperty("clientId")]
        public string ClientId { get; set; }
        [JsonProperty("clientSecret")]
        public string ClientSecret { get; set; }
    }
}