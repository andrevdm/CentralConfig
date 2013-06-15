namespace Avdm.Config
{
    /// <summary>
    /// Replacement for System.ConfigurationManager to enable centralised configuration
    /// Can load settings from MongoDB and have machine specific configs
    /// NetTpConfigManager is the static helper, the NetTpConfigManagerCore implements the actual functionality
    /// </summary>
    public static class ConfigManager
    {
        private static readonly ConfigManagerCore g_settings;

        static ConfigManager()
        {
            g_settings = new ConfigManagerCore();
        }

        public static ConfigManagerCore.NetTpAppSettings AppSettings
        {
            get { return g_settings.AppSettings; }
        }

        public static TResult GetSection<TResult>( string sectionName )
            where TResult : class
        {
            return g_settings.GetSection<TResult>( sectionName );
        }
    }
}
