﻿using Cloudy.CMS.ContentTypeSupport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudy.CMS.Routing
{
    public interface IContentRouteMatcher
    {
        IEnumerable<ContentRouteDescriptor> GetFor(ContentTypeDescriptor contentType);
    }
}
