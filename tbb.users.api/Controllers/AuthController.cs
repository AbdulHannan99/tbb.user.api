﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using tbb.users.api.Providers;
using tbb.users.api.Models;

namespace tbb.users.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserProvider _userProvider;

        public AuthController(IUserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || !IsValidEmail(request.Email))
            {
                return BadRequest(new { Message = "A valid email is required." });
            }

            if (string.IsNullOrEmpty(request.Password) || request.Password.Length < 12)
            {
                return BadRequest(new { Message = "Password must be at least 12 characters long." });
            }

            bool isValidUser = await _userProvider.ValidateUserAsync(request.Email, request.Password);
            if (!isValidUser)
            {
                return Unauthorized(new { Message = "Incorrect email or password." });
            }

            // Additional code for generating and returning JWT token or session management
            return Ok(new { Message = "Login successful", RedirectUrl = "/dashboard" });
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}