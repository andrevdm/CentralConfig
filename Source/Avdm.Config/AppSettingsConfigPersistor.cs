using System.Configuration;

namespace Avdm.Config
{
    /// <summary>
    /// Reads config settings from the application's config file
    /// </summary>
    /// <remarks>
    /// Note that you can have machine specific app settings. 
    /// Use the format 
    ///    key="!eee!$nnn!_key"
    ///    or
    ///    key="!eee!_key"
    ///    or
    ///    key="!$nnn!_key"
    ///        where eee = machine name
    ///        where nnn = version
    ///        where key = key name
    /// 
    /// E.g. 
    ///   Default value for all versions
    ///     key="test" value="defaultValue"
    ///   Default value for version 1
    ///     key="test" value="!$1!_defaultValue"
    ///   Value for version 2 on svr1
    ///     key="!svr1!$2!_test" value="svr1Value"
    ///   Default for all versions on NetTpdev1
    ///     key="!NetTpdev1!_test" value="NetTpdev1Value"
    /// 
    /// The machine name is ready from the IEnvironment implementor which by default will read from Environment.MachineName
    /// </remarks>
    public class AppSettingsConfigPersistor : IConfigPersistor
    {
        public string ReadAppSetting( string key, string machine, long version )
        {
            return ConfigurationManager.AppSettings["!" + machine + "!$" + version + "!_" + key];
        }

        public string ReadAppSetting( string key, string machine )
        {
            return ConfigurationManager.AppSettings["!" + machine + "!_" + key];
        }

        public string ReadAppSetting( string key, long version )
        {
            return ConfigurationManager.AppSettings["!$" + version + "!_" + key];
        }

        public string ReadAppSetting( string key )
        {
            return ConfigurationManager.AppSettings[key];
        }

        public TResult GetSection<TResult>( string sectionName, string machine, long version )
        {
            return (TResult)ConfigurationManager.GetSection( machine + "_v" + version + "_" + sectionName );
        }

        public TResult GetSection<TResult>( string sectionName, string machine )
        {
            return (TResult)ConfigurationManager.GetSection( machine + "_" + sectionName );
        }

        public TResult GetSection<TResult>( string sectionName, long version )
        {
            return (TResult)ConfigurationManager.GetSection( "_v" + version + "_" + sectionName );
        }

        public TResult GetSection<TResult>( string sectionName )
        {
            return (TResult)ConfigurationManager.GetSection( sectionName );
        }
    }
}
