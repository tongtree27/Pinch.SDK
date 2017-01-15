﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Pinch.SDK.Auth;
using Pinch.SDK.WebSample.Helpers;
using Pinch.SDK.WebSample.Models;
using Pinch.SDK.WebSample.ViewModels;

namespace Pinch.SDK.WebSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly PinchSettings _settings;

        public HomeController(IOptions<PinchSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Connect()
        {
            var token = HttpContext.Session.GetObjectFromJson<GetAccessTokenFromCodeResponse>("AccessToken");
            var model = new IndexVm()
            {
                AccessToken = token
            };

            var api = new PinchApi(_settings.SecretKey, _settings.MerchantId, true, _settings.BaseUri, _settings.AuthUri);
            if (!string.IsNullOrEmpty(model.AccessToken?.AccessToken))
            {
                var result = await api.Auth.GetClaims(model.AccessToken.AccessToken);
                //var result = User.Claims.Select(x => new GetClaimsResponseItem()
                //{
                //    Type = x.Type,
                //    Value = x.Value
                //}).ToList();
                model.Claims = result;
            }
            else
            {
                model.ConnectUrl = api.Auth.GetConnectUrl(_settings.ApplicationId, Url.Action("Callback", "Pinch", null, Request.Scheme));
            }

            return View(model);
        }
    }
}
