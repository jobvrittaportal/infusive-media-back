using Infusive_back.EntityData;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Infusive_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController(MyDbContext db) : ControllerBase
    {
        readonly private MyDbContext db = db;

        [HttpGet]
        public IActionResult GetStatuss([FromQuery] int? skip, [FromQuery] int? limit, [FromQuery] string? search)
        {
            try
            {


                var query = (from ityp in db.Status
                             select new GetStatus
                             {
                                 Id = ityp.Id,
                                 StatusName = ityp.StatusName ?? "",
                                 Status = ityp.IsActive ?? false,
                             });

                if (!string.IsNullOrEmpty(search))
                {
                    var lower = search.ToLower();
                    query = query.Where(u =>
                        u.StatusName.ToLower().Contains(lower)
                    );
                }

                var totalCount = query.Count();

                var status = query.OrderBy(u => u.Id).Skip(skip ?? 0).Take(limit ?? 10).ToList();

                return Ok(new { totalCount = totalCount, status = status });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Authorize]
        public IActionResult AddStatus([FromBody] PostEditStatus pi)
        {
            try
            {
                int userId = Int32.Parse(User.FindFirst("userId")?.Value);

                Status? alreadyExist = db.Status.AsEnumerable().FirstOrDefault(f => Regex.Replace(f.StatusName, @"\s+", "").ToLower() == Regex.Replace(pi.StatusName, @"\s+", "").ToLower());

                if (alreadyExist != null)
                {
                    return BadRequest("Already Exist");
                }

                Status newStatus = new()
                {
                    StatusName = pi.StatusName,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                };
                db.Status.Add(newStatus);
                db.SaveChanges();

                GetStatus response = new()
                {
                    Id = newStatus.Id,
                    StatusName = newStatus.StatusName,
                    Status = newStatus.IsActive ?? false,
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
        public IActionResult UpdateStatus([FromBody] PostEditStatus ei)
        {
            try
            {
                int userId = Int32.Parse(User.FindFirst("userId")?.Value);

                //if (id != ei.Id)
                //    return BadRequest("Invalid Record Id");

                Status? statusToUpdate = db.Status.Find(ei.Id);
                if (statusToUpdate == null)
                    return NotFound("Status not found");

                bool nameExists = db.Status.AsEnumerable()
                    .Any(f => Regex.Replace(f.StatusName, @"\s+", "").ToLower() == Regex.Replace(ei.StatusName, @"\s+", "").ToLower() && f.Id != ei.Id);
                if (nameExists)
                    return BadRequest("Status name already exists");

                statusToUpdate.StatusName = ei.StatusName;
                statusToUpdate.IsActive = ei.Status;
                statusToUpdate.UpdatedAt = DateTime.Now;
                statusToUpdate.UpdatedBy = userId;

                db.Status.Update(statusToUpdate);
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
        public IActionResult ToggleeStatus(int id, [FromBody] ToggleStatus ts)
        {
            try
            {

                if (id != ts.Id) return BadRequest("Invalid Record Id");

                Status? ExistingStatus = db.Status.FirstOrDefault(f => f.Id == id);

                if (ExistingStatus == null) return NotFound("Record Not Found");

                ExistingStatus.IsActive = ts.Status;

                db.Status.Update(ExistingStatus);
                db.SaveChanges();

                return Ok(new { Message = "Status Changed successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class GetStatus
        {
            public required int Id { get; set; }
            public required string StatusName { get; set; }
            public required bool Status { get; set; }
        }
        public class PostEditStatus
        {
            public int? Id { get; set; }
            public required string StatusName { get; set; }
            public bool? Status { get; set; } = true;
        }
        public class ToggleStatus
        {
            public required int Id { get; set; }
            public required bool Status { get; set; }
        }

    }
}
