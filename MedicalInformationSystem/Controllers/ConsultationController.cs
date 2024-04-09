using MedicalInformationSystem.Models;
using MedicalInformationSystem.Services.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MedicalInformationSystem.Controllers;

[ApiController]
[Route("/api/consultation/")]
public class ConsultationController : ControllerBase
{
    private readonly IJwtService _jwtService;

    public ConsultationController(/*IConsultationService consultationService,*/ IJwtService jwtService)
    {
        //_consultationService = consultationService;
        _jwtService = jwtService;
    }

    [Authorize(Policy = "TokenPolicy")]
    [SwaggerOperation(Summary = "Get a list of medical inspections for consultation")]
    [HttpGet("")]
    public ActionResult<InspectionPagedListModel> GetInspectionsForConsultation(
        [FromQuery] Guid[]? icdRoots,
        bool grouped = false,
        int page = 1,
        int size = 5
    )
    {
        try
        {
            return Ok(); //todo connect consultationService
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [Authorize(Policy = "TokenPolicy")]
    [SwaggerOperation(Summary = "Get concrete consultation")]
    [HttpGet("{id:guid}")]
    public ActionResult<ConsultationModel> GetConsultationInformation(Guid id)
    {
        try
        {
            return Ok(); //todo connect consultationService
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [Authorize(Policy = "TokenPolicy")]
    [SwaggerOperation(Summary = "Add comment to concrete consultation")]
    [HttpPost("{id:guid}/comment")]
    public ActionResult<Guid> AddConsultationComment(Guid id,
        [FromBody]
        CommentCreateModel commentCreateModel
        )
    {
        try
        {
            return Ok(); //todo connect consultationService
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    [Authorize(Policy = "TokenPolicy")]
    [SwaggerOperation(Summary = "Edit comment")]
    [HttpPut("comment/{id:guid}")]
    public ActionResult EditComment(Guid id,
        [FromBody]
        InspectionCommentCreateModel inspectionCommentCreateModel
    )
    {
        try
        {
            return Ok(); //todo connect consultationService
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}