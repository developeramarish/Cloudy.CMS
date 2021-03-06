﻿using Cloudy.CMS.ComposableSupport;
using Cloudy.CMS.ContentTypeSupport;

namespace Cloudy.CMS.ContentSupport.RuntimeSupport
{
    public interface IContentInstanceInitializer : IComposable
    {
        void Initialize(IContent content, ContentTypeDescriptor contentType);
    }
}