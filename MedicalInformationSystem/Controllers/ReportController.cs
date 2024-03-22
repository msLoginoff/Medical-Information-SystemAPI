using System.ComponentModel.DataAnnotations;
using System.Net;
using MedicalInformationSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalInformationSystem.Controllers;

[ApiController]
[Route("api/report/icdrootsreport")]
public class ReportController : ControllerBase
{
    [HttpGet]
    public ActionResult<IcdRootsReportModel> GetPostsList(
        [FromQuery]
        [Required]
        DateTime start,
        [Required]
        DateTime end,
        [FromQuery]
        string[]? icdRoots
    )
    {
        /*try
        { 
            return _postService.GetPostsList(userId, tags, author, min, max, sorting, onlyMyCommunities, page, size);
        }
        catch (BadRequest e)
        {
            return new JsonResult(new Response
            {
                Status = "Error",
                Message = e.Message
            })
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }
        catch (NotAuthorize e)
        {
            return new JsonResult(new Response
            {
                Status = "Error",
                Message = e.Message
            })
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }*/
        return new IcdRootsReportModel();
    }
}