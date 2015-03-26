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
        private readonly IVBinAssemblyResolver m_assemblyResolver;
        private readonly IConfigPersistor m_defaultPersistor;

        public IConfigPersistor Persistor
        {
            get
            {
                return m_defaultPersistor ?? ObjectFactory.GetInstance<IConfigPersistor>();
            }
        }

        public ConfigManagerCore()
            : this( null )
        {
        }

        public ConfigManagerCore( IConfigPersistor defaultPersistor )
        {
            m_environment = ObjectFactory.GetInstance<IEnvironment>();
            m_defaultPersistor = defaultPersistor;
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
            var section = Persistor.GetSection<T>( sectionName, m_environment.MachineName, m_assemblyResolver.CurrentVersion );

            if( section != null )
            {
                return section;
            }

            section = Persistor.GetSection<T>( sectionName, m_environment.MachineName );

            if( section != null )
            {
                return section;
            }

            section = Persistor.GetSection<T>( sectionName, m_assemblyResolver.CurrentVersion );

            if( section != null )
            {
                return section;
            }

            return Persistor.GetSection<T>( sectionName );
        }

        private string ReadAppSetting( string key )
        {
            var value = Persistor.ReadAppSetting( key, m_environment.MachineName, m_assemblyResolver.CurrentVersion );

            if( value != null )
            {
                return value;
            }

            value = Persistor.ReadAppSetting( key, m_environment.MachineName );

            if( value != null )
            {
                return value;
            }

            value = Persistor.ReadAppSetting( key, m_assemblyResolver.CurrentVersion );

            if( value != null )
            {
                return value;
            }

            return Persistor.ReadAppSetting( key );
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