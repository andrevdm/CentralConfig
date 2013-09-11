using System;

namespace CentralConfig
{
    /// <summary>
    /// IEnvironment implementation that uses Syste.Environment
    /// </summary>
    public class SystemEnvironment : IEnvironment
    {
        public string MachineName
        {
            get { return Environment.MachineName; }
        }
    }
}
