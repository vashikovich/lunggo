﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Lunggo.Framework.SnowMaker
{
    public class BlobOptimisticDataStore : IOptimisticDataStore
    {
        const string DefaultSeedValue = "1";

        readonly CloudBlobContainer _blobContainer;

        readonly IDictionary<string, ICloudBlob> _blobReferences;
        readonly object _blobReferencesLock = new object();

        public Func<String, long> SeedValueInitializer { get; set; }

        public BlobOptimisticDataStore(CloudStorageAccount account, string containerName)
        {
            var blobClient = account.CreateCloudBlobClient();
            _blobContainer = blobClient.GetContainerReference(containerName.ToLower());
            _blobContainer.CreateIfNotExists();

            _blobReferences = new Dictionary<string, ICloudBlob>();
        }

        public string GetData(string blockName)
        {
            var blobReference = GetBlobReference(blockName);
            using (var stream = new MemoryStream())
            {
                blobReference.DownloadToStream(stream);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public bool TryOptimisticWrite(string scopeName, string data)
        {
            var blobReference = GetBlobReference(scopeName);
            try
            {
                UploadText(
                    blobReference,
                    data,
                    AccessCondition.GenerateIfMatchCondition(blobReference.Properties.ETag));
            }
            catch (StorageException exc)
            {
                if (exc.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed)
                    return false;

                throw;
            }
            return true;
        }

        ICloudBlob GetBlobReference(string blockName)
        {
            return _blobReferences.GetValue(
                blockName,
                _blobReferencesLock,
                () => InitializeBlobReference(blockName));
        }

        private ICloudBlob InitializeBlobReference(string blockName)
        {
            var blobReference = _blobContainer.GetBlockBlobReference(blockName);

            if (blobReference.Exists())
                return blobReference;

            var seedValue = SeedValueInitializer == null ? DefaultSeedValue : SeedValueInitializer.Invoke(blockName).ToString(CultureInfo.InvariantCulture);

            try
            {
                UploadText(blobReference, seedValue, AccessCondition.GenerateIfNoneMatchCondition("*"));
            }
            catch (StorageException uploadException)
            {
                if (uploadException.RequestInformation.HttpStatusCode != (int)HttpStatusCode.Conflict)
                    throw;
            }

            return blobReference;
        }

        void UploadText(ICloudBlob blob, string text, AccessCondition accessCondition)
        {
            blob.Properties.ContentEncoding = "UTF-8";
            blob.Properties.ContentType = "text/plain";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                blob.UploadFromStream(stream, accessCondition);
            }
        }
    }
}