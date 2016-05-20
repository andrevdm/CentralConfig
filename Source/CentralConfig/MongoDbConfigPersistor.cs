using System;
using System.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace CentralConfig
{
    public class MongoDbConfigPersistor : IConfigPersistor
    {
        private readonly ConfigMemoizer m_memoizer;

        public MongoDbConfigPersistor( bool memoize = false )
        {
            var configPersistor = new MongoDbConfigPersistorImpl();
            m_memoizer = new ConfigMemoizer( memoize, configPersistor );
        }

        public MongoDbConfigPersistor( string mongoConStr, string mongoDatabase, bool memoize = false )
        {
            var configPersistor = new MongoDbConfigPersistorImpl(mongoConStr, mongoDatabase);
            m_memoizer = new ConfigMemoizer( memoize, configPersistor );
        }

        public string ReadAppSetting( string key, string machine, long version )
        {
            return m_memoizer.ReadAppSetting( key, machine, version );
        }

        public string ReadAppSetting( string key, string machine )
        {
            return m_memoizer.ReadAppSetting( key, machine );
        }

        public string ReadAppSetting( string key, long version )
        {
            return m_memoizer.ReadAppSetting( key, version );
        }

        public string ReadAppSetting( string key )
        {
            return m_memoizer.ReadAppSetting( key );
        }

        public TResult GetSection<TResult>( string sectionName, string machine, long version )
        {
            return m_memoizer.GetSection<TResult>( sectionName, machine, version );
        }

        public TResult GetSection<TResult>( string sectionName, string machine )
        {
            return m_memoizer.GetSection<TResult>( sectionName, machine );
        }

        public TResult GetSection<TResult>( string sectionName, long version )
        {
            return m_memoizer.GetSection<TResult>( sectionName, version );
        }

        public TResult GetSection<TResult>( string sectionName )
        {
            return m_memoizer.GetSection<TResult>( sectionName );
        }
    }

    public class MongoDbConfigPersistorImpl : IConfigPersistor
    {
        private readonly string m_mongoConStr;
        private readonly string m_mongoDatabase;

        public MongoDbConfigPersistorImpl()
        {
            m_mongoConStr = ConfigurationManager.AppSettings["MongoDB.Server"];
            m_mongoDatabase = ConfigurationManager.AppSettings["CentralConfig.MongoDatabase"];
        }

        public MongoDbConfigPersistorImpl( string mongoConStr, string mongoDatabase )
        {
            if( mongoConStr == null )
                throw new ArgumentNullException( nameof( mongoConStr ) );

            if( mongoDatabase == null )
                throw new ArgumentNullException( nameof( mongoDatabase ) );

            m_mongoConStr = mongoConStr;
            m_mongoDatabase = mongoDatabase;
        }

        public string ReadAppSetting( string key, string machine, long version )
        {
            var db = GetConfigDatabase();
            var col = db.GetCollection( "ConfigValue" );

            var result = col.FindOneAs<ConfigValue>(
                Query.And(
                    Query.EQ( "key", key ),
                    Query.EQ( "machine", machine ),
                    Query.EQ( "version", version ) ) );

            return result?.value;
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

            return result?.value;
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

            return result?.value;
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

            return result?.value;
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

            return result != null ? result.sectionData : default( TResult );
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
            string con = m_mongoConStr;
            var client = new MongoClient( con );
            var server = client.GetServer();
            var db = server.GetDatabase( m_mongoDatabase );
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
