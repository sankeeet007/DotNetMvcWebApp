using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MVCwebApp.Data;
using MVCwebApp.Dto;
using MVCwebApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MVCwebApp.Controllers
{
    // new method for DI is using primary cntor ⬇️⬇️⬇️⬇️
    public class AuthController(AppDbContext context, IConfiguration configuration) : Controller      // MVC cntroler:- Controller is inherited 
                                                                        // API cntroler:- ControllerBase is inherited
    {
        //private readonly AppDbContext context;
        //public AuthController(AppDbContext context)       // this was used previouly in .net 7,8,9 for DependencyInjection
        //{
        //    this.context = context;
        //}

        public IActionResult Login()
        {

            ViewBag.SuccessMessage = TempData["SuccessMessage"];    // Controller -> View
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> CreateUser(UserDto dto)
        {
            if(dto == null || string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                ViewBag.ErrorMessage = "Kindly Fill All the details!!!";
                return View("Register");
            }
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if(existingUser == null)
            {
                var user = new User
                {
                    Username = dto.Username,
                    Password = dto.Password,
                    Email = dto.Email
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
            else
            {
                ViewBag.ErrorMessage = "Email already exists.";     //Controller -> View
                return RedirectToAction("Register");
            }
            TempData["SuccessMessage"] = "User created successfully. Please log in.";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> LoginUser(UserDto dto)
        {

            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                ViewBag.ErrorMessage = "Kindly Fill All the details!!!";
                return View("Login");
            }

            var doesUserExist = await context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if(doesUserExist == null)
            {
                ViewBag.ErrorMessage = "User with this Email Does not Exits.";
                return View("Login");
            } 
            else
            {
                if(doesUserExist.Password == dto.Password)
                {
                    var token = GenerateJwtToken(dto);
                    Response.Cookies.Append("jwt_key", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddMinutes(30)
                    });

                    return RedirectToAction("Index", "Dashboard");
                } 
                else
                {
                    ViewBag.ErrorMessage = "Incorrect Password.";
                    return View("Login");
                }
            }
        }

        private string GenerateJwtToken(UserDto dto)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKey = configuration["JwtSettings:Secret"]; // Fetch the secret key from configuration
            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                       new Claim(ClaimTypes.Name, dto.Email),
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }
        public IActionResult LogoutUser()
        {
            Response.Cookies.Delete("jwt_key");
            return RedirectToAction("Login");
        }
    }
}
