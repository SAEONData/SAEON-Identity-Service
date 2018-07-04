﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SAEON.Identity.Service.Config
{
    public class ConfigController : Controller
    {
        ConfigControllerLogic _logic;

        public ConfigController()
        {
            _logic = new ConfigControllerLogic();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ClientResources()
        {
            return View(_logic.GetClientResources());
        }

        public ActionResult ClientResourceAdd()
        {
            return View("ClientResourceEdit", _logic.GetClientResource(0));
        }

        public ActionResult ClientResourceEdit(int Id)
        {
            return View(_logic.GetClientResource(Id));
        }

        public ActionResult ClientResourceDelete(int Id)
        {
            bool result = _logic.DeleteClient(Id);

            return RedirectToAction("ClientResources");
        }

        [HttpPost]
        public ActionResult ClientResourceAdd(Client client)
        {
            if (ModelState.IsValid)
            {
                //Convert back to IS Client
                var isClient = _logic.BuildClient(client);

                //Save changes to DB
                var result = _logic.SaveClient(isClient);

                return RedirectToAction("ClientResources");
            }

            return View(client);
        }

        [HttpPost]
        public ActionResult ClientResourceEdit(Client client)
        {
            if (ModelState.IsValid)
            {
                //Convert back to IS Client
                var isClient = _logic.BuildClient(client);

                //Save changes to DB
                var result = _logic.SaveClient(isClient);

                return RedirectToAction("ClientResources");
            }

            return View(client);
        }

        public ActionResult ApiResources()
        {
            return View(_logic.GetApiResources());
        }

        public ActionResult ApiResourceEdit(int Id)
        {
            return View(_logic.GetApiResource(Id));
        }

        public ActionResult ApiResourceAdd()
        {
            return View("ApiResourceEdit", _logic.GetApiResource(0));
        }

        [HttpPost]
        public ActionResult ApiResourceEdit(API api)
        {
            if (ModelState.IsValid)
            {
                //Convert back to IS Client
                var isApi = _logic.BuildApi(api);

                //Save changes to DB
                var result = _logic.SaveApi(isApi);

                return RedirectToAction("ApiResources");
            }

            return View(api);
        }

        [HttpPost]
        public ActionResult ApiResourceAdd(API api)
        {
            if (ModelState.IsValid)
            {
                //Convert back to IS Client
                var isApi = _logic.BuildApi(api);

                //Save changes to DB
                var result = _logic.SaveApi(isApi);

                return RedirectToAction("ApiResources");
            }

            return View(api);
        }

        public ActionResult ApiResourceDelete(int Id)
        {
            bool result = _logic.DeleteApi(Id);

            return RedirectToAction("ApiResources");
        }
    }

}