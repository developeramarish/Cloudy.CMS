﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudy.CMS.UI.DataTableSupport.BackendSupport
{
    [Authorize]
    [Area("Cloudy.CMS")]
    public class BackendApiController
    {
        IBackendProvider BackendProvider { get; }

        public BackendApiController(IBackendProvider backendProvider)
        {
            BackendProvider = backendProvider;
        }

        public Result GetAll(string provider, int page, string sortBy, string sortDirection)
        {
            var direction = sortDirection == "ascending" ? (SortDirection?)SortDirection.Ascending : sortDirection == "descending" ? (SortDirection?)SortDirection.Descending : null;

            var backend = BackendProvider.GetFor(provider);

            if(backend == null)
            {
                throw new BackendNotFoundException(provider);
            }

            return backend.Load(new Query(page, sortBy, direction));
        }
    }
}
