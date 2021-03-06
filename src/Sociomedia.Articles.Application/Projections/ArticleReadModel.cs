﻿using System;

namespace Sociomedia.Articles.Application.Projections {
    public class ArticleReadModel
    {
        public Guid ArticleId { get; set; }
        public string ExternalArticleId { get; set; }
        public Guid MediaId { get; set; }
        public DateTimeOffset PublishDate { get; set; }
    }
}