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
        protected IConfigurationRoot builder;

        protected CloudServiceFactory()
        {
            builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .Build();
        }

        public abstract CloudService CreateCloudService();
    }
}