using System.Globalization;
using System.Timers;
using Shared.Factory;
using Shared.models;
using Shared.Services;
using Shared.util;
using Xunit.Abstractions;
using Timer = System.Timers.Timer;

namespace Test
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void GetSynchronizedFolderList()
        {
            var syncFolders = ConfigIO.ReadSynchronizedFiles();

            output.WriteLine(syncFolders.ToString());
        }

        [Fact]
        public async void GenericTest()
        {

         
        }
    }
}