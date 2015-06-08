﻿using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunggo.Framework.TableStorage
{
    public partial class TableStorageService
    {
        private static readonly TableStorageService Instance = new TableStorageService();
        private bool _isInitialized;
        private static readonly AzureTableStorageClient Client = AzureTableStorageClient.GetClientInstance();

        private TableStorageService()
        {
            
        }

        public void Init()
        {
            if (!_isInitialized)
            {
                Client.Init();
                _isInitialized = true;
            }
            else
            {
                throw new InvalidOperationException("TableStorageService is already initialized");
            }
        }
        public static TableStorageService GetInstance()
        {
            return Instance;
        }

        public CloudTable GetTableByReference(string reference)
        {
            return Client.GetTableByReference(reference);
        }
        public void InsertEntityToTableStorage<T>(T objectParam, string nameReference) where T : ITableEntity, new()
        {
            Client.InsertEntityToTableStorage(objectParam, nameReference);
        }
        public void InsertOrReplaceEntityToTableStorage<T>(T objectParam, string nameReference) where T : ITableEntity, new()
        {
            Client.InsertOrReplaceEntityToTableStorage(objectParam, nameReference);
        }
    }
}
