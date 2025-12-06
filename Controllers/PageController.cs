using Infusive_back.EntityData;
using Infusive_back.JwtAuth;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hrlense.Controllers.hrlense
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {

        private readonly MyDbContext dbContext;
        private readonly CheckPermission permission;
        public PageController(MyDbContext dbContext, CheckPermission permission)
        {
            this.dbContext = dbContext;
            this.permission = permission;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Pages(string? lazyParams, string? search = null)
        {
            //if (!permission.HasPermission(User, "Page")) return BadRequest();

            //LazyParams param = JsonConvert.DeserializeObject<LazyParams>(lazyParams);

            var query = from pm in dbContext.Page
                        where pm.IsFeature == false && pm.ParentId == null
                        select new RolePermission
                        {
                            Id = pm.Id,
                            Name = pm.Name,
                            Label = pm.Label,
                            Url = pm.Url,
                            Description = pm.Description,
                            IsFeature = pm.IsFeature,
                            ParentId = pm.ParentId,
                            Parent = null,
                            Features = dbContext.Page
                                .Where(f => f.IsFeature == true && f.ParentId == pm.Id)
                                .Select(pf => new RolePermission
                                {
                                    Id = pf.Id,
                                    Name = pf.Name,
                                    Label = pf.Label,
                                    Url = pf.Url,
                                    Description = pf.Description,
                                    IsFeature = pm.IsFeature,
                                    ParentId = pm.ParentId,
                                    Parent = pm.Name,
                                }).ToList()
                        };


            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(search)

                );
            }

            var pages = query
                .OrderBy(o => o.Name)
                .ToList();

            var Totalcount = pages.Count();
            //var result = pages.Skip(param.First).Take(param.Rows).ToArray();

            return Ok(new { Totalcount, pages });
        }


        [HttpPost]
        [Authorize]
        public IActionResult Create_Page(CreatePage newPage)
        {
            //if (!permission.HasPermission(User, "Page")) return BadRequest();

            if (dbContext.Page.Any(f => f.IsFeature == false && f.Name == newPage.Name))
                return BadRequest("A page with this name already exists.");
            if (string.IsNullOrWhiteSpace(newPage.Url) || string.IsNullOrWhiteSpace(newPage.Label))
                return BadRequest("Url and Lable is required for page.");

            Page page = new()
            {
                Name = newPage.Name,
                Label = newPage.Label,
                Url = newPage.Url,
                Description = newPage.Desc,
                IsFeature = false,
                ParentId = null
            };
            dbContext.Page.Add(page);
            dbContext.SaveChanges();

            if (newPage.Features != null && newPage.Features.Count > 0)
            {
                foreach (var f in newPage.Features)
                {
                    Page feature = new()
                    {
                        Name = f.Name,
                        Label = null,
                        Url = null,
                        Description = f.Desc,
                        IsFeature = true,
                        ParentId = page.Id
                    };
                    dbContext.Page.Add(feature);
                }
                dbContext.SaveChanges();
            }
            permission.UpdatePage();
            return Ok(newPage);
        }

        [HttpPut("{id}")]
        [Authorize]
        public IActionResult Update_Page(int id, CreatePage page)
        {
            //if (!permission.HasPermission(User, "Page")) return BadRequest();

            if (id != page.Id) return BadRequest(new { Message = "Can't update, invalid pageId" });
            var db_page = dbContext.Page.Find(page.Id);
            if (db_page == null) return NotFound(new { Message = "Page not found" });

            db_page.Name = page.Name;
            db_page.Label = page.Label;
            db_page.Url = page.Url;
            db_page.Description = page.Desc;
            dbContext.Page.Update(db_page);
            dbContext.SaveChanges();

            if (page.Features != null && page.Features.Count > 0)
            {
                foreach (var f in page.Features)
                {
                    Page? feature = dbContext.Page.Find(f.FeatureId);
                    if (feature != null)
                    {
                        if (f.State == "remove")
                        {
                            dbContext.Page.Remove(feature);
                        }
                        else if (f.State == "update")
                        {
                            feature.Name = f.Name;
                            feature.Description = f.Desc;
                            dbContext.Page.Update(feature);
                        }
                    }
                    else if (f.State == "create")
                    {
                        Page new_feature = new()
                        {
                            Name = f.Name,
                            Label = null,
                            Url = null,
                            Description = f.Desc,
                            IsFeature = true,
                            ParentId = page.Id
                        };
                        dbContext.Page.Add(new_feature);
                    }
                }
                dbContext.SaveChanges();
            }
            permission.UpdatePage();
            return Ok(db_page);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete_Page(int id)
        {
            //if (!permission.HasPermission(User, "Page")) return BadRequest();

            var page = dbContext.Page.Find(id);
            if (page == null) return NotFound(new { Message = "Page not found" });

            var childPages = dbContext.Page.Where(p => p.ParentId == id).ToList();
            if (childPages.Count > 0)
            {
                dbContext.Page.RemoveRange(childPages);
                dbContext.SaveChanges();
            }
            dbContext.Page.Remove(page);
            dbContext.SaveChanges();
            permission.UpdatePage();
            return Ok(page);
        }

        [HttpGet]
        [Route("permission/{roleId}")]
        [Authorize]
        public IActionResult RolePermissions(int roleId)
        {
            //if (!permission.HasPermission(User, "Role")) return BadRequest("Permission not granted!");
            var rolePermissions = (from pm in dbContext.Page
                                   join pp in dbContext.Permission.Where(pp => pp.RoleId == roleId)
                                   on pm.Id equals pp.PageId into tpp
                                   from pp in tpp.DefaultIfEmpty()
                                   where pm.IsFeature == false && pm.ParentId == null
                                   select new RolePermission
                                   {
                                       Id = pm.Id,
                                       Name = pm.Name,
                                       Label = pm.Label,
                                       Url = pm.Url,
                                       Description = pm.Description,
                                       IsFeature = pm.IsFeature,
                                       ParentId = pm.ParentId,
                                       Parent = null,
                                       Permission = pp != null,
                                       Features = (from fm in dbContext.Page
                                                   join fp in dbContext.Permission.Where(fp => fp.RoleId == roleId)
                                                   on fm.Id equals fp.PageId into tpp
                                                   from fp in tpp.DefaultIfEmpty()
                                                   where fm.IsFeature == true && fm.ParentId == pm.Id
                                                   select new RolePermission
                                                   {
                                                       Id = fm.Id,
                                                       Name = fm.Name,
                                                       Label = fm.Label,
                                                       Url = fm.Url,
                                                       Description = fm.Description,
                                                       IsFeature = fm.IsFeature,
                                                       ParentId = fm.ParentId,
                                                       Parent = pm.Name,
                                                       Permission = fp != null,
                                                   })
                                    .OrderBy(o => o.Name).ToList()
                                   })
                                    .OrderBy(o => o.Name).ToList();
            return Ok(rolePermissions);
        }

        [HttpPost]
        [Route("permission")]
        [Authorize]
        public IActionResult AddRoleUpdatePermissions([FromBody] UpdateRolePermission rolePermissions)
        {
            try
            {
                //if (!permission.HasPermission(User, "Role")) return BadRequest();
                Role newRole = new() { Name = rolePermissions.Name, Desc = rolePermissions.Description };
                dbContext.Role.Add(newRole);
                dbContext.SaveChanges();

                var existingPermissions = dbContext.Permission.Where(f => f.RoleId == newRole.Id).ToList();

                var newPermissions = new List<Permission>();
                var permissionsToRemove = new List<Permission>();

                foreach (var pp in rolePermissions.Permissions)
                {
                    var dbpp = existingPermissions.FirstOrDefault(f => f.PageId == pp.PageId);

                    if (dbpp == null && pp.Permission)
                    {
                        newPermissions.Add(new Permission
                        {
                            RoleId = newRole.Id,
                            PageId = pp.PageId
                        });
                    }
                    else if (dbpp != null && !pp.Permission)
                    {
                        permissionsToRemove.Add(dbpp);
                    }
                }

                if (newPermissions.Any())
                {
                    dbContext.Permission.AddRange(newPermissions);
                }

                if (permissionsToRemove.Any())
                {
                    dbContext.Permission.RemoveRange(permissionsToRemove);
                }

                dbContext.SaveChanges();
                permission.UpdatePagePermission();
                return Ok(new { Message = "Role & Permissions updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"An error occurred while updating permissions: {ex.Message}" });
            }
        }

        [HttpPut]
        [Route("permission/{roleId}")]
        [Authorize]
        public IActionResult UpdateRoleAndPermissions(int roleId, [FromBody] UpdateRolePermission rolePermissions)
        {
            try
            {
                //if (!permission.HasPermission(User, "Role")) return BadRequest();

                if (roleId != rolePermissions.Id) return BadRequest(new { Message = "Can't update, invalid roleId" });
                var existingRole = dbContext.Role.Find(rolePermissions.Id);
                if (existingRole == null) return NotFound(new { Message = "Role not found" });

                existingRole.Name = rolePermissions.Name;
                existingRole.Desc = rolePermissions.Description;
                dbContext.Role.Update(existingRole);
                dbContext.SaveChanges();

                var existingPermissions = dbContext.Permission.Where(f => f.RoleId == roleId).ToList();

                var newPermissions = new List<Permission>();
                var permissionsToRemove = new List<Permission>();

                foreach (var pp in rolePermissions.Permissions)
                {
                    var dbpp = existingPermissions.FirstOrDefault(f => f.PageId == pp.PageId);

                    if (dbpp == null && pp.Permission)
                    {
                        newPermissions.Add(new Permission
                        {
                            RoleId = roleId,
                            PageId = pp.PageId
                        });
                    }
                    else if (dbpp != null && !pp.Permission)
                    {
                        permissionsToRemove.Add(dbpp);
                    }
                }

                if (newPermissions.Any())
                {
                    dbContext.Permission.AddRange(newPermissions);
                }

                if (permissionsToRemove.Any())
                {
                    dbContext.Permission.RemoveRange(permissionsToRemove);
                }

                dbContext.SaveChanges();
                permission.UpdatePagePermission();
                return Ok(new { Message = "Role & Permissions updated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"An error occurred while updating permissions: {ex.Message}" });
            }
        }

        public class UpdateRolePermission
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public string? Description { get; set; }
            public required List<PagePermission> Permissions { get; set; }
        }


        public class CreateFeature
        {
            public int? FeatureId { get; set; }
            public required string Name { get; set; }
            public required string State { get; set; } // create | update | remove
            public string? Desc { get; set; }
        }
        public class CreatePage
        {
            public int? Id { get; set; }
            public required string Name { get; set; }
            public required string Label { get; set; }
            public required string Url { get; set; }
            public string? Desc { get; set; }
            public List<CreateFeature>? Features { get; set; }
        }

        public class RolePermission
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public string? Label { get; set; }
            public string? Url { get; set; }
            public string? Description { get; set; }
            public bool IsFeature { get; set; }
            public int? ParentId { get; set; }
            public string? Parent { get; set; }
            public bool Permission { get; set; }
            public List<RolePermission>? Features { get; set; }
        }

        public class PagePermission
        {
            public int PageId { get; set; }
            public bool Permission { get; set; }
        }

    }

}