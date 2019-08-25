﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAGrim.Backup.Azure.Constants {
    static class AzureUris {
        public const string EnvDev = "dev";
        public const string EnvAzure = "azure";
        
        // https://docs.microsoft.com/en-us/azure/app-service/app-service-authentication-overview
        public static void Initialize(string env) {
            if (env == EnvDev) {
                AuthenticateUrl = "http://localhost:7071/api/Authenticate";
                TokenVerificationUri = "http://localhost:7071/api/VerifyToken";
                FetchPartitionUrl = "http://localhost:7071/api/v3_Partitions";
                FetchItemsInPartitionUrl = "http://localhost:7071/api/v3_Download";
                UploadItemsUrl = "http://localhost:7071/api/v3_Upload";
                DeleteItemsUrl = "http://localhost:7071/api/v2_Remove";
                FetchLimitationsUrl = "http://localhost:7071/api/v2_Limits";
                DeleteAccountUrl = "http://localhost:7071/api/v3_Delete";
            }
            else if (env == EnvAzure) {
                AuthenticateUrl = "http://grimdawn.dreamcrash.org/ia/backup/";
                TokenVerificationUri = "https://iagd.azurewebsites.net/api/VerifyToken";
                FetchPartitionUrl = "https://iagd.azurewebsites.net/api/v3_Partitions";
                FetchItemsInPartitionUrl = "https://iagd.azurewebsites.net/api/v3_Download";
                UploadItemsUrl = "https://iagd.azurewebsites.net/api/v3_Upload";
                DeleteItemsUrl = "https://iagd.azurewebsites.net/api/v2_Remove";
                FetchLimitationsUrl = "https://iagd.azurewebsites.net/api/v2_Limits";
                DeleteAccountUrl = "https://iagd.azurewebsites.net/api/v3_Delete";
            }
            else {
                throw new ArgumentException(env);
            }
        }

        public static string AuthenticateUrl { get; private set; }
        public static string TokenVerificationUri { get; private set; }
        public static string FetchPartitionUrl { get; private set; }
        public static string FetchItemsInPartitionUrl { get; private set; }
        public static string UploadItemsUrl { get; private set; }
        public static string DeleteItemsUrl { get; private set; }
        public static string FetchLimitationsUrl { get; private set; }
        public static string DeleteAccountUrl { get; private set; }
    }
}
