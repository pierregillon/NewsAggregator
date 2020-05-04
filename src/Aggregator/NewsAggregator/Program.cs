﻿using System;
using System.Threading.Tasks;
using NewsAggregator.Application.Commands.AddRssSource;
using NewsAggregator.Application.Commands.SynchronizeRssSources;
using NewsAggregator.Infrastructure.CQRS;

#if !RELEASE
using CQRSlite.Events;
using NewsAggregator.Infrastructure;
#endif

namespace NewsAggregator
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var container = ContainerBuilder.Build();

#if !RELEASE
            var eventStore = (EventStoreOrg) container.GetInstance<IEventStore>();
            await eventStore.Connect("localhost");
#endif

            var commandDispatcher = container.GetInstance<ICommandDispatcher>();

            await commandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("https://www.lemonde.fr/rss/une.xml")));
            await commandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("https://www.marianne.net/rss_marianne.xml")));
            await commandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("http://rss.liberation.fr/rss/9/")));
            await commandDispatcher.Dispatch(new AddRssSourceCommand(new Uri("https://www.francetvinfo.fr/france.rss")));
            
            await commandDispatcher.Dispatch(new SynchronizeRssSourcesCommand());
            await commandDispatcher.Dispatch(new SynchronizeRssSourcesCommand());

            Console.WriteLine("ended.");
            Console.ReadKey();
        }
    }
}