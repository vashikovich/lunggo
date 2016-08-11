﻿using System.Collections.Generic;

namespace Lunggo.Framework.Notification
{
    public partial class NotificationService
    {
        private static readonly NotificationService Instance = new NotificationService();
        private bool _isInitialized;
        private static readonly AzureNotificationClient Client = AzureNotificationClient.GetClientInstance();

        private NotificationService()
        {
            
        }
        public void Init(string connString, string hubName)
        {
            if (!_isInitialized)
            {
                Client.Init(connString, hubName);
                _isInitialized = true;
            }
        }
        public static NotificationService GetInstance()
        {
            return Instance;
        }

        public string RegisterDevice(string notificationHandle, string deviceId)
        {
            return Client.RegisterDevice(notificationHandle, deviceId);
        }

        public bool SetTags(string registrationId, string notificationHandle, Platform platform,
            IEnumerable<string> tags)
        {
            return Client.SetTags(registrationId, notificationHandle, platform, tags);
        }

        public bool AddTags(string registrationId, string notificationHandle, Platform platform,
            IEnumerable<string> tags)
        {
            return Client.AddTags(registrationId, notificationHandle, platform, tags);
        }

        public void DeleteRegistration(string registrationId)
        {
            Client.DeleteRegistration(registrationId);
        }
    }
}
