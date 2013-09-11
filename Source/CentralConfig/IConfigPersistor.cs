namespace CentralConfig
{
    public interface IConfigPersistor
    {
        string ReadAppSetting( string key, string machine, long version );
        string ReadAppSetting( string key, string machine );
        string ReadAppSetting( string key, long version );
        string ReadAppSetting( string key );

        TResult GetSection<TResult>( string sectionName, string machine, long version );
        TResult GetSection<TResult>( string sectionName, string machine );
        TResult GetSection<TResult>( string sectionName, long version );
        TResult GetSection<TResult>( string sectionName );
    }
}
