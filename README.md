# CentralConfig - Multi-machine configuration manager
 
## Overview

CentralConfig is a replacement for the built in .net ConfigurationManager. It makes managing multi-machine configurations simple while, from a usability point of view, looking very similar to ConfigurationManager.


## Features

  * All config must be able to be stored in version control
  * A way to have config centralised. I.e. you should not have to update the same setting on multiple machines.
  * Support for multiple environments (dev, build, test, prod etc)
  * A sane and simple way of having default values. You should not have to specify the same settings for each machine if a default will work. 
  * Override a setting for a particular machine. e.g. one machine in an environment must have a slightly different config. 
  * Simple and light weight
  * Support for simple values as well as more complex object types
  * A full replacement for ConfigManager 


## What about the .net config transformer

The .net transformer does a reasonable job but did not quite do everything I required. The examples below should show some of the differences


## Using CentralConfig

Install "CentralConfig" from nuget
Unless you have created a custom persistor use the mongo settings persistor
Set the central config mongo settings
Setup DI

### app.settings for the mong persistor

```xml
   <appSettings>
     <add key="MongoDB.Server" value="mongodb://localhost:27017" />
     <add key="CentralConfig.MongoDatabase" value="TestConfig" />
   </appSettings>
```

### DI setup

```csharp
   ObjectFactory.Configure( x =>
   {
       x.For<IEnvironment>().Use<SystemEnvironment>();
       x.For<IConfigPersistor>().Use<MongoDbConfigPersistor>();
   } );
````

### Configure settings in Mongo

```javascript
   {
       "key" : "TestSetting",
       "machine" : null,
       "value" : "some value"
   }
```

### Use the setting from code

```csharp
   //ConfigManager.AppSettings["TestSetting"]
   Console.WriteLine( "test: {0}", ConfigManager.AppSettings["TestSetting"] );
```


## Different Persistors

I've always used the mongo persistor but you can easily create new persistors as required. To do so implement the IPersistor interface

```csharp
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
```


## Order of matching

When you request a value, CentralConfig will try find the most specific value it can. So it will first look for the setting match both the key and the current machine name, if that does not match then it looks for a value matching on key only

```javascript
   {
       "key" : "TestSetting",
       "machine" : null,
       "value" : "some value"
   },
   {
       "key" : "TestSetting",
       "machine" : "MYPC",
       "value" : "another value"
   }
```

Given the mongo settings above when you request "TestSetting" you will get "another value" if your machine name is MYPC or "some value otherwise"
