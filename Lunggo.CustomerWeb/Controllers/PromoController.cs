﻿using System.Collections.Generic;
using System.Web.Mvc;

namespace Lunggo.CustomerWeb.Controllers
{
    public class PromoController : Controller
    {
        [Route("{langCode}/Promo/{name}")]
        public ActionResult Promo(string name)
        {
            var termList = new List<string>();
            var howToGet = new List<string>();
            switch (name.ToLower())
            {
                case "onlinerevolution":
                    return View("OnlineRevolution");
                case "onlinerevolutionwebview":
                    return View("OnlineRevolutionWebview");
                case "imlek":
                    return View("Imlek");
                case "btnterbanginhemat":

                    ViewBag.Title = "Payday Madness Permata Bank";
                    ViewBag.ImageSrcDesktop = "/Assets/images/campaign/PaydayMadness/PaydayMadness-page.jpg";
                    ViewBag.ImageSrcMobile = "/Assets/images/campaign/PaydayMadness/PaydayMadness-page-mobile.jpg";
                    ViewBag.ImageAlt = "Payday Madness Permata Bank";
                    ViewBag.PromoTitle = "Payday Madness Permata Bank";
                    ViewBag.PromoDescription = "Yeay payday time! Saatnya memanjakan diri bersama keluarga atau sahabat dengan menginap di hotel-hotel terbaik pilihan kamu. Nikmati diskon 15% maksimal Rp 150.000 untuk setiap reservasi kamar hotel di Indonesia dan luar negeri dengan menggunakan metode pembayaran transfer bank melalui ATM Bank Permata.";
                    ViewBag.PeriodeBooking = "25 - 27 Agustus 2017";
                    ViewBag.PeriodeInap = "Kapan Saja";

                    termList.Add("Diskon sebesar 15% (Maks. Rp 150.000). Tanpa minimum transaksi");
                    termList.Add("Berlaku untuk pemesanan setiap tanggal <span class=\"os-bold blue-txt\">25, 26, dan 27</span> setiap bulan");
                    termList.Add("Periode terbang dan inap kapan saja");
                    termList.Add("Berlaku untuk semua hotel di dalam dan luar negeri");
                    termList.Add("Berlaku untuk pembayaran menggunakan bank transfer supported by Permata Bank");
                    termList.Add("Berlaku untuk pemesanan melalui situs Travorama");
                    termList.Add("Program promo hanya dapat digunakan satu kali per hari untuk satu user");
                    termList.Add("Program tidak dapat digabungkan dengan voucher dan promo lainnya");
                    termList.Add("Travorama berhak penuh untuk mengubah syarat dan ketentuan tanpa pemberitahuan terlebih dahulu");
                    ViewBag.Terms = termList;

                    howToGet.Add("Cari dan pesan kamar hotel melalui situs Travorama");
                    howToGet.Add("Pilih metode pembayaran bank transfer supported by Permata Bank (Payday Madness)");
                    howToGet.Add("Harga akan otomatis terpotong sesuai program promo");
                    howToGet.Add("Lakukan pembayaran sesuai dengan instruksi");
                    howToGet.Add("Selesaikan proses pembayaran dan lakukan konfirmasi pembayaran");
                    howToGet.Add("E-voucher hotel akan dikirimkan ke alamat email Anda");
                    ViewBag.HowToGet = howToGet;


                    return View("BTNTerbanginHemat");
                case "btnterbanginhematwebview":
                    return View("BTNTerbanginHematWebview");
                case "hutbtn":
                    return View("HutBTN");
                case "hutbtnwebview":
                    return View("HutBTNWebview");
                case "harbolnas2016":
                    return View("Harbolnas2016");
                case "mataharimall":
                    return View("MatahariMall");
                case "paydaymadnesspermatabank":

                    ViewBag.Title = "Payday Madness Permata Bank";
                    ViewBag.ImageSrc = "/Assets/images/campaign/PaydayMadness/PaydayMadness-page-mobile.jpg";
                    ViewBag.ImageAlt = "Payday Madness Permata Bank";
                    ViewBag.PromoTitle = "Payday Madness Permata Bank";
                    ViewBag.PromoDescription = "Yeay payday time! Saatnya memanjakan diri bersama keluarga atau sahabat dengan menginap di hotel-hotel terbaik pilihan kamu. Nikmati diskon 15% maksimal Rp 150.000 untuk setiap reservasi kamar hotel di Indonesia dan luar negeri dengan menggunakan metode pembayaran transfer bank melalui ATM Bank Permata.";
                    ViewBag.PeriodeBooking = "25 - 27 Agustus 2017";
                    ViewBag.PeriodeInap = "Kapan Saja";

                    termList.Add("Diskon sebesar 15% (Maks. Rp 150.000). Tanpa minimum transaksi");
                    termList.Add("Berlaku untuk pemesanan setiap tanggal <span class=\"os-bold blue-txt\">25, 26, dan 27</span> setiap bulan");
                    termList.Add("Periode terbang dan inap kapan saja");
                    termList.Add("Berlaku untuk semua hotel di dalam dan luar negeri");
                    termList.Add("Berlaku untuk pembayaran menggunakan bank transfer supported by Permata Bank");
                    termList.Add("Berlaku untuk pemesanan melalui situs Travorama");
                    termList.Add("Program promo hanya dapat digunakan satu kali per hari untuk satu user");
                    termList.Add("Program tidak dapat digabungkan dengan voucher dan promo lainnya");
                    termList.Add("Travorama berhak penuh untuk mengubah syarat dan ketentuan tanpa pemberitahuan terlebih dahulu");
                    ViewBag.Terms = termList;

                    howToGet.Add("Cari dan pesan kamar hotel melalui situs Travorama");
                    howToGet.Add("Pilih metode pembayaran bank transfer supported by Permata Bank (Payday Madness)");
                    howToGet.Add("Harga akan otomatis terpotong sesuai program promo");
                    howToGet.Add("Lakukan pembayaran sesuai dengan instruksi");
                    howToGet.Add("Selesaikan proses pembayaran dan lakukan konfirmasi pembayaran");
                    howToGet.Add("E-voucher hotel akan dikirimkan ke alamat email Anda");
                    ViewBag.HowToGet = howToGet;
                    
                    return View("PaydayMadnessPermataBank");
                case "paydaymegadeals":
                    return View("PaydayMegaDeals");
                default:
                    return RedirectToAction("Index", "Index");
            }
        }
    }
}