using Infusive_back.EntityData;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace Infusive_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndustryTypeController(MyDbContext db) : ControllerBase
    {
        readonly private MyDbContext db = db;

        [HttpGet]
        public IActionResult GetIndustryTypes([FromQuery] int? skip, [FromQuery] int? limit, [FromQuery] string? search)
        {
            try
            {

                var query = (from ityp in db.IndustryType
                             select new GetIndustryType
                             {
                                 Id = ityp.Id,
                                 IndustryName = ityp.IndustryName ?? "",
                                 Status = ityp.Status ?? false,
                             });

                var totalCount = query.Count();

                var indusrties = query.OrderBy(u => u.Id).Skip(skip ?? 0).Take(limit ?? 10).ToList();

                return Ok(new { totalCount = totalCount, indusrties = indusrties });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        [Authorize]
        public IActionResult AddIndustryType([FromBody] PostEditIndustryType pi)
        {
            try
            {
                int userId = Int32.Parse(User.FindFirst("userId")?.Value);

                IndustryType? alreadyExist = db.IndustryType.AsEnumerable().FirstOrDefault(f => Regex.Replace(f.IndustryName, @"\s+", "").ToLower() == Regex.Replace(pi.IndustryName, @"\s+", "").ToLower());

                if (alreadyExist != null)
                {
                    return BadRequest("Already Exist");
                }

                IndustryType newIndustry = new()
                {
                    IndustryName = pi.IndustryName,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId,
                };
                db.IndustryType.Add(newIndustry);
                db.SaveChanges();

                GetIndustryType response = new()
                {
                    Id = newIndustry.Id,
                    IndustryName = newIndustry.IndustryName,
                    Status = newIndustry.Status ?? false,
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
        public IActionResult UpdateIndustryType( [FromBody] PostEditIndustryType ei)
        {
            try
            {
                int userId = Int32.Parse(User.FindFirst("userId")?.Value);

                //if (id != ei.Id)
                //    return BadRequest("Invalid Record Id");

                IndustryType? industryTypeToUpdate = db.IndustryType.Find(ei.Id);
                if (industryTypeToUpdate == null)
                    return NotFound("Industry Type not found");

                bool nameExists = db.IndustryType.AsEnumerable()
                    .Any(f => Regex.Replace(f.IndustryName, @"\s+", "").ToLower() == Regex.Replace(ei.IndustryName, @"\s+", "").ToLower() && f.Id != ei.Id);
                if (nameExists)
                    return BadRequest("Industry Type name already exists");

                industryTypeToUpdate.IndustryName = ei.IndustryName;
                industryTypeToUpdate.Status = ei.Status;
                industryTypeToUpdate.UpdatedAt = DateTime.Now;
                industryTypeToUpdate.UpdatedBy = userId;

                db.IndustryType.Update(industryTypeToUpdate);
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
        public IActionResult ToggleIndustryTypeStatus(int id, [FromBody] ToggleStatus ts)
        {
            try
            {

                if (id != ts.Id) return BadRequest("Invalid Record Id");

                IndustryType? ExistingIndustry = db.IndustryType.FirstOrDefault(f => f.Id == id);

                if (ExistingIndustry == null) return NotFound("Record Not Found");

                ExistingIndustry.Status = ts.Status;

                db.IndustryType.Update(ExistingIndustry);
                db.SaveChanges();

                return Ok(new { Message = "Status Changed successfully" });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class GetIndustryType
        {
            public required int Id { get; set; }
            public required string IndustryName { get; set; }
            public required bool Status { get; set; }
        }
        public class PostEditIndustryType
        {
            public int? Id { get; set; }
            public required string IndustryName { get; set; }
            public bool? Status { get; set; } = true;
        }
        public class ToggleStatus
        {
            public required int Id { get; set; }
            public required bool Status { get; set; }
        }

    }
}
