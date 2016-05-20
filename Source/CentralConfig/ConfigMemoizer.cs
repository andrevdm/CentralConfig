using System;
using System.Collections.Concurrent;

namespace CentralConfig
{
    public class ConfigMemoizer : IConfigPersistor
    {
        private readonly bool m_enable;
        private readonly IConfigPersistor m_configPersistor;
        private readonly ConcurrentDictionary<Tuple<string, string, long>, string> m_values = new ConcurrentDictionary<Tuple<string, string, long>, string>();
        private readonly ConcurrentDictionary<Tuple<string, string, long>, object> m_sections = new ConcurrentDictionary<Tuple<string, string, long>, object>();

        public ConfigMemoizer( bool enable, IConfigPersistor configPersistor )
        {
            if( configPersistor == null )
                throw new ArgumentNullException( nameof( configPersistor ) );

            m_enable = enable;
            m_configPersistor = configPersistor;
        }

        private string ReadAppSetting( Tuple<string, string, long> key, Func<string> read )
        {
            if( !m_enable )
                return read();

            string value;
            if( m_values.TryGetValue( key, out value ) )
                return value;

            value = read();
            m_values[key] = value;
            return value;
        }

        private object ReadSection( Tuple<string, string, long> key, Func<object> read )
        {
            if( !m_enable )
                return read();

            object value;
            if( m_sections.TryGetValue( key, out value ) )
                return value;

            value = read();
            m_sections[key] = value;
            return value;
        }

        public string ReadAppSetting( string key, string machine, long version )
        {
            return ReadAppSetting( new Tuple<string, string, long>( key, machine, version ), () => m_configPersistor.ReadAppSetting( key, machine, version ) );
        }

        public string ReadAppSetting( string key, string machine )
        {
            return ReadAppSetting( new Tuple<string, string, long>( key, machine, long.MinValue ), () => m_configPersistor.ReadAppSetting( key, machine ) );
        }

        public string ReadAppSetting( string key, long version )
        {
            return ReadAppSetting( new Tuple<string, string, long>( key, "", version ), () => m_configPersistor.ReadAppSetting( key, version ) );
        }

        public string ReadAppSetting( string key )
        {
            return ReadAppSetting( new Tuple<string, string, long>( key, "", long.MinValue ), () => m_configPersistor.ReadAppSetting( key ) );
        }

        public TResult GetSection<TResult>( string sectionName, string machine, long version )
        {
            return (TResult)ReadSection( new Tuple<string, string, long>( sectionName, machine, version ), () => m_configPersistor.GetSection<TResult>( sectionName, machine, version ) );
        }

        public TResult GetSection<TResult>( string sectionName, string machine )
        {
            return (TResult)ReadSection( new Tuple<string, string, long>( sectionName, machine, long.MinValue ), () => m_configPersistor.GetSection<TResult>( sectionName, machine ) );
        }

        public TResult GetSection<TResult>( string sectionName, long version )
        {
            return (TResult)ReadSection( new Tuple<string, string, long>( sectionName, "", version ), () => m_configPersistor.GetSection<TResult>( sectionName, version ) );
        }

        public TResult GetSection<TResult>( string sectionName )
        {
            return (TResult)ReadSection( new Tuple<string, string, long>( sectionName, "", long.MinValue ), () => m_configPersistor.GetSection<TResult>( sectionName ) );
        }
    }
}