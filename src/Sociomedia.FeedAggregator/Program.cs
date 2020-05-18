﻿using System.Threading.Tasks;

namespace Sociomedia.FeedAggregator
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var configuration = ConfigurationManager<Configuration>.Read();

            var container = ContainerBuilder.Build(configuration);

            var application = container.GetInstance<FeedAggregatorApplication>();

            await application.Run();
        }
    }
}