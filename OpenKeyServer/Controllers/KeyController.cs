using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
            user.LastUpdated = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            await context.SaveChangesAsync();
            return Ok(new CommonResponse { code = (int)Code.Success });
        }
        [Authorize]
        [HttpPost("update/v2")]
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
            user.LastUpdated = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            await context.SaveChangesAsync();
            return Ok(new CommonResponse { code = (int)Code.Success });
        }
        [Authorize]
        [HttpPost("update/v3")]
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
            user.LastUpdated = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
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
                        Type = user.Type,
                        LastUpdated = user.LastUpdated
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

        [HttpPost("sync_key")]
        public async Task<IActionResult> SyncKey([FromBody] SyncKeyRequest request)
        {
            if (request.apiSecret != configuration["Secret"])
            {
                return Ok(new CommonResponse()
                {
                    code = (int)Code.AuthFailed
                });
            }

            long fromTimestamp = request.fromTimestamp ?? -1;

            var users = await context.Users
                .Where(u => u.LastUpdated > fromTimestamp)
                .ToListAsync();

            var items = users.Select(u => new KeyItemResponse
            {
                Id = u.Id,
                Value = new GetKeyResponse
                {
                    Key = u.Key,
                    Type = u.Type, 
                    LastUpdated = u.LastUpdated
                }
            }).ToList();

            return Ok(new CommonResponse()
            {
                code = (int)Code.Success,
                data = new SyncKeyResponse
                {
                    Items = items
                }
            });
        }
    }
}
