using System.Net;
using System.Security.Claims;
using MedicalInformationSystem.Exceptions;
using MedicalInformationSystem.Models;
using MedicalInformationSystem.Services.InspectionService;
using MedicalInformationSystem.Services.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace MedicalInformationSystem.Controllers;

[ApiController]
[Route("/api/inspection/")]
public class InspectionController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IInspectionService _inspectionService;

    public InspectionController(IInspectionService inspectionService, IJwtService jwtService)
    {
        _inspectionService = inspectionService;
        _jwtService = jwtService;
    }

    [Authorize(Policy = "TokenPolicy")]
    [SwaggerOperation(Summary = "Get full information about specified inspection")]
    [HttpGet("{id:guid}")]
    public ActionResult<InspectionModel> GetInspectionInformation(
        Guid id)
    {
        try
        {
            return Ok(_inspectionService.GetInspectionInformation(id)); //todo connect inspectionService
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
        catch (Forbidden e)
        {
            return new JsonResult(new Response
            {
                Status = "Error",
                Message = e.Message
            })
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }
        catch (ServerError e)
        {
            return new JsonResult(new Response
            {
                Status = "Error",
                Message = e.Message
            })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
    
    [Authorize(Policy = "TokenPolicy")]
    [SwaggerOperation(Summary = "Edit concrete inspection")]
    [HttpPut("{id:guid}")]
    public ActionResult EditInspectionInformation(
        Guid id,
        [FromBody]
        InspectionEditModel inspectionEditModel
    )
    {
        try
        {
            var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return Ok(_inspectionService.EditInspectionInformation(id, doctorId, inspectionEditModel)); //todo connect inspectionService
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
        catch (Forbidden e)
        {
            return new JsonResult(new Response
            {
                Status = "Error",
                Message = e.Message
            })
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }
        catch (ServerError e)
        {
            return new JsonResult(new Response
            {
                Status = "Error",
                Message = e.Message
            })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
    }
    
    [Authorize(Policy = "TokenPolicy")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ServerError))]
    [SwaggerOperation(Summary = "Get medical inspection chain for root chain")]
    [HttpGet("{id:guid}/chain")]
    public ActionResult<InspectionPreviewModel> GetInspectionChain(
        Guid id)
    {
        try
        {
            return Ok(_inspectionService.GetInspectionChain(id)); //todo connect inspectionService
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
        catch (Forbidden e)
        {
            return new JsonResult(new Response
            {
                Status = "Error",
                Message = e.Message
            })
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }
        catch (Exception ex)
        {
            var errorResponse = new ServerError("");
            return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
        }
    }
}