using Infusive_back.EntityData;
using Infusive_back.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static Infusive_back.Controllers.RoleController;

namespace Infusive_back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupportController : ControllerBase
    {
        private readonly MyDbContext db;
        public SupportController(MyDbContext db)
        {
            this.db = db;
        }
        private async Task<string> GetHrlenseToken()
        {
            int userId = Int32.Parse(User.FindFirst("userId")?.Value);

            var user = db.User_Details.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                throw new Exception("User not found in the database");

            var employeeId = user.EmpId;

            using var client = new HttpClient();

            var response = await client.PostAsync(
                $"https://hrms-demo.jobvritta.com/api/DropDown/generateToken?employee_Code={employeeId}",
                null
            );

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to generate HRMS token");

            var json = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            return json.GetProperty("token").GetString();
        }

        [HttpGet("CheckUser")]
        [Authorize]
        public async Task<IActionResult> CheckUser([FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { success = false, message = "UserId is required" });
                }

                using (var httpClient = new HttpClient())
                {
                    // Add required header for external API
                    httpClient.DefaultRequestHeaders.Add("hrms-key", "Amar@Deep1Jobvritta#157");

                    var url = $"https://hrms-demo.jobvritta.com/api/Hrlense_Employee/employeeCodeCheck?empCode={userId}";

                    var response = await httpClient.GetAsync(url);

                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "User found"
                        });

                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return Ok(new { success = false, message = "User not found" });
                    }
                    else
                    {
                        return Ok(new
                        {
                            success = false,
                            message = "User inactive or invalid employee"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("ApplicationGroup")]
        [Authorize]
        public async Task<IActionResult> ApplicationGroupDropDown()
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(
                    "https://hrms-demo.jobvritta.com/api/DropDown/ApplicationGroup"
                );

                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("supportAssignGroup")]
        [Authorize]
        public async Task<IActionResult> SupportGroup()
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(
                    "https://hrms-demo.jobvritta.com/api/DropDown/supportAssignGroup"
                );

                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet]
        [Route("supportStatus")]
        [Authorize]

        public async Task<IActionResult> getSupportStatus()
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(
                    "https://hrms-demo.jobvritta.com/api/DropDown/supportStatus"
                );

                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        

        [HttpPost("CreateTicket")]
        [Authorize]
        public async Task<IActionResult> CreateTicket([FromForm] CreateSupportTicketDto dto)
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                var formData = new MultipartFormDataContent
                {
                    { new StringContent(dto.Application_Id.ToString()), "Application_Id" },
                    { new StringContent(dto.Priority), "Priority" },
                    { new StringContent(dto.Impact), "Impact" },
                    { new StringContent(dto.Assign_Group_Id.ToString()), "Assign_Group_Id" },
                    { new StringContent(dto.Subject), "Subject" },
                    { new StringContent(dto.Desc), "Desc" }
                };

                if (dto.Screenshot != null)
                {
                    var stream = dto.Screenshot.OpenReadStream();
                    formData.Add(new StreamContent(stream), "Screenshot", dto.Screenshot.FileName);
                }

                var response = await client.PostAsync(
                    "https://hrms-demo.jobvritta.com/api/Support/CreateTicket",
                    formData
                );

                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("getTicket")]
        [Authorize]
        public async Task<IActionResult> GetTickets([FromQuery] string? filter, [FromQuery] string? lazyParams)
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                string url =
                    $"https://hrms-demo.jobvritta.com/api/Support/getTicket" +
                    $"?filter={Uri.EscapeDataString(filter ?? "")}" +
                    $"&lazyParams={Uri.EscapeDataString(lazyParams ?? "")}";

                var response = await client.GetStringAsync(url);

                var json = JsonDocument.Parse(response).RootElement;

                return Ok(new
                {
                    totalCount = json.GetProperty("totalCount").GetInt32(),
                    result = json.GetProperty("result")
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("getTicketHistory")]
        [Authorize]
        public async Task<IActionResult> GetTicketHistory(int ticket_Id)
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                string url = $"https://hrms-demo.jobvritta.com/api/Support/getTicketHistory?ticket_Id={ticket_Id}";

                var response = await client.GetAsync(url);
                string jsonString = await response.Content.ReadAsStringAsync();

                var json = JsonDocument.Parse(jsonString).RootElement;
                return Ok(json);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[HttpPut]
        //[Route("changeTicketStatus")]
        //[Authorize]
        //public async Task<IActionResult> ChangeStatus(ChangeStatusDto data)
        //{
        //    try
        //    {
        //        string token = await GetHrlenseToken();

        //        using var client = new HttpClient();
        //        client.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("Bearer", token);

        //        var form = new MultipartFormDataContent();
        //        form.Add(new StringContent(data.Ticket_Id.ToString()), "Ticket_Id");
        //        form.Add(new StringContent(data.Status.ToString()), "Status");
        //        form.Add(new StringContent(data.Remark ?? ""), "Remark");

        //        var response = await client.PutAsync(
        //            "https://hrms-demo.jobvritta.com/api/DropDown/changeTicketStatus",
        //            form
        //        );

        //        var result = await response.Content.ReadAsStringAsync();

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            return BadRequest(new { message = "External API failed", response = result });
        //        }

        //        return Ok(new { message = "Status updated successfully"});
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        [HttpPut("changeTicketStatus")]
        [Authorize]
        public async Task<IActionResult> ChangeTicketStatus([FromBody] ChangeStatusDto model)
        {
            try
            {
                string token = await GetHrlenseToken();
                using var client = new HttpClient();

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Put,
                    "https://hrms-demo.jobvritta.com/api/Support/changeTicketStatus");

                request.Content = content;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request);
                string hrmsResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, hrmsResponse);

                return Ok(new { message = "Ticket status Updated Successfully"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("updateTicket")]
        [Authorize]
        public async Task<IActionResult> UpdateTicket([FromBody] TicketUpdateDto model)
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient(); // <-- MISSING BEFORE

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(
                    HttpMethod.Put,
                    "https://hrms-demo.jobvritta.com/api/Support/updateTicket"
                );

                request.Content = content;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request);
                string hrmsResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, hrmsResponse);

                return Ok(new { message = "Ticket Updated Successfully", hrmsResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("getNotes")]
        [Authorize]
        public async Task<IActionResult> GetExternalNotes(int ticket_Id)
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();

                string url = $"https://hrms-demo.jobvritta.com/api/Support/getNotes?ticket_Id={ticket_Id}";

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request);
                string apiResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, apiResponse);
                }

                var result = JsonSerializer.Deserialize<object>(apiResponse);
                return Ok(result);


            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("addNote")]
        [Authorize]
        public async Task<IActionResult> AddExternalNote([FromForm] AddNoteDto dto)
        {
            try
            {
                string token = await GetHrlenseToken();

                using var client = new HttpClient();
                using var formData = new MultipartFormDataContent();

                // Required text fields
                formData.Add(new StringContent(dto.Ticket_Id.ToString()), "Ticket_Id");
                formData.Add(new StringContent(dto.Note_Text ?? ""), "Note_Text");

                // Optional attachment
                if (dto.Attachment != null)
                {
                    var streamContent = new StreamContent(dto.Attachment.OpenReadStream());
                    streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(dto.Attachment.ContentType);

                    formData.Add(streamContent, "Attachment", dto.Attachment.FileName);
                }

                string url = "https://hrms-demo.jobvritta.com/api/Support/addNote";
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = formData
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request);
                var apiResponse = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, apiResponse);

                return Ok(JsonSerializer.Deserialize<object>(apiResponse));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("NoteFile/{*filename}")]
        [Authorize]
        public async Task<IActionResult> GetExternalNoteFile(string filename)
        {
            try
            {
                if (string.IsNullOrEmpty(filename))
                    return BadRequest("Filename required");

                string token = await GetHrlenseToken();

                string url = $"https://hrms-demo.jobvritta.com/api/Support/NoteFile/{Uri.EscapeDataString(filename)}";

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    var msg = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, msg);
                }

                var stream = await response.Content.ReadAsStreamAsync();
                var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

                return File(stream, contentType, filename);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        public class CreateSupportTicketDto
        {
            public int Application_Id { get; set; }
            public string Priority { get; set; }
            public string Impact { get; set; }
            public int Assign_Group_Id { get; set; }
            public string Subject { get; set; }
            public string Desc { get; set; }
            public IFormFile? Screenshot { get; set; }
        }

        public class ChangeStatusDto
        {
            public int Ticket_Id { get; set; }
            public int Status { get; set; }
            public string Remark { get; set; }
        }
        public class TicketUpdateDto
        {
            public int Ticket_Id { get; set; }
            public int Application_Id { get; set; }
            public int Assign_Group_Id { get; set; }
            public string Priority { get; set; }
            public string Impact { get; set; }
            public string Subject { get; set; }
            public string Desc { get; set; }

            public List<TicketChangeDto>? Changes { get; set; }
        }

        public class TicketChangeDto
        {
            public string Field_Name { get; set; }
            public string Old_Value { get; set; }
            public string New_Value { get; set; }
        }

        public class AddNoteDto
        {
            public int Ticket_Id { get; set; }
            public string Note_Text { get; set; }
            public IFormFile? Attachment { get; set; }
        }
    }
}