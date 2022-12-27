using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Chat.Controllers
{

    public class AccountController : Controller
    {
        private ApplicationContext _context;
        public AccountController(ApplicationContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Token([FromBody] UserModel myUser)
        {
            var identity = await GetIdentity(myUser.UserName, myUser.Password);
            if (identity == null)
            {
                return BadRequest("Invalid username or password.");
            }
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Json(encodedJwt);
        }
        [HttpPost]
        public async Task<IActionResult> GetMessages([FromBody] string group)
        {
            try
            {
                ChanelModel chanel = _context.Chanels.Include(o => o.Users).FirstOrDefault(g => g.GroupName == group);
                List<MessageData> messages = new();
                foreach (MessageData message in _context.Messages)
                    if (message.ChanelID == chanel.Id)
                        messages.Add(message);
                return Json(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            UserModel person = await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(x => x.UserName == username && x.Password == password);
            if (person != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, person.UserName),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role.Name)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            return null;
        }
    }
}

