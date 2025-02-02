﻿using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiCatalogo.Controllers
{
    [Route("api/teste")]
    [ApiController]
    [ApiVersion(3)]
    [ApiVersion(4)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class Teste3Controller : ControllerBase
    {
        [MapToApiVersion(3)]
        [HttpGet]
        public string GetVersion3()
        {
            return "Version3 - GET - Api Versão 3.0";
        }

        [MapToApiVersion(4)]
        [HttpGet]
        public string GetVersion4()
        {
            return "Version4 - GET - Api Versão 4.0";
        }
    }
}
