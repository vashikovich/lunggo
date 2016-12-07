﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunggo.ApCommon.Hotel.Service;
using Lunggo.ApCommon.Hotel.Wrapper.HotelBeds.Content;
using Lunggo.Framework.BlobStorage;
using Lunggo.Framework.Config;
using Lunggo.Framework.Database;
using Lunggo.Framework.Documents;
using Lunggo.Framework.TableStorage;

namespace Lunggo.Webjob.HotelContentWrapper
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Init();
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing.
            stopwatch.Start();

            var hotel = new HotelBedsService();
            var service = HotelService.GetInstance();
            Console.WriteLine("start!");
            service.SaveAutoCompleteToTableStorage();

            var hotelService = HotelService.GetInstance();
            hotelService.UpdateHotelImage();

            ///*Try to get file name*/
            //var blobService = BlobStorageService.GetInstance();
            //blobService.GetFileNameList("hotelimage");
            stopwatch.Stop();
            Debug.Print("Done in : {0}", stopwatch.Elapsed);
            Console.WriteLine("Done in : {0}", stopwatch.Elapsed);
            Console.ReadKey();
        }
    }
}