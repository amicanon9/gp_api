using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Gp_Api.Helpers
{
    public class JwtHelpers
    {
        private readonly IConfiguration Configuration;

        public JwtHelpers(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public string RefreshToken(string username, string token, int expireMinutes = 20)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var payload = tokenHandler.ReadJwtToken(token).Payload;
            if (payload.ContainsKey("role_id") && payload.ContainsKey("user_id") && payload.ContainsKey("book_id") && payload.ContainsKey("company_name"))
            {
                var issuer = Configuration.GetValue<string>("JwtSettings:Issuer");
                var signKey = Configuration.GetValue<string>("JwtSettings:SignKey");

                // 設定要加入到 JWT Token 中的聲明資訊(Claims)
                var claims = new List<Claim>();
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, username)); // User.Identity.Name                                                       
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT ID

                claims.Add(new Claim("role_id", payload["role_id"].ToString()));
                claims.Add(new Claim("user_id", payload["user_id"].ToString()));
                claims.Add(new Claim("book_id", payload["book_id"].ToString()));
                claims.Add(new Claim("permission_level", payload["permission_level"].ToString()));
                claims.Add(new Claim("company_name", payload["company_name"].ToString()));

                var userClaimsIdentity = new ClaimsIdentity(claims);

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));
                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                // 建立 SecurityTokenDescriptor
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = issuer,
                    Subject = userClaimsIdentity,
                    Expires = DateTime.Now.AddMinutes(expireMinutes),
                    SigningCredentials = signingCredentials
                };
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var serializeToken = tokenHandler.WriteToken(securityToken);

                return serializeToken;
            }
            return "error";
        }

        public string GenerateToken(int role_id, int user_id, string userName, int company_id,byte permission_level ,string company_name, int expireMinutes = 20)
        {
            var issuer = Configuration.GetValue<string>("JwtSettings:Issuer");
            var signKey = Configuration.GetValue<string>("JwtSettings:SignKey");

            // 設定要加入到 JWT Token 中的聲明資訊(Claims)
            var claims = new List<Claim>();

            // 在 RFC 7519 規格中(Section#4)，總共定義了 7 個預設的 Claims，我們應該只用的到兩種！
            //claims.Add(new Claim(JwtRegisteredClaimNames.Iss, issuer));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, userName)); // User.Identity.Name
            //claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "The Audience"));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds().ToString()));
            //claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
            //claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())); // 必須為數字
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT ID

            // 網路上常看到的這個 NameId 設定是多餘的
            //claims.Add(new Claim(JwtRegisteredClaimNames.NameId, userName));

            // 這個 Claim 也以直接被 JwtRegisteredClaimNames.Sub 取代，所以也是多餘的
            //claims.Add(new Claim(ClaimTypes.Name, userName));

            // 你可以自行擴充 "roles" 加入登入者該有的角色
            claims.Add(new Claim("role_id", role_id.ToString()));
            claims.Add(new Claim("user_id", user_id.ToString()));
            claims.Add(new Claim("book_id", company_id.ToString()));
            claims.Add(new Claim("permission_level", permission_level.ToString()));
            claims.Add(new Claim("company_name", company_name));

            var userClaimsIdentity = new ClaimsIdentity(claims);

            // 建立一組對稱式加密的金鑰，主要用於 JWT 簽章之用
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey));

            // HmacSha256 有要求必須要大於 128 bits，所以 key 不能太短，至少要 16 字元以上
            // https://stackoverflow.com/questions/47279947/idx10603-the-algorithm-hs256-requires-the-securitykey-keysize-to-be-greater
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // 建立 SecurityTokenDescriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                //Audience = issuer, // 由於你的 API 受眾通常沒有區分特別對象，因此通常不太需要設定，也不太需要驗證
                //NotBefore = DateTime.Now, // 預設值就是 DateTime.Now
                //IssuedAt = DateTime.Now, // 預設值就是 DateTime.Now
                Subject = userClaimsIdentity,
                Expires = DateTime.Now.AddMinutes(expireMinutes),
                SigningCredentials = signingCredentials
            };

            // 產出所需要的 JWT securityToken 物件，並取得序列化後的 Token 結果(字串格式)
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }
    }
}
