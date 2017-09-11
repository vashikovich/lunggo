﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Lunggo.ApCommon.Flight.Wrapper.Tiket.Model;
using Lunggo.ApCommon.Hotel.Wrapper.HotelBeds.Sdk.types;
using Lunggo.Framework.Config;
using Lunggo.Framework.Extension;
using RestSharp;

namespace Lunggo.ApCommon.Flight.Wrapper.Tiket
{
    internal partial class TiketWrapper
    {
        private partial class TiketClientHandler
        {
            private static readonly TiketClientHandler ClientInstance = new TiketClientHandler();
            private bool _isInitialized;
            private static string _basePath;
            private static string _sharedSecret;
            private static string _token;
            private static string _confirmKey;


            private TiketClientHandler()
            {
                
            }

            internal static TiketClientHandler GetClientInstance()
            {
                return ClientInstance;
            }


            internal void Init()
            {
                if (!_isInitialized)
                {
                    _basePath = ConfigManager.GetInstance().GetConfigValue("tiket", "apiUrl");
                    _sharedSecret = ConfigManager.GetInstance().GetConfigValue("tiket", "apiSecret");
                    _confirmKey = ConfigManager.GetInstance().GetConfigValue("tiket", "apiConfirmKey");
                    _isInitialized = true;
                }
            }

            public RestClient CreateTiketClient()
            {
                var client = new RestClient(_basePath)
                {
                    CookieContainer = new CookieContainer(),
                    UserAgent = "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36"
                };
                return client;
            }

            public string GetToken()
            {
                var client = CreateTiketClient();
                var url = "/apiv1/payexpress?method=getToken&secretkey=" + _sharedSecret + "&output=json";
                var request = new RestRequest(url, Method.GET);
                var response = client.Execute(request);
                var responseToken = response.Content.Deserialize<TiketBaseResponse>();
                _token = responseToken == null ? null : responseToken.Token;
                return _token;
            }

            
        }
    }
}
