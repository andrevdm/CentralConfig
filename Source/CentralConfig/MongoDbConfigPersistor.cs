using System.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace CentralConfig
{
    public class MongoDbConfigPersistor : IConfigPersistor
    {
        public string ReadAppSetting( string key, string machine, long version )
        {
            var db = GetConfigDatabase();
            var col = db.GetCollection( "ConfigValue" );

            var result = col.FindOneAs<ConfigValue>(
                Query.And(
                    Query.EQ( "key", key ),
                    Query.EQ( "machine", machine ),
                    Query.EQ( "version", version ) ) );

            return result != null ? result.value : null;
        }

        public string ReadAppSetting( string key, string machine )
        {
            var db = GetConfigDatabase();
            var col = db.GetCollection( "ConfigValue" );

            var result = col.FindOneAs<ConfigValue>(
                Query.And(
                    Query.EQ( "key", key ),
                    Query.EQ( "machine", machine ),
                    Query.EQ( "version", BsonNull.Value ) ) );

            return result != null ? result.value : null;
        }

        public string ReadAppSetting( string key, long version )
        {
            var db = GetConfigDatabase();
            var col = db.GetCollection( "ConfigValue" );

            var result = col.FindOneAs<ConfigValue>(
                Query.And(
                    Query.EQ( "key", key ),
                    Query.EQ( "machine", BsonNull.Value ),
                    Query.EQ( "version", version ) ) );

            return result != null ? result.value : null;
        }

        public string ReadAppSetting( string key )
        {
            var db = GetConfigDatabase();
            var col = db.GetCollection( "ConfigValue" );

            var result = col.FindOneAs<ConfigValue>(
                Query.And(
                    Query.EQ( "key", key ),
                    Query.EQ( "machine", BsonNull.Value ),
                    Query.EQ( "version", BsonNull.Value ) ) );

            return result != null ? result.value : null;
        }

        public TResult GetSection<TResult>( string sectionName, string machine, long version )
        {
            var db = GetConfigDatabase();
            var col = db.GetCollection( "ConfigSection" );

            var result = col.FindOneAs<ConfigSection<TResult>>(
                Query.And(
                    Query.EQ( "key", sectionName ),
                    Query.EQ( "machine", machine ),
                    Query.EQ( "version", version ) ) );

            return result != null ? result.sectionData : default(TResult);
        }

        public TResult GetSection<TResult>( string sectionName, string machine )
        {
            var db = GetConfigDatabase();
            var col = db.GetCollection( "ConfigSection" );

            var result = col.FindOneAs<ConfigSection<TResult>>(
                Query.And(
                    Query.EQ( "key", sectionName ),
                    Query.EQ( "machine", machine ),
                    Query.EQ( "version", BsonNull.Value ) ) );

            return result != null ? result.sectionData : default( TResult );
        }

        public TResult GetSection<TResult>( string sectionName, long version )
        {
            var db = GetConfigDatabase();
            var col = db.GetCollection( "ConfigSection" );

            var result = col.FindOneAs<ConfigSection<TResult>>(
                Query.And(
                    Query.EQ( "key", sectionName ),
                    Query.EQ( "machine", BsonNull.Value ),
                    Query.EQ( "version", version ) ) );

            return result != null ? result.sectionData : default( TResult );
        }

        public TResult GetSection<TResult>( string sectionName )
        {
            var db = GetConfigDatabase();
            var col = db.GetCollection( "ConfigSection" );

            var result = col.FindOneAs<ConfigSection<TResult>>(
                Query.And(
                    Query.EQ( "key", sectionName ),
                    Query.EQ( "machine", BsonNull.Value ),
                    Query.EQ( "version", BsonNull.Value ) ) );

            return result != null ? result.sectionData : default( TResult );
        }

        private MongoDatabase GetConfigDatabase()
        {
            string con = ConfigurationManager.AppSettings["MongoDB.Server"];
            var client = new MongoClient( con );
            var server = client.GetServer();
            var db = server.GetDatabase( ConfigurationManager.AppSettings["CentralConfig.MongoDatabase"] );
            return db;
        }

        private class ConfigValue
        {
            public ObjectId Id;
            public string key;
            public string machine;
            public int? version;
            public string value;
        }

        private class ConfigSection<T>
        {
            public ObjectId Id;
            public string key;
            public string machine;
            public int? version;
            public T sectionData;
        }
    }
}
