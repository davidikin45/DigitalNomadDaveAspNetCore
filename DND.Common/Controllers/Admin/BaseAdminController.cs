﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Middleware;
using System;

namespace DND.Common.Controllers.Admin
{
    public abstract class BaseAdminControllerAuthorize : BaseControllerAuthorize
    {

        public BaseAdminControllerAuthorize(IMapper mapper)
             : base(mapper)
        {
        }

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("file-manager")]
        public ActionResult FileManager()
        {
            return View();
        }

        //https://stackoverflow.com/questions/565239/any-way-to-clear-flush-remove-outputcache/16038654
        [Route("clear-cache")]
        public ActionResult ClearCache()
        {
            ResponseCachingCustomMiddleware.ClearResponseCache();
            return View();
        }
    }
}