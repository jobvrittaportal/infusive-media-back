using Infusive_back.EntityData;
using Infusive_back.JwtAuth;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;

namespace Infusive_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController(MyDbContext db, CheckPermission p) : ControllerBase
    {
        readonly private MyDbContext db = db;
        readonly private CheckPermission p = p;


        [HttpGet]
        [Authorize]
        public IActionResult GetRoles([FromQuery] string? text, [FromQuery] string? lazyParams)
        {
            if (!p.HasPermission(User, "Roles", "Read")) return BadRequest(new { Message = "Permission Denied!" });

            try
            {
                int first = 0, rows = 10;
                string sortField = "Id";
                int sortOrder = -1;

                if (!string.IsNullOrEmpty(lazyParams))
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var parsed = JsonSerializer.Deserialize<LazyParams>(lazyParams, options);
                    if (parsed != null)
                    {
                        first = parsed.First;
                        rows = parsed.Rows;
                        sortField = parsed.SortField ?? "Id";
                        sortOrder = parsed.SortOrder.HasValue ? parsed.SortOrder.Value : -1;
                    }
                }
                var query = db.Role.AsQueryable();
                if (!string.IsNullOrEmpty(text))
                {
                    query = query.Where(r => r.Name.Contains(text));
                }
                var totalRecords = query.Count();
                query = query.OrderByDynamic(sortField, sortOrder == 1);
                var result = query.Skip(first).Take(rows).Select(s => new { Id = s.Id, Name = s.Name, Description = s.Desc }).ToList();
                return Ok(new { roles = result, count = totalRecords });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddRole(RoleDto role)
        {
            try
            {
                if (!p.HasPermission(User, "Roles", "Add")) return BadRequest(new { Message = "Permission Denied!" });

                Role? alreadyExist = db.Role.SingleOrDefault(f => f.Name == role.Name);
                if (alreadyExist != null)
                {
                    return BadRequest("Role Already Exist");
                }
                else
                {
                    Role newRole = new Role
                    {
                        Name = role.Name,
                        Desc = role.Description
                    };
                    db.Role.Add(newRole);
                    db.SaveChanges();
                    return Ok(new { role });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult EditRole(int id, [FromBody] Role updatedData)
        {
            try
            {
                if (!p.HasPermission(User, "Roles", "Edit")) return BadRequest(new { Message = "Permission Denied!" });
                if (id <= 0)
                {
                    return BadRequest(new { error = "Role ID is required" });
                }
                if (string.IsNullOrWhiteSpace(updatedData.Name))
                {
                    return BadRequest(new { error = "RoleName is required" });
                }
                var role = db.Role.SingleOrDefault(r => r.Id == id);

                if (role == null)
                {
                    return NotFound(new { error = "Role not found" });
                }
                role.Name = updatedData.Name;
                role.Desc = updatedData.Desc;
                db.SaveChanges();
                return Ok(new { role });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult DeleteRole(int id)
        {
            try
            {
                if (!p.HasPermission(User, "Roles", "Delete")) return BadRequest(new { Message = "Permission Denied!" });
                var role = db.Role.SingleOrDefault(r => r.Id == id);
                if (role == null)
                {
                    return NotFound(new { error = "Role not found" });
                }
                db.Role.Remove(role);
                db.SaveChanges();
                return Ok(new { message = "Role deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("dropdown")]
        [Authorize]
        public IActionResult GetRoleDropdown()
        {
            try
            {
                var roles = db.Role.Select(r => new { r.Id, r.Name }).ToList();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        public class LazyParams
        {
            public int First { get; set; }
            public int Rows { get; set; }
            public int Page { get; set; }
            public string? SortField { get; set; }
            public int? SortOrder { get; set; } // 1 = ASC, -1 = DESC
        }

        public class RoleDto
        {
            public required string Name { get; set; }
            public string? Description { get; set; }
        }

    }
    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string sortField, bool ascending)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var property = Expression.PropertyOrField(param, sortField);
            var lambda = Expression.Lambda(property, param);
            string methodName = ascending ? "OrderBy" : "OrderByDescending";
            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type);
            return (IQueryable<T>)method.Invoke(null, new object[] { query, lambda })!;
        }
    }
}