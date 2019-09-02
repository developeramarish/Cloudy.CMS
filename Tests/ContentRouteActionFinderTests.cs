﻿using Cloudy.CMS.ContentSupport;
using Cloudy.CMS.ModelBinding;
using Cloudy.CMS.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ContentRouteActionFinderTests
    {
        [Fact]
        public void RequiresMatchingControllerName()
        {
            var action = new ControllerActionDescriptor { ControllerName = "lorem", MethodInfo = typeof(MyController).GetMethod(nameof(MyController.ActionWithTypeAParameter)) };

            var actions = Mock.Of<IActionDescriptorCollectionProvider>();
            Mock.Get(actions).SetupGet(a => a.ActionDescriptors).Returns(new ActionDescriptorCollection(new List<ActionDescriptor> { action }.AsReadOnly(), 0));

            var result = new ContentRouteActionFinder(actions).Find("ipsum", new MyContentA());

            Assert.Null(result);
        }

        [Fact]
        public void RequiresContentRouteAttribute()
        {
            var action = new ControllerActionDescriptor { ControllerName = "lorem", MethodInfo = typeof(MyController).GetMethod(nameof(MyController.ActionWithNonAnnotatedTypeAParameter)) };

            var actions = Mock.Of<IActionDescriptorCollectionProvider>();
            Mock.Get(actions).SetupGet(a => a.ActionDescriptors).Returns(new ActionDescriptorCollection(new List<ActionDescriptor> { action }.AsReadOnly(), 0));

            var result = new ContentRouteActionFinder(actions).Find("ipsum", new MyContentA());

            Assert.Null(result);
        }

        [Fact]
        public void FindsActionWithExactContentRouteType()
        {
            var actionA = new ControllerActionDescriptor { ControllerName = "lorem", MethodInfo = typeof(MyController).GetMethod(nameof(MyController.ActionWithTypeAParameter)) };
            var actionB = new ControllerActionDescriptor { ControllerName = "lorem", MethodInfo = typeof(MyController).GetMethod(nameof(MyController.ActionWithTypeBParameter)) };

            var actions = Mock.Of<IActionDescriptorCollectionProvider>();
            Mock.Get(actions).SetupGet(a => a.ActionDescriptors).Returns(new ActionDescriptorCollection(new List<ActionDescriptor> { actionA, actionB }.AsReadOnly(), 0));

            var result = new ContentRouteActionFinder(actions).Find("lorem", new MyContentA());

            Assert.Same(actionA, result);
        }

        [Fact]
        public void FindsActionWithAssignableContentRouteType()
        {
            var action = new ControllerActionDescriptor { ControllerName = "lorem", MethodInfo = typeof(MyController).GetMethod(nameof(MyController.ActionWithIContentParameter)) };

            var actions = Mock.Of<IActionDescriptorCollectionProvider>();
            Mock.Get(actions).SetupGet(a => a.ActionDescriptors).Returns(new ActionDescriptorCollection(new List<ActionDescriptor> { action }.AsReadOnly(), 0));

            var result = new ContentRouteActionFinder(actions).Find("lorem", new MyContentA());

            Assert.Same(action, result);
        }

        [Fact]
        public void FindsActionWithMostSpecificContentRouteType()
        {
            var actionA = new ControllerActionDescriptor { ControllerName = "lorem", MethodInfo = typeof(MyController).GetMethod(nameof(MyController.ActionWithIContentParameter)) };
            var actionB = new ControllerActionDescriptor { ControllerName = "lorem", MethodInfo = typeof(MyController).GetMethod(nameof(MyController.ActionWithTypeAParameter)) };

            var actions = Mock.Of<IActionDescriptorCollectionProvider>();
            Mock.Get(actions).SetupGet(a => a.ActionDescriptors).Returns(new ActionDescriptorCollection(new List<ActionDescriptor> { actionA, actionB }.AsReadOnly(), 0));

            var result = new ContentRouteActionFinder(actions).Find("lorem", new MyContentADescendant());

            Assert.Same(actionB, result);
        }

        class MyContentA : IContent {
            public string Id { get; set; }
            public string ContentTypeId { get; set; }
        }

        class MyContentB : IContent
        {
            public string Id { get; set; }
            public string ContentTypeId { get; set; }
        }

        class MyContentADescendant : MyContentA { }

        class MyController
        {
            public void ActionWithNonAnnotatedTypeAParameter(MyContentA parameter) { }
            public void ActionWithTypeAParameter([FromContentRoute] MyContentA parameter) { }
            public void ActionWithTypeADescendantParameter([FromContentRoute] MyContentADescendant parameter) { }
            public void ActionWithTypeBParameter([FromContentRoute] MyContentB parameter) { }
            public void ActionWithIContentParameter([FromContentRoute] IContent parameter) { }
        }
    }
}
