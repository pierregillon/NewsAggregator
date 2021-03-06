﻿using System.Linq;
using FluentAssertions;
using Sociomedia.Articles.Domain;
using Sociomedia.Articles.Domain.Keywords;
using Xunit;

namespace Sociomedia.Articles.Tests.UnitTests
{
    public class KeywordsTests
    {
        [Fact]
        public void A_keyword_contains_another()
        {
            new Keyword("john wick", 1)
                .Contains(new Keyword("john", 2))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void A_keyword_does_not_contains_different_one()
        {
            new Keyword("john wick likes john connor", 1)
                .Contains(new Keyword("test", 2))
                .Should()
                .BeFalse();
        }

        [Fact]
        public void A_keyword_does_not_contains_same_in_different_order()
        {
            new Keyword("john wick", 1)
                .Contains(new Keyword("wick john", 2))
                .Should()
                .BeFalse();
        }

        [Fact]
        public void Intersection_of_keywords()
        {
            new[] { new Keyword("john wick", 1) }
                .Intersect(new[] { new Keyword("john wick", 1) })
                .Should()
                .BeEquivalentTo(new Keyword("john wick", 1));
        }
    }
}