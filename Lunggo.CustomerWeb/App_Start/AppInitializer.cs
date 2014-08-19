﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lunggo.Framework.Config;
using Lunggo.Framework.Message;
using Lunggo.Framework.SnowMaker;
using Microsoft.WindowsAzure.Storage;

namespace Lunggo.CustomerWeb
{
    public class AppInitializer
    {
        public static void Init()
        {
            InitConfigurationManager();
            InitI18NMessageManager();
            InitUniqueIdGenerator();
        }

        private static void InitConfigurationManager()
        {
            var configManager = ConfigManager.GetInstance();
            var configDirectoryPath = HttpContext.Current.Server.MapPath(@"~/Config/");
            configManager.Init(configDirectoryPath);
        }

        private static void InitI18NMessageManager()
        {
            var configDirectoryPath = HttpContext.Current.Server.MapPath(@"~/Config/");
            var messageManager = MessageManager.GetInstance();
            messageManager.Init(configDirectoryPath);
        }

        private static void InitUniqueIdGenerator()
        {
            var generator = UniqueIdGenerator.GetInstance();
            var seqContainerName = ConfigManager.GetInstance().GetConfigValue("general", "seqGeneratorContainerName");
            var storageConnectionString = ConfigManager.GetInstance().GetConfigValue("azurestorage", "connectionString");
            var optimisticData = new BlobOptimisticDataStore(CloudStorageAccount.Parse(storageConnectionString), seqContainerName)
            {
                SeedValueInitializer = (sequenceName) => generator.GetIdInitialValue(sequenceName)
            };
            generator.Init(optimisticData);
            generator.BatchSize = 100;
        }

    }
}