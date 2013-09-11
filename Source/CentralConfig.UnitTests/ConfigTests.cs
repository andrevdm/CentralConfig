using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StructureMap;
using VBin;

namespace CentralConfig.UnitTests
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        public void ReadAppSettingsUsesMostSpecificValueFirst()
        {
            var environment = new Mock<IEnvironment>();

            var persistor = new Mock<IConfigPersistor>();
            persistor.Setup( p => p.ReadAppSetting( "someKey", "m1", 1 ) ).Returns( "someKey_v1_m1_123" );
            persistor.Setup( p => p.ReadAppSetting( "someKey", "m2" ) ).Returns( "someKey_default_m2_234" );
            persistor.Setup( p => p.ReadAppSetting( "someKey", 3 ) ).Returns( "someKey_v3_allMachines_345" );
            persistor.Setup( p => p.ReadAppSetting( "someKey" ) ).Returns( "someKey_default_456" );

            var assemblyResolver = new Mock<IVBinAssemblyResolver>();

            ObjectFactory.Configure( x =>
            {
                x.For<IEnvironment>().Singleton().Use( () => environment.Object );
                x.For<IConfigPersistor>().Singleton().Use( () => persistor.Object );
                x.For<IVBinAssemblyResolver>().Singleton().Use( () => assemblyResolver.Object );
            } );

            var config = new ConfigManagerCore();

            assemblyResolver.SetupGet( a => a.CurrentVersion ).Returns( 1 );
            environment.SetupGet( e => e.MachineName ).Returns( "m1" );
            Assert.AreEqual( "someKey_v1_m1_123", config.AppSettings["someKey"] );// "Expecting machine + version"

            assemblyResolver.SetupGet( a => a.CurrentVersion ).Returns( 1 );
            environment.SetupGet( e => e.MachineName ).Returns( "m2" );
            Assert.AreEqual( "someKey_default_m2_234", config.AppSettings["someKey"] );// "Expecting machine default"

            assemblyResolver.SetupGet( a => a.CurrentVersion ).Returns( 3 );
            environment.SetupGet( e => e.MachineName ).Returns( "mUnknown" );
            Assert.AreEqual( "someKey_v3_allMachines_345", config.AppSettings["someKey"] );// "Expecting default version"

            assemblyResolver.SetupGet( a => a.CurrentVersion ).Returns( 4 );
            environment.SetupGet( e => e.MachineName ).Returns( "mUnknown" );
            Assert.AreEqual( "someKey_default_456", config.AppSettings["someKey"] );// "Expecting default"

            assemblyResolver.SetupGet( a => a.CurrentVersion ).Returns( 4 );
            environment.SetupGet( e => e.MachineName ).Returns( "mUnknown" );
            Assert.IsNull( config.AppSettings["Unknown"] );//"Unknown value should return a null"
        }

       [TestMethod]
        public void GetSectionUsesMostSpecificValueFirst()
        {
            var environment = new Mock<IEnvironment>();

            var persistor = new Mock<IConfigPersistor>();
            persistor.Setup( p => p.GetSection<TestSection>( "someKey", "m1", 1 ) ).Returns( new TestSection( "someKey_v1_m1_123" ) );
            persistor.Setup( p => p.GetSection<TestSection>( "someKey", "m2" ) ).Returns( new TestSection( "someKey_default_m2_234" ) );
            persistor.Setup( p => p.GetSection<TestSection>( "someKey", 3 ) ).Returns( new TestSection( "someKey_v3_allMachines_345" ) );
            persistor.Setup( p => p.GetSection<TestSection>( "someKey" ) ).Returns( new TestSection( "someKey_default_456" ) );

            var assemblyResolver = new Mock<IVBinAssemblyResolver>();

            ObjectFactory.Configure( x =>
            {
                x.For<IEnvironment>().Singleton().Use( () => environment.Object );
                x.For<IConfigPersistor>().Singleton().Use( () => persistor.Object );
                x.For<IVBinAssemblyResolver>().Singleton().Use( () => assemblyResolver.Object );
            } );

            var config = new ConfigManagerCore();

            assemblyResolver.SetupGet( a => a.CurrentVersion ).Returns( 1 );
            environment.SetupGet( e => e.MachineName ).Returns( "m1" );
            Assert.AreEqual( "someKey_v1_m1_123", config.GetSection<TestSection>( "someKey" ).Name );//"Expecting machine + version"

            assemblyResolver.SetupGet( a => a.CurrentVersion ).Returns( 1 );
            environment.SetupGet( e => e.MachineName ).Returns( "m2" );
            Assert.AreEqual( "someKey_default_m2_234", config.GetSection<TestSection>( "someKey" ).Name );//"Expecting machine default"

            assemblyResolver.SetupGet( a => a.CurrentVersion ).Returns( 3 );
            environment.SetupGet( e => e.MachineName ).Returns( "mUnknown" );
            Assert.AreEqual( "someKey_v3_allMachines_345", config.GetSection<TestSection>( "someKey" ).Name );//"Expecting default version"

            assemblyResolver.SetupGet( a => a.CurrentVersion ).Returns( 4 );
            environment.SetupGet( e => e.MachineName ).Returns( "mUnknown" );
            Assert.AreEqual( "someKey_default_456", config.GetSection<TestSection>( "someKey" ).Name );//"Expecting default"

            assemblyResolver.SetupGet( a => a.CurrentVersion ).Returns( 4 );
            environment.SetupGet( e => e.MachineName ).Returns( "mUnknown" );
            Assert.IsNull( config.GetSection<TestSection>( "Unknown" ) ); // "Unknown value should return a null"
            Assert.IsNull( config.GetSection<ConfigTests>( "Unknown" ) ); //"Unknown type should return a null"
        }

        private class TestSection
        {
            public TestSection( string name )
            {
                Name = name;
            }

            public string Name { get; set; }
        }
    }
}
