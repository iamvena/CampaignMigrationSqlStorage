using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MigrationSqlStorage
{
    public class DefaultConnectionProvider
    {
        private static IMongoDatabase _db = null;

        private static object _syncRoot = new object();

        private const string ConnectionString = "MacHaik.Campaigns.DataStore.Services.MongoConnectionString";

        private const string ConnectionDb = "MacHaik.Campaigns.DataStore.Services.MongoDatabaseName";

        private static void Initialize()
        {
            var mongoCn = ConfigurationManager.AppSettings[ConnectionString];

            var mongoDb = ConfigurationManager.AppSettings[ConnectionDb];

            if (string.IsNullOrEmpty(mongoCn))
            {
                throw new Exception($"{mongoCn} is not set.");
            }
            else if (string.IsNullOrEmpty(mongoDb))
            {
                throw new Exception($"{mongoDb} is not set.");
            }

            Console.WriteLine($"ConnectionString:{mongoCn}");

            Console.WriteLine($"Database:{mongoDb}");

            if (_db == null)
            {
                lock (_syncRoot)
                {
                    if (_db == null)
                    {
                        _db = new MongoClient(mongoCn).GetDatabase(mongoDb);
                    }
                }
            }
        }

        public static IMongoDatabase GetInstance()
        {
            Initialize();

            return _db;
        }
    }
}
