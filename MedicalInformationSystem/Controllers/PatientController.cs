using System.ComponentModel;
using System.Net;
using System.Security.Claims;
using MedicalInformationSystem.Exceptions;
using MedicalInformationSystem.Models;
using MedicalInformationSystem.Services;
using MedicalInformationSystem.Services.DoctorService;
using MedicalInformationSystem.Services.Jwt;
using MedicalInformationSystem.Services.PatientService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace MedicalInformationSystem.Controllers;
[ApiController]
[Route("api/patient")]
public class PatientController : ControllerBase
{
    private readonly IPatientService _patientService; //todo change on PatientService
    private readonly IJwtService _jwtService;

    public PatientController(IPatientService patientService, IJwtService jwtService)
    {
        _patientService = patientService;
        _jwtService = jwtService;
    }
    
    [Authorize(Policy = "TokenPolicy")]
    [HttpPost]
    [SwaggerOperation(Summary = "Create new patient")]
    public ActionResult CreatePatient([FromBody] PatientCreateModel patientCreateModel)
    {
        try
        {
            
            return Ok(_patientService.CreatePatient(patientCreateModel)); //todo connect patient service creating
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
    
    [Authorize(Policy = "TokenPolicy")]
    [HttpGet]
    [SwaggerOperation(Summary = "Get patients list")]
    public ActionResult<PatientPagedListModel> GetPatients([FromQuery] string? name,
        [FromQuery] List<ConclusionEnum> conclusions,
        PatientSortingEnum sorting,
        bool scheduledVisits,
        bool onlyMine,
        int page = 1,
        int size = 5)
    {
        try
        {
            var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(_patientService.GetPatients(doctorId, name,conclusions, sorting, scheduledVisits, onlyMine, page, size)); //todo add service
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
    
    [Authorize(Policy = "TokenPolicy")]
    [HttpPost("{patientId:guid}/inspections")]
    [SwaggerOperation(Summary = "Create inspection for specified patient")]
    public ActionResult CreateInspection(
        [FromBody] InspectionCreateModel inspectionCreateModel, Guid patientId)
    {
        try
        {
            var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(_patientService.CreateInspection(doctorId, patientId, inspectionCreateModel)); //todo connect patient service creating inspection, return guid
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
    
    [Authorize(Policy = "TokenPolicy")]
    [HttpGet("{patientId:guid}/inspections")]
    [SwaggerOperation(Summary = "Get a list of patient medical inspections")]
    public ActionResult<InspectionPagedListModel> GetInspections(
        Guid patientId,
        [FromQuery] 
        Guid[] icdRoots,
        bool grouped = false,
        int page = 1, 
        int size = 5)
    {
        try
        {
            var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(_patientService.GetPatientInspections(doctorId, patientId, icdRoots, grouped, page, size)); //todo connect patient service creating inspection, return Insp PagedListModel
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
    
    [Authorize(Policy = "TokenPolicy")]
    [HttpGet("{patientId:guid}")]
    [SwaggerOperation(Summary = "Get patient card")]
    public ActionResult<PatientModel> GetPatientCard(
        Guid patientId)
    {
        try
        {
            return Ok(_patientService.GetPatientCard(patientId)); //todo connect patient service, return PatientModel
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
    
    [Authorize(Policy = "TokenPolicy")]
    [HttpGet("{patientId:guid}/inspections/search")]
    [SwaggerOperation(Summary = "Search for patient medical inspections without child inspections")]
    public ActionResult<IEnumerable<InspectionShortModel>> GetPatientRootInspections( //todo надо уточнить, не факт что только рутовые
        Guid patientId,
        [FromQuery]
        string? request)
    {
        try
        {
            return Ok(_patientService.GetInspectionsWithoutChild(patientId, request)); //todo connect patient service, return Insp Short Model
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