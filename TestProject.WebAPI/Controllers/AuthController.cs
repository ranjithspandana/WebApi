using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using TestProject.WebAPI.Data;
using TestProject.WebAPI.Models;

namespace TestProject.WebAPI.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TestProjectContext _testProjectContext;

        public AuthController(TestProjectContext testProjectContext)
        {
            _testProjectContext = testProjectContext;
        }
    }
}
