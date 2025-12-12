using Infusive_back.EntityData;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Infusive_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SourceController(MyDbContext db) : ControllerBase
    {
        readonly private MyDbContext db = db;

        [HttpGet]
        public IActionResult GetSourcee([FromQuery] int? skip, [FromQuery] int? limit, [FromQuery] string? search)
        {
            try
            {

                var query = (from ityp in db.Source
                             select new GetSource
                             {
                                 Id = ityp.Id,
                                 SourceName = ityp.SourceName ?? "",
                                 Status = ityp.Status ?? false,
                             });

                if (!string.IsNullOrEmpty(search))
                {
                    var lower = search.ToLower();
                    query = query.Where(u =>
                        u.SourceName.ToLower().Contains(lower)
                    );
                }

                var totalCount = query.Count();

                var source = query.OrderBy(u => u.Id).Skip(skip ?? 0).Take(limit ?? 10).ToList();

                return Ok(new { totalCount = totalCount, indusrties = source });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Authorize]
        public IActionResult AddSource([FromBody] PostEditSource pi)
        {
            try
            {
                int userId = Int32.Parse(User.FindFirst("userId")?.Value);

                Source? alreadyExist = db.Source.AsEnumerable().FirstOrDefault(f => Regex.Replace(f.SourceName, @"\s+", "").ToLower() == Regex.Replace(pi.SourceName, @"\s+", "").ToLower());

                if (alreadyExist != null)
                {
                    return BadRequest("Already Exist");
                }

                Source newSource = new()
                {
                    SourceName = pi.SourceName,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                };
                db.Source.Add(newSource);
                db.SaveChanges();

                GetSource response = new()
                {
                    Id = newSource.Id,
                    SourceName = newSource.SourceName,
                    Status = newSource.Status ?? false,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Authorize]
        public IActionResult UpdateStatua([FromBody] PostEditSource ei)
        {
            try
            {
                int userId = Int32.Parse(User.FindFirst("userId")?.Value);

                //if (id != ei.Id)
                //    return BadRequest("Invalid Record Id");

                Source? sourceToUpdate = db.Source.Find(ei.Id);
                if (sourceToUpdate == null)
                    return NotFound("Source not found");

                bool nameExists = db.Source.AsEnumerable()
                    .Any(f => Regex.Replace(f.SourceName, @"\s+", "").ToLower() == Regex.Replace(ei.SourceName, @"\s+", "").ToLower() && f.Id != ei.Id);
                if (nameExists)
                    return BadRequest(" Source name already exists");

                sourceToUpdate.SourceName = ei.SourceName;
                sourceToUpdate.Status = ei.Status;
                sourceToUpdate.UpdatedAt = DateTime.Now;
                sourceToUpdate.UpdatedBy = userId;

                db.Source.Update(sourceToUpdate);
                db.SaveChanges();

                return Ok(new { Message = "Updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("toggleStatus/{id}")]
        [Authorize]
        public IActionResult ToggleSourceStatus(int id, [FromBody] ToggleStatus ts)
        {
            try
            {

                if (id != ts.Id) return BadRequest("Invalid Record Id");

                Source? ExistingSource = db.Source.FirstOrDefault(f => f.Id == id);

                if (ExistingSource == null) return NotFound("Record Not Found");

                ExistingSource.Status = ts.Status;

                db.Source.Update(ExistingSource);
                db.SaveChanges();

                return Ok(new { Message = "Status Changed successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class GetSource
        {
            public required int Id { get; set; }
            public required string SourceName { get; set; }
            public required bool Status { get; set; }
        }
        public class PostEditSource
        {
            public int? Id { get; set; }
            public required string SourceName { get; set; }
            public bool? Status { get; set; } = true;
        }
        public class ToggleStatus
        {
            public required int Id { get; set; }
            public required bool Status { get; set; }
        }

    }
}
