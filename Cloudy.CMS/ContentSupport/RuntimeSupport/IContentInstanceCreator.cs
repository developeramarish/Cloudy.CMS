﻿using Cloudy.CMS.ContentTypeSupport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudy.CMS.ContentSupport.RuntimeSupport
{
    public interface IContentInstanceCreator
    {
        IContent Create(ContentTypeDescriptor contentType);
    }
}
