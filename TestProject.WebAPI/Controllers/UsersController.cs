using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestProject.WebAPI.Data;
using TestProject.WebAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using AutoMapper;
using TestProject.WebAPI.Helpers;
using TestProject.WebAPI.Models;
using System;
using System.Net.Http;

namespace TestProject.WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private IUsersService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(
            IUsersService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("/token")]
        public  IActionResult Authenticate([FromBody]LoginModel model)
        {
            var user = _userService.Authenticate(model.Email, model.Password);

            if (user == null)
                return BadRequest(new { message = "Email or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = AuthOptions.GetSymmetricSecurityKey();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info and authentication token
            return Ok(new LoginResponseModel
            {
                Username = user.Email,
                AccessToken = tokenString
            });
        }

        [AllowAnonymous]
        [HttpGet("/curentuser")]
        public  IActionResult IsAuthorized()
        {

            return BadRequest(new { message = "CurrentUser" });
        }

        [AllowAnonymous]
        [HttpPost()]
        public async Task<IActionResult> Register([FromBody]RegisterModel model)
        {
            // map model to entity
            var user = _mapper.Map<User>(model);

            try
            {
                // create user
                await _userService.Add(user, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetData([FromQuery]string[] firstNames=null)
        {
           
            var users = await _userService.GetAll();
            if (firstNames==null || firstNames.Length==0)
            {
                var model = _mapper.Map<IList<UserModel>>(users);
                return Ok(model);
            }
            else
            {
                IEnumerable<User> filtered = users.Where(item =>
                firstNames.Any(name => name.Equals(item.FirstName)));
                var model = _mapper.Map<IList<UserModel>>(filtered);
                return Ok(model);
            }
        }
        /*
        [HttpGet]
        public async Task<IActionResult> GetData(List<string> firstNames=null)
        {
            var users = await _userService.GetAll();
            var model = _mapper.Map<IList<UserModel>>(users);
            return Ok(model);
        }*/

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserById(id);
            if (user == null) return NotFound();
            var model = _mapper.Map<UserModel>(user);
            return Ok(model);
        }

        /*
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByFirstNames(string[] firstNames)
        {
            var user = await _userService.GetByFirstNames(firstNames);
            var model = _mapper.Map<UserModel>(user);
            return Ok(model);
        }*/

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]UpdateModel model)
        {
            // map model to entity and set id
            var user = _mapper.Map<User>(model);
            user.Id = id;

            try
            {
                // update user 
                user = await _userService.Update(user, model.Password);
                return Ok(user);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("/import")]
        public async Task<IActionResult> ImportData([FromBody]byte[] bytedata)
        {
            //var d = content.ToString();
            /* Change the code to update the list of users */
            var users = await _userService.GetAll();
            try
            {
                // update user 
                
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return NotFound();
            }
            return Ok();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.Delete(id);
            if (result)
                return Ok();
            else
                return NotFound();
        }
    }
}
