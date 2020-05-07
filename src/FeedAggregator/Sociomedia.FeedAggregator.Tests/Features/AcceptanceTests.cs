﻿using System;
using CQRSlite.Events;
using EventStore.ClientAPI;
using NSubstitute;
using Sociomedia.Domain.Articles;
using Sociomedia.FeedAggregator.Application.Queries;
using Sociomedia.Infrastructure;
using Sociomedia.Infrastructure.CQRS;
using StructureMap;

namespace Sociomedia.FeedAggregator.Tests.Features
{
    public abstract class AcceptanceTests
    {
        protected readonly ICommandDispatcher CommandDispatcher;
        protected readonly InMemoryEventStore EventStore;
        protected readonly IMediaFeedFinder MediaFeedFinder;
        protected readonly Container Container;

        protected AcceptanceTests()
        {
            Container = ContainerBuilder.Build();

            Container.Inject<ILogger>(new EmptyLogger());
            Container.Inject(Substitute.For<IHtmlPageDownloader>());
            Container.Inject<IEventStore>(Container.GetInstance<InMemoryEventStore>());

            MediaFeedFinder = Container.GetInstance<IMediaFeedFinder>();
            CommandDispatcher = Container.GetInstance<ICommandDispatcher>();
            EventStore = (InMemoryEventStore) Container.GetInstance<IEventStore>();
        }

        private class EmptyLogger : ILogger
        {
            public void Error(string format, params object[] args) { }

            public void Error(Exception ex, string format, params object[] args) { }

            public void Info(string format, params object[] args) { }

            public void Info(Exception ex, string format, params object[] args) { }

            public void Debug(string format, params object[] args) { }

            public void Debug(Exception ex, string format, params object[] args) { }
        }
    }
}