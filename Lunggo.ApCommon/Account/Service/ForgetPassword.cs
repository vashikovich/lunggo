﻿using Lunggo.ApCommon.Account.Model.Logic;
using Lunggo.ApCommon.Constant;
using Lunggo.Framework.Encoder;
using Lunggo.Framework.Pattern;
using Lunggo.Framework.Queue;
using Lunggo.Framework.Redis;
using Lunggo.Framework.SmsGateway;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lunggo.ApCommon.Account.Service
{
    public partial class AccountService : SingletonBase<AccountService>
    {
        public bool CheckContactData (ForgetPasswordInput forgetPasswordInput)
        {
            var userId = GetUserIdFromDb(forgetPasswordInput);
            if (userId.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public ForgetPasswordOutput ForgetPassword(ForgetPasswordInput forgetPasswordInput)
        {
            var otp = GenerateOtp();
            if (!string.IsNullOrEmpty(forgetPasswordInput.PhoneNumber))
            {
                
                var isSendSmsSuccess = SendOtpToUser(forgetPasswordInput.PhoneNumber, otp);
                if (isSendSmsSuccess == false)
                {
                    return new ForgetPasswordOutput
                    {
                        isSuccess = false
                    };
                }
                var expireTime = DateTime.UtcNow.AddMinutes(30);
                if (CheckPhoneNumberFromResetPasswordDb(forgetPasswordInput) == false)
                {
                    InsertDataResetPasswordToDb(forgetPasswordInput, otp, expireTime);
                }
                else
                {
                    UpdateDateResetPasswordToDb(forgetPasswordInput, otp, expireTime);
                }


                if (isSendSmsSuccess == false)
                {
                    return new ForgetPasswordOutput
                    {
                        isSuccess = false
                    };
                }
                InsertSmsTimeToCache(forgetPasswordInput.PhoneNumber, otp);


                return new ForgetPasswordOutput
                {
                    CountryCallCd = forgetPasswordInput.CountryCallCd,
                    PhoneNumber = forgetPasswordInput.PhoneNumber,
                    Otp = otp,
                    ExpireTime = expireTime,
                    isSuccess = true
                };
            }
            else
            {
                SendOtpToUserByEmail(forgetPasswordInput.Email, otp);
                var expireTime = DateTime.UtcNow.AddMinutes(30);
                if (CheckPhoneNumberFromResetPasswordDb(forgetPasswordInput) == false)
                {
                    InsertDataResetPasswordToDb(forgetPasswordInput, otp, expireTime);
                }
                else
                {
                    UpdateDateResetPasswordToDb(forgetPasswordInput, otp, expireTime);
                }
                InsertEmailTimeToCache(forgetPasswordInput.Email, otp);


                return new ForgetPasswordOutput
                {
                    CountryCallCd = forgetPasswordInput.CountryCallCd,
                    Email = forgetPasswordInput.Email,
                    Otp = otp,
                    ExpireTime = expireTime,
                    isSuccess = true
                };
            }
            
        }
        
        public string GenerateOtp()
        {
            var rng = new RNGCryptoServiceProvider();
            byte[] randomByte = new byte[8];
            rng.GetBytes(randomByte);
            var randomInt = Math.Abs(BitConverter.ToInt32(randomByte, 0));
            var intOtp = randomInt % 1000000;
            var otp = intOtp.ToString("D6");
            return otp;
        }

        public bool CheckPhoneNumberFromResetPasswordDb(ForgetPasswordInput forgetPasswordInput)
        {
            var OtpHash = GetOtpHashFromDb(forgetPasswordInput);
            if (OtpHash.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool SendOtpToUser (string phoneNumber, string otp)
        {
            var message = "Your Otp Is : " + otp;
            var smsGateway = new SmsGateway();
            var response = smsGateway.SendSms(phoneNumber, message);
            if (response.Content.Contains("<text>Success</text>"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void InsertSmsTimeToCache (string phoneNumber, string otp)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "ForgotPasswordSmsTimer:" + phoneNumber;
            var redisValue = DateTime.UtcNow.AddSeconds(90).ToString();
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            redisDb.StringSet(redisKey, redisValue, TimeSpan.FromSeconds(90));
        }

        public void InsertEmailTimeToCache (string email, string otp)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "ForgotPasswordEmailTimer:" + email;
            var redisValue = DateTime.UtcNow.AddSeconds(90).ToString();
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            redisDb.StringSet(redisKey, redisValue, TimeSpan.FromSeconds(90));
        }
        public bool CheckTimerSms(string phoneNumber, out int? resendCooldownSeconds)
        {
            if (phoneNumber.StartsWith("0"))
            {
                phoneNumber = phoneNumber.Substring(1);
            }
            
            var redisService = RedisService.GetInstance();
            var redisKey = "ForgotPasswordSmsTimer:" + phoneNumber;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var result = redisDb.KeyExists(redisKey);
            var resendCooldown = redisDb.StringGetWithExpiry(redisKey).Expiry;
            
            if (result == false)
            {
                resendCooldownSeconds = null;
                return true;
                
            }
            else
            {
                resendCooldownSeconds = Convert.ToInt32(resendCooldown.Value.TotalSeconds) + 30;
                return false;              
            }
        }

        public bool CheckTimerEmail(string email, out int? resendCooldownSeconds)
        {
            var redisService = RedisService.GetInstance();
            var redisKey = "ForgotPasswordEmailTimer:" + email;
            var redisDb = redisService.GetDatabase(ApConstant.SearchResultCacheName);
            var result = redisDb.KeyExists(redisKey);
            var resendCooldown = redisDb.StringGetWithExpiry(redisKey).Expiry;

            if (result == false)
            {
                resendCooldownSeconds = null;
                return true;

            }
            else
            {
                resendCooldownSeconds = Convert.ToInt32(resendCooldown.Value.TotalSeconds) + 30;
                return false;
            }
        }

        public void SendOtpToUserByEmail(string email, string otp)
        {
            var activityQueue = QueueService.GetInstance().GetQueueByReference("ForgotPasswordByOtpEmail");
            activityQueue.AddMessage(new CloudQueueMessage(email + ":" + otp));
        }
    }
}
