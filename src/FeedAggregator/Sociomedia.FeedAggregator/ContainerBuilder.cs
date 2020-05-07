﻿using Sociomedia.Application;
using Sociomedia.Domain.Medias;
using Sociomedia.FeedAggregator.Application.Queries;
using Sociomedia.FeedAggregator.Application.SynchronizeAllMediaFeeds;
using Sociomedia.FeedAggregator.Domain;
using Sociomedia.FeedAggregator.Infrastructure;
using Sociomedia.Infrastructure;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Graph.Scanning;

namespace Sociomedia.FeedAggregator
{
    public class ContainerBuilder
    {
        public static Container Build()
        {
            return new Container(registry => {
                registry.IncludeRegistry<SociomediaRegistry>();
                registry.For<IFeedParser>().Use<FeedParser>();
                registry.For<IFeedReader>().Use<FeedReader>();
                registry.For<IMediaFeedFinder>().Use<MediaFeedFinder>();
                registry.For<InMemoryDatabase>().Use<InMemoryDatabase>().Singleton();

                registry.Scan(scanner => {
                    scanner.AssemblyContainingType(typeof(SynchronizeAllMediaFeedsCommand));
                    scanner.Convention<AllInterfacesConvention>();
                    scanner.AddAllTypesOf(typeof(IEventListener<>));
                    scanner.AddAllTypesOf(typeof(ICommandHandler<>));
                });
            });
        }

        private class AllInterfacesConvention : IRegistrationConvention
        {
            public void ScanTypes(TypeSet types, Registry services)
            {
                foreach (var type in types.FindTypes(TypeClassification.Concretes | TypeClassification.Closed)) {
                    foreach (var @interface in type.GetInterfaces()) {
                        services.For(@interface).Use(type);
                    }
                }
            }
        }
    }
}