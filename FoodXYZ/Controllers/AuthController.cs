using FoodXYZ.Contracts.Authentication;
using Microsoft.AspNetCore.Mvc;
using FoodXYZ.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace FoodXYZ.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly DataContext dataContext;
    private readonly IConfiguration configuration;

    public AuthController(DataContext dataContext, IConfiguration configuration)
    {
        this.dataContext = dataContext;
        this.configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Username == request.Username);

        if(user == null)
        {
            return NotFound(new{
                message= "User tidak ditemukan!"
            });
        }

        if(user.Password != HashPassword(request.Password))
        {
            return NotFound(new{
                message= "Password salah!"
            });
        }

        var token = GenerateToken(user);
        var response = new 
        {
            data = new AuthenticationRequest(
                user.Name,
                user.Username,
                user.Address,
                user.Image,
                token
            )
        };
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = await dataContext.Users.FirstOrDefaultAsync(x => x.Username == request.Username);

        if(user != null){
            return BadRequest(
                "User sudah ada"
            );
        }

        if(request.Password != request.Password_confirmation){
            return BadRequest(
                "Password tidak sama"
            );
        }

        var hashedPassword = HashPassword(request.Password);

        var data = new User
        {
            Name = request.Name,
            Username = request.Username,
            Address = request.Address,
            Password = hashedPassword 
        };

        dataContext.Users.Add(data);
        await dataContext.SaveChangesAsync();

        var token = GenerateToken(data);

        return Ok(new{
            data = new AuthenticationRequest(
                data.Name,
                data.Username,
                data.Address,
                data.Image,
                token
            )
        });
    }

    private string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JwtSettings:Secret-Key").Value));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            expires: DateTime.Now.AddDays(1),
            claims: claims,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        using(SHA256 sha256 = SHA256.Create())
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = sha256.ComputeHash(passwordBytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}