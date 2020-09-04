using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using VMS_Auth_Service.DTOs;
using VMS_Auth_Service.Models;
using VMS_Auth_Service.Repository;

namespace VMS_Auth_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository authRepo, IConfiguration config) {
            _authRepo = authRepo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto) { 
            if(_authRepo.UserExists(userForRegisterDto.Email, userForRegisterDto.Type))
                return BadRequest("Username already exists");

            var userToCreate = new User {
                Uid = userForRegisterDto.Uid,
                Name = userForRegisterDto.Name,
                Type = userForRegisterDto.Type,
                Email = userForRegisterDto.Email
            };

            var createdUser = await _authRepo.Register(userToCreate, userForRegisterDto.Password);
             
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            User userFromRepo = await _authRepo.Login(userForLoginDto.Email.ToLower(), userForLoginDto.Password, userForLoginDto.Type.ToLower());

            if (userFromRepo == null)
                return Unauthorized();

            //Token Generation
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Uid.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Email)
            };

            //A security key is created using the secret from the appsettings.Development.json file
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //The created key is encrypted using a security algorithm
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //The token details are specified using the SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = creds
            };

            //The token handler instance is created
            var tokenHandler = new JwtSecurityTokenHandler();

            //The token is created using the token handler
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                id = userFromRepo.Uid,
                name = userFromRepo.Name,
                type = userFromRepo.Type,
                email = userFromRepo.Email,
                //The token is writen into the request using the token handler
                token = tokenHandler.WriteToken(token),
                tokenExpirationDate = tokenDescriptor.Expires
            });
        }
    }
}
