using System;
using StructureMap;
using VBin;
using VBin.Manager;

namespace CentralConfig
{
    /// <summary>
    /// Replacement for System.ConfigurationManager to enable centralised configuration
    /// Can load settings from MongoDB and have machine specific configs
    /// </summary>
    public class ConfigManagerCore
    {
        private readonly ConfigAppSettings m_appSettings;
        private readonly IEnvironment m_environment;
        private readonly IConfigPersistor m_persistor;
        private readonly IVBinAssemblyResolver m_assemblyResolver;

        public ConfigManagerCore()
            : this( null )
        {
        }

        public ConfigManagerCore( IConfigPersistor defaultPersistor )
        {
            m_environment = ObjectFactory.GetInstance<IEnvironment>();
            m_persistor = defaultPersistor ?? ObjectFactory.GetInstance<IConfigPersistor>();
            m_assemblyResolver = VBinManager.Resolver;

            m_appSettings = new ConfigAppSettings( this );
        }

        public ConfigAppSettings AppSettings
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

        public class ConfigAppSettings
        {
            private readonly ConfigManagerCore m_config;

            public ConfigAppSettings( ConfigManagerCore config )
            {
                if( config == null )
                {
                    throw new ArgumentNullException( "config" );
                }

                m_config = config;
            }

            public string this[string key]
            {
                get { return m_config.ReadAppSetting( key ); }
            }
        }
    }
}