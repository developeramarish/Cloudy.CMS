﻿using Cloudy.CMS.ContentSupport;
using Cloudy.CMS.ContentTypeSupport;
using Cloudy.CMS.ContentTypeSupport.GroupSupport;
using Cloudy.CMS.SingletonSupport;
using Cloudy.CMS.UI.ContentAppSupport.ContentTypeActionSupport;
using Cloudy.CMS.UI.ContentAppSupport.ListActionSupport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cloudy.CMS.UI.ContentAppSupport.Controllers
{
    [Authorize]
    [Area("Cloudy.CMS")]
    public class ContentTypeGroupProviderController
    {
        IContentTypeGroupProvider ContentTypeGroupProvider { get; }
        IHumanizer Humanizer { get; }
        IPluralizer Pluralizer { get; }
        IContentTypeGroupMatcher ContentTypeGroupMatcher { get; }

        public ContentTypeGroupProviderController(IContentTypeGroupProvider contentTypeGroupProvider, IHumanizer humanizer, IPluralizer pluralizer, IContentTypeGroupMatcher contentTypeGroupMatcher)
        {
            ContentTypeGroupProvider = contentTypeGroupProvider;
            Humanizer = humanizer;
            Pluralizer = pluralizer;
            ContentTypeGroupMatcher = contentTypeGroupMatcher;
        }

        public IEnumerable<ContentTypeGroupResponseItem> GetAll()
        {
            var result = new List<ContentTypeGroupResponseItem>();

            foreach (var contentType in ContentTypeGroupProvider.GetAll())
            {
                result.Add(GetItem(contentType));
            }

            return result.AsReadOnly();
        }

        ContentTypeGroupResponseItem GetItem(ContentTypeGroupDescriptor contentTypeGroup)
        {
            var name = contentTypeGroup.Type.GetCustomAttribute<DisplayAttribute>()?.Name ?? contentTypeGroup.Type.Name;
            string pluralName;

            if (name.Contains(':') && !contentTypeGroup.Id.Contains(':'))
            {
                var nameSplit = name.Split(':');

                name = nameSplit.First();
                pluralName = nameSplit.Last();
            }
            else
            {
                if(name.Length >= 2 && name.StartsWith('I') && char.IsUpper(name[1]))
                {
                    name = name.Substring(1);
                }

                name = Humanizer.Humanize(name);
                pluralName = Pluralizer.Pluralize(name);
            }

            var item = new ContentTypeGroupResponseItem
            {
                Id = contentTypeGroup.Id,
                Name = name,
                LowerCaseName = name.Substring(0, 1).ToLower() + name.Substring(1),
                PluralName = pluralName,
                LowerCasePluralName = pluralName.Substring(0, 1).ToLower() + pluralName.Substring(1),
                ContentTypes = ContentTypeGroupMatcher.GetContentTypesFor(contentTypeGroup.Id).Select(t => t.Id).ToList().AsReadOnly(),
            };
            return item;
        }

        public class ContentTypeGroupResponseItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string LowerCaseName { get; set; }
            public string PluralName { get; set; }
            public string LowerCasePluralName { get; set; }
            public IEnumerable<string> ContentTypes { get; set; }
        }
    }
}
