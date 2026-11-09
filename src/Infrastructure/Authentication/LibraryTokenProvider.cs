using System.Threading;
using System;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Authentication;

internal sealed class LibraryTokenProvider(
    IConfiguration configuration,
    IUnitOfWork unitOfWork) : ILibraryTokenProvider
{
    public async Task<string> CreateAsync(User user, CancellationToken ct)
    {
        var userRoles = await unitOfWork.GetDbSet<UserRole>()
            .Where(ur => ur.UserId == user.Id)
            .Select(ur => ur.RoleId)
            .ToListAsync(ct);

        var roles = await unitOfWork.GetDbSet<Role>()
            .Where(r => userRoles.Contains(r.Id))
            .Select(r => r.Name)
            .ToListAsync(ct);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("userName", user.UserName),
            new Claim("name", user.Name),
            new Claim("lastName", user.LastName)
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(configuration["Jwt:ExpirationInMinutes"]!, CultureInfo.InvariantCulture)),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}


