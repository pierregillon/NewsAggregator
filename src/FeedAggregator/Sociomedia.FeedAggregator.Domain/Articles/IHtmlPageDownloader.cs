﻿using System;
using System.Threading.Tasks;

namespace Sociomedia.Domain.Articles
{
    public interface IHtmlPageDownloader
    {
        Task<string> Download(Uri url);
    }
}