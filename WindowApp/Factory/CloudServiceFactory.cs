using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowApp.Factory
{
    public abstract class CloudServiceFactory
    {
        protected static readonly IConfigurationRoot Builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName)
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        public abstract ICloudService CreateCloudService();
    }
}