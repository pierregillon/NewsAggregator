﻿using System;
using System.Collections.Generic;

namespace NewsAggregator.Domain.Articles
{
    public class ArticleCreated : DomainEvent
    {
        public ArticleCreated(Guid id, string name, Uri url, IReadOnlyCollection<Keyword> keywords, Guid rssSourceId)
        {
            Id = id;
            Name = name;
            Url = url;
            Keywords = keywords;
            RssSourceId = rssSourceId;
        }

        public string Name { get; }
        public IReadOnlyCollection<Keyword> Keywords { get; }
        public Uri Url { get; }
        public Guid RssSourceId { get; }
    }
}