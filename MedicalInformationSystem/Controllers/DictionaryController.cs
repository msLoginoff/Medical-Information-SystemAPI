using System.Net;
using MedicalInformationSystem.Exceptions;
using MedicalInformationSystem.Models;
using MedicalInformationSystem.Services;
using MedicalInformationSystem.Services.DictionaryService;
using MedicalInformationSystem.Services.Jwt;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MedicalInformationSystem.Controllers;

[ApiController]
[Route("api/dictionary")]
public class DictionaryController : ControllerBase
{
    private readonly IDictionaryService _dictionaryService;
    private readonly IJwtService _jwtService;

    public DictionaryController(IDictionaryService dictionaryService, IJwtService jwtService)
    {
        _dictionaryService = dictionaryService;
        _jwtService = jwtService;
    }

    [HttpGet("speciality")]
    [SwaggerOperation(Summary = "Get specialities list")]
    public ActionResult<SpecialtiesPagedListModel> GetSpecialities([FromQuery] string? name,
        int page = 1,
        int size = 5)
    {
        try
        {
            return Ok(_dictionaryService.GetSpecialities(name, page, size));
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
        catch (NotFoundException e)
        {
            return new JsonResult(new Response
            {
                Status = "Error",
                Message = e.Message
            })
            {
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }
    }
    
    [HttpGet("icd10")]
    [SwaggerOperation(Summary = "Search for diagnoses in ICD-10 dictionary")]
    public ActionResult<Icd10SearchModel> GetIcd([FromQuery] string? request,
        int page = 1,
        int size = 5)
    {
        try
        {
            return Ok(_dictionaryService.GetSpecialities(request, page, size));
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
        catch (NotFoundException e)
        {
            return new JsonResult(new Response
            {
                Status = "Error",
                Message = e.Message
            })
            {
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }
    }
    
    [HttpGet("icd10/roots")]
    [SwaggerOperation(Summary = "Get root ICD-10 elements")]
    public ActionResult<Icd10RecordModel> GetIcdRoots()
    {
        try
        {
            return Ok();
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
        catch (NotFoundException e)
        {
            return new JsonResult(new Response
            {
                Status = "Error",
                Message = e.Message
            })
            {
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }
    }
}