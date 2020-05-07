﻿using System;

namespace Sociomedia.DomainEvents.Media
{
    public class MediaFeedRemoved : MediaEvent
    {
        public MediaFeedRemoved(Guid mediaId, string feedUrl) : base(mediaId)
        {
            FeedUrl = feedUrl;
        }

        public string FeedUrl { get; }
    }
}