using Avdm.Core;
using Avdm.Core.Di;
using Avdm.Deploy.Sbin;
using StructureMap;

namespace Avdm.Config
{
    /// <summary>
    /// Replacement for System.ConfigurationManager to enable centralised configuration
    /// Can load settings from MongoDB and have machine specific configs
    /// </summary>
    public class ConfigManagerCore
    {
        private readonly NetTpAppSettings m_appSettings;
        private readonly IEnvironment m_environment;
        private readonly IConfigPersistor m_persistor;
        private readonly ISbinAssemblyResolver m_assemblyResolver;

        public ConfigManagerCore()
            : this( null )
        {
        }

        public ConfigManagerCore( IConfigPersistor defaultPersistor )
        {
            m_environment = ObjectFactory.GetInstance<IEnvironment>();
            m_persistor = defaultPersistor ?? ObjectFactory.GetInstance<IConfigPersistor>();
            m_assemblyResolver = ObjectFactory.GetInstance<ISbinAssemblyResolver>();

            m_appSettings = new NetTpAppSettings( this );
        }

        public NetTpAppSettings AppSettings
        {
            get { return m_appSettings; }
        }

        public T GetSection<T>( string sectionName )
            where T : class
        {
            var section = m_persistor.GetSection<T>( sectionName, m_environment.MachineName, m_assemblyResolver.CurrentVersion );

            if( section != null )
            {
                return section;
            }

            section = m_persistor.GetSection<T>( sectionName, m_environment.MachineName );

            if( section != null )
            {
                return section;
            }

            section = m_persistor.GetSection<T>( sectionName, m_assemblyResolver.CurrentVersion );

            if( section != null )
            {
                return section;
            }

            return m_persistor.GetSection<T>( sectionName );
        }

        private string ReadAppSetting( string key )
        {
            var value = m_persistor.ReadAppSetting( key, m_environment.MachineName, m_assemblyResolver.CurrentVersion );

            if( value != null )
            {
                return value;
            }

            value = m_persistor.ReadAppSetting( key, m_environment.MachineName );

            if( value != null )
            {
                return value;
            }

            value = m_persistor.ReadAppSetting( key, m_assemblyResolver.CurrentVersion );

            if( value != null )
            {
                return value;
            }

            return m_persistor.ReadAppSetting( key );
        }

        public class NetTpAppSettings
        {
            private readonly ConfigManagerCore m_config;

            public NetTpAppSettings( ConfigManagerCore config )
            {
                Preconditions.CheckNotNull( config, "config" );

                m_config = config;
            }

            public string this[string key]
            {
                get { return m_config.ReadAppSetting( key ); }
            }
        }
    }
}