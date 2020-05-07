﻿using System;
using Sociomedia.DomainEvents;

namespace Sociomedia.FeedAggregator.Domain.Themes
{
    public class ArticleAddedToTheme : DomainEvent
    {
        public ArticleAddedToTheme(Guid themeId, ThemeArticle article) : base(themeId, "theme")
        {
            Article = article;
        }

        public ThemeArticle Article { get; }
    }
}