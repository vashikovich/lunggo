﻿using System.Web;
using Lunggo.ApCommon.Constant;
using Lunggo.ApCommon.Dictionary;
using Lunggo.ApCommon.Flight.Service;
using Lunggo.Framework.Config;
using Lunggo.Framework.Queue;
using Lunggo.Framework.I18nMessage;
using Lunggo.Framework.Redis;
using Lunggo.Framework.SnowMaker;
using Microsoft.WindowsAzure.Storage;
using Lunggo.Framework.Database;


namespace Lunggo.BackendWeb
{
    public class AppInitializer
    {
        public static void Init()
        {
            InitConfigurationManager();
            InitI18NMessageManager();
            InitDatabaseService();
            InitRedisService();
            InitQueueService();
            //InitLogger();
            InitDictionaryService();
            InitFlightService();
            InitUniqueIdGenerator();
        }

        private static void InitUniqueIdGenerator()
        {
            var generator = UniqueIdGenerator.GetInstance();
            var seqContainerName = ConfigManager.GetInstance().GetConfigValue("general", "seqGeneratorContainerName");
            var storageConnectionString = ConfigManager.GetInstance().GetConfigValue("azureStorage", "connectionString");
            var optimisticData = new BlobOptimisticDataStore(CloudStorageAccount.Parse(storageConnectionString), seqContainerName)
            {
                SeedValueInitializer = (sequenceName) => generator.GetIdInitialValue(sequenceName)
            };
            generator.Init(optimisticData);
            generator.BatchSize = 100;
        }

        private static void InitConfigurationManager()
        {
            var configManager = ConfigManager.GetInstance();
            var configDirectoryPath = HttpContext.Current.Server.MapPath(@"~/Config/");
            configManager.Init(configDirectoryPath);
        }

        private static void InitI18NMessageManager()
        {
            var messageManager = MessageManager.GetInstance();
            messageManager.Init("Config");
        }

        private static void InitDatabaseService()
        {
            var connString = ConfigManager.GetInstance().GetConfigValue("db", "connectionString");
            var database = DbService.GetInstance();
            database.Init(connString);
        }
        private static void InitQueueService()
        {
            var connString = ConfigManager.GetInstance().GetConfigValue("azureStorage", "connectionString");
            var queue = QueueService.GetInstance();
            queue.Init(connString);
        }

        private static void InitRedisService()
        {
            var redisService = RedisService.GetInstance();
            redisService.Init(new RedisConnectionProperty[]
            {
                new RedisConnectionProperty
                {
                    ConnectionName = ApConstant.SearchResultCacheName,
                    ConnectionString = ConfigManager.GetInstance().GetConfigValue("redis", "searchResultCacheConnectionString")
                },
                
                new RedisConnectionProperty
                {
                    ConnectionName = ApConstant.MasterDataCacheName,
                    ConnectionString = ConfigManager.GetInstance().GetConfigValue("redis", "masterDataCacheConnectionString")
                }, 
                 
            });
        }

        private static void InitDictionaryService()
        {
            var dictionary = DictionaryService.GetInstance();
            dictionary.Init("Config");
        }

        private static void InitFlightService()
        {
            var flight = FlightService.GetInstance();
            flight.Init();
        }
    }
}