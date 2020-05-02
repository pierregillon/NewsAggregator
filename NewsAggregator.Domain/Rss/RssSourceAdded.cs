﻿using System;

namespace NewsAggregator.Domain.Rss {
    public class RssSourceAdded : DomainEvent
    {
        public RssSourceAdded(Guid aggregateId, Uri url)
        {
            Id = aggregateId;
            Url = url;
        }

        public Uri Url { get; }
    }
}