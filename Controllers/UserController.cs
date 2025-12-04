using DotNetOpenAuth.OAuth.ChannelElements;
using Infusive_back.EntityData;
using Infusive_back.JwtAuth;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Hrlense.Controllers.hrlense.PageController;

namespace Infusive_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        readonly private MyDbContext db;
        private readonly JwtAuth.ITokenManager tokenManager;
        private readonly PermissionCheck _permissionCheck;

        public UserController(MyDbContext db, JwtAuth.ITokenManager tokenManager)
        {
            this.db = db;
            this.tokenManager = tokenManager;
            _permissionCheck = new PermissionCheck(db);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUsers([FromQuery] string? text)
        {
            try
            {
                var query = db.User_Details
                    .Select(user => new
                    {
                        user.Id,
                        user.UserId,
                        user.Name,
                        user.Email,
                        user.Mobile,
                        Roles = user.UserRoles
                            .Select(ur => new
                            {
                                ur.Role.Id,
                                ur.Role.Name
                            })
                            .ToList()
                    })
                    .ToList();

                return Ok(query);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost]
        [Authorize]
        public IActionResult AddUsers([FromBody] UserDto users)
        {
            try
            {
                var existingUser = db.User_Details.FirstOrDefault(u => u.Email == users.Email);
                if (existingUser != null)
                    return BadRequest("Email already exists !");

                var newUser = new User_Details
                {
                    UserId = users.UserId,
                    Name = users.Name,
                    Email = users.Email,
                    Password = users.Password,
                    Mobile = users.Mobile,
                };
                db.User_Details.Add(newUser);
                db.SaveChanges();

                foreach (var roleId in users.Roles)
                {
                    db.UserRole.Add(new UserRole
                    {
                        UserId = newUser.Id,
                        RoleId = roleId
                    });
                }

                db.SaveChanges();

                return Ok(new { message = "Posted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut]
        [Authorize]
        public IActionResult UpdateUser([FromBody] UpdateUserDto userDto)
        {
            try
            {
                var user = db.User_Details.Include(u => u.UserRoles).FirstOrDefault(u => u.Id == userDto.Id);

                if (user == null)
                    return NotFound("User not found");

                user.Name = userDto.Name;
                user.Email = userDto.Email;
                user.Mobile = userDto.Mobile;

                user.UserRoles.Clear();
                foreach (var roleId in userDto.Roles)
                {
                    user.UserRoles.Add(new UserRole
                    {
                        UserId = userDto.Id,
                        RoleId = roleId
                    });
                }

                db.SaveChanges();
                return Ok(new { message = "Updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost]
        [Route("login")]
        public IActionResult Login(LoginPayload user)
        {
            try
            {
                var User = db.User_Details.SingleOrDefault(u => u.Email.ToLower().Trim() == user.Email.ToLower().Trim() && user.Password == u.Password);
                if (User == null)
                {
                    return BadRequest("Email or password is incorrect");
                }

                if (User.IsActive == false)
                {
                    return BadRequest("Your Account is Inactive, Please Contact to Admin.");
                }

                var token = tokenManager.NewToken(User.Id.ToString());

                int[] roleIds = db.UserRole.Where(f => f.UserId == User.Id).OrderBy(o => o.RoleId).Select(c => c.RoleId).ToArray();

                var rolePermissions = (from pm in db.Page
                                       join spm in db.Page
                                       on pm.ParentId equals spm.Id into tspm
                                       from spm in tspm.DefaultIfEmpty()
                                       join pp in db.Permission
                                       .Where(pp => roleIds.Contains(pp.RoleId))
                                       on pm.Id equals pp.PageId into tpp
                                       from pp in tpp.DefaultIfEmpty()
                                       select new
                                       {
                                           pm.Id,
                                           pm.Name,
                                           pm.Label,
                                           pm.Url,
                                           pm.Description,
                                           pm.IsFeature,
                                           pm.ParentId,
                                           Parent = spm == null ? pm.Name : spm.Name,
                                           Permission = pp != null,
                                           RoleId = pp != null ? pp.RoleId : (int?)null
                                       })
                                       .GroupBy(rp => rp.Id)
                                      .Select(g => g.First())
                                      .ToList()
                                      .Select(rp => new RolePermission
                                      {
                                          Id = rp.Id,
                                          Name = rp.Name,
                                          Label = rp.Label,
                                          Url = rp.Url,
                                          Description = rp.Description,
                                          IsFeature = rp.IsFeature,
                                          ParentId = rp.ParentId,
                                          Parent = rp.Parent,
                                          Permission = rp.Permission
                                      }).ToList();

                return Ok(new
                {
                    isAdmin = User.Email == "admin" ? true : false,
                    message = "Login successful",
                    token = token,
                    userId = User.Id,
                    userName = User.Name,
                    roleId = roleIds,
                    permissions = rolePermissions,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        public class UserDto
        {
            public required string UserId { get; set; }
            public required string Name { get; set; }
            public required string Email { get; set; }
            public required string Password { get; set; }
            public required string Mobile { get; set; }
            public required List<int> Roles { get; set; }
        }

        public class UpdateUserDto
        {
            public required int Id { get; set; }
            public required string Name { get; set; }
            public required string Email { get; set; }
            public required string Mobile { get; set; }
            public required List<int> Roles { get; set; }
        }


        public class LoginPayload
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }
    }
}