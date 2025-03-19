using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenKeyServer.Data;
using OpenKeyServer.Data.Request;
using OpenKeyServer.Data.Response;

namespace OpenKeyServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class KeyController(ApplicationDbContext context,IKeyValidation keyValidation,IConfiguration configuration) : ControllerBase
    {
        [HttpPost("set/{userName}")]
        public async Task<IActionResult> SetKeyOld([FromBody] string value,string userName)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userName);
            if (user == null)
            {
                // user = new ApplicationUser { Id = userName };
                // context.Users.Add(user);
                return Ok(new CommonResponse { code = (int)Code.InvalidRequest });
            }
            if (!user.AllowOld)
            {
                return Ok(new CommonResponse { code = (int)Code.InvalidRequest });
            }
            user.Key = value;
            await context.SaveChangesAsync();
            return Ok(new CommonResponse { code = (int)Code.Success });
        }
        [Authorize]
        [HttpPost("v2/update")]
        public async Task<IActionResult> SetKeyV2([FromBody] SetKeyV2 request)
        {
            var keyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!keyValidation.ValidateKey($"{request.keyId}:{request.rsaPublicKey}", request.bindNumber,
                    request.chipId))
            {
                return Ok(new CommonResponse { code = (int)Code.InvalidRequest });
            }
            
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == keyId);
            if (user == null)
            {
                user = new ApplicationUser { Id = keyId };
                context.Users.Add(user);
            }
            user.Key = $"{request.keyId}:{request.rsaPublicKey}";
            await context.SaveChangesAsync();
            return Ok(new CommonResponse { code = (int)Code.Success });
        }
        [Authorize]
        [HttpPost("v3/update")]
        public async Task<IActionResult> SetKeyV3([FromBody] SetKeyV3 request)
        {
            var keyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!keyValidation.ValidateKey($"{request.keyId}:{request.aesKey}:{request.aesKey}", request.bindNumber,
                    request.chipId))
            {
                return Ok(new CommonResponse { code = (int)Code.InvalidRequest });
            }
            
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == keyId);
            if (user == null)
            {
                user = new ApplicationUser { Id = keyId };
                context.Users.Add(user);
            }
            user.Key = $"{request.keyId}:{request.aesKey}:{request.aesKey}";
            await context.SaveChangesAsync();
            return Ok(new CommonResponse { code = (int)Code.Success });
        }

        [HttpPost("get_key")]
        public async Task<IActionResult> GetKey([FromBody] GetKeyRequest request)
        {
            if (request.apiSecret != configuration["Secret"])
            {
                return Ok(new CommonResponse()
                {
                    code = (int)Code.AuthFailed
                });
            }
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.keyId);
            if (user == null)
            {
                return Ok(new CommonResponse()
                {
                    code = (int)Code.Success,
                    data = new GetKeyResponse()
                    {
                        Key = user.Key,
                        Type = user.Type
                    }
                });
            }
            else
            {
                return Ok(new CommonResponse()
                {
                    code = (int)Code.InvalidRequest,
                    data = null
                });
            }
        }
    }
}
