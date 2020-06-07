﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Sociomedia.Articles.Domain.Articles;
using Sociomedia.Articles.Domain.Keywords;
using Sociomedia.Core.Domain;
using Sociomedia.Core.Infrastructure.CQRS;
using Sociomedia.Themes.Domain;
using Xunit;
using Keyword = Sociomedia.Themes.Domain.Keyword;

namespace Sociomedia.Tests.AcceptanceTests
{
    public class ChallengeThemesOnArticleEventReceived : AcceptanceTests
    {
        [Fact]
        public async Task Create_new_theme_on_article_keywords_defined_with_common_keywords()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();

            await EventPublisher.Publish(new [] {
                new ArticleKeywordsDefined(article1Id, new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 2), new Articles.Domain.Keywords.Keyword("france", 2) }),
                new ArticleKeywordsDefined(article2Id, new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 3), new Articles.Domain.Keywords.Keyword("chine", 2) }),
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 5) }, new[] { article1Id, article2Id })
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Add_article_to_existing_theme()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();
            var article3Id = Guid.NewGuid();

            await EventPublisher.Publish(new[] {
                new ArticleKeywordsDefined(article1Id, new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 2), new Articles.Domain.Keywords.Keyword("france", 2) }),
                new ArticleKeywordsDefined(article2Id, new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 3), new Articles.Domain.Keywords.Keyword("chine", 2) }),
                new ArticleKeywordsDefined(article3Id, new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 3), new Articles.Domain.Keywords.Keyword("italie", 3) })
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 5) }, new[] { article1Id, article2Id }),
                    new ArticleAddedToTheme(default, article3Id),
                    new ThemeKeywordsUpdated(default, new[] { new Keyword("coronavirus", 8) })
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Create_2_themes_on_article_keywords_defined()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();
            var article3Id = Guid.NewGuid();

            await EventPublisher.Publish(new[] {
                new ArticleKeywordsDefined(article1Id, new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 2), new Articles.Domain.Keywords.Keyword("france", 2) }),
                new ArticleKeywordsDefined(article2Id, new[] { new Articles.Domain.Keywords.Keyword("opera", 3), new Articles.Domain.Keywords.Keyword("chine", 2) }),
                new ArticleKeywordsDefined(article3Id, new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 5), new Articles.Domain.Keywords.Keyword("chine", 3) }),
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 7) }, new[] { article1Id, article3Id }),
                    new ThemeAdded(default, new[] { new Keyword("chine", 5) }, new[] { article2Id, article3Id }),
                }, x => x.ExcludeDomainEventTechnicalFields());
        }

        [Fact]
        public async Task Create_a_new_theme_on_keyword_theme_intersection()
        {
            var article1Id = Guid.NewGuid();
            var article2Id = Guid.NewGuid();
            var article3Id = Guid.NewGuid();

            await EventPublisher.Publish(new[] {
                new ArticleKeywordsDefined(article1Id, new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 2), new Articles.Domain.Keywords.Keyword("france", 2) }),
                new ArticleKeywordsDefined(article2Id, new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 3), new Articles.Domain.Keywords.Keyword("france", 3) }),
                new ArticleKeywordsDefined(article3Id, new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 5), new Articles.Domain.Keywords.Keyword("chine", 3) }),
            });

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 5), new Keyword("france", 5) }, new[] { article1Id, article2Id }),
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 10) }, new[] { article1Id, article2Id, article3Id })
                }, x => x.ExcludeDomainEventTechnicalFields());
        }


        [Fact]
        public async Task Create_multiple_themes()
        {
            var events = new[] {
                new ArticleKeywordsDefined(Guid.NewGuid(), new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 2), new Articles.Domain.Keywords.Keyword("france", 2) }),
                new ArticleKeywordsDefined(Guid.NewGuid(), new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 8), new Articles.Domain.Keywords.Keyword("chine", 3) }),
                new ArticleKeywordsDefined(Guid.NewGuid(), new[] { new Articles.Domain.Keywords.Keyword("italie", 5), new Articles.Domain.Keywords.Keyword("france", 5) }),
                new ArticleKeywordsDefined(Guid.NewGuid(), new[] { new Articles.Domain.Keywords.Keyword("chine", 5), new Articles.Domain.Keywords.Keyword("opera", 3) }),
                new ArticleKeywordsDefined(Guid.NewGuid(), new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 2), new Articles.Domain.Keywords.Keyword("italie", 6) }),
                new ArticleKeywordsDefined(Guid.NewGuid(), new[] { new Articles.Domain.Keywords.Keyword("coronavirus", 5) }),
            };

            await EventPublisher.Publish(events);

            (await EventStore.GetNewEvents())
                .Should()
                .BeEquivalentTo(new DomainEvent[] {
                    new ThemeAdded(default, new[] { new Keyword("coronavirus", 10) }, new[] { events.ElementAt(0).Id, events.ElementAt(1).Id }),
                    new ThemeAdded(default, new[] { new Keyword("france", 7) }, new[] { events.ElementAt(0).Id, events.ElementAt(2).Id }),
                    new ThemeAdded(default, new[] { new Keyword("chine", 8) }, new[] { events.ElementAt(1).Id, events.ElementAt(3).Id }),
                    new ArticleAddedToTheme(default, events.ElementAt(4).Id),
                    new ThemeKeywordsUpdated(default, new[] { new Keyword("coronavirus", 12) }),
                    new ThemeAdded(default, new[] { new Keyword("italie", 8) }, new[] { events.ElementAt(2).Id, events.ElementAt(4).Id }),
                    new ArticleAddedToTheme(default, events.ElementAt(5).Id),
                    new ThemeKeywordsUpdated(default, new[] { new Keyword("coronavirus", 17) }),
                }, x => x.ExcludeDomainEventTechnicalFields());
        }
    }
}