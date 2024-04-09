using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using MedicalInformationSystem.Exceptions;
using MedicalInformationSystem.Models;
using MedicalInformationSystem.Services;
using MedicalInformationSystem.Services.DoctorService;
using MedicalInformationSystem.Services.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace MedicalInformationSystem.Controllers;
[ApiController]
[Route("api/doctor")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorService _doctorService;
    private readonly IJwtService _jwtService;

    public DoctorController(IDoctorService doctorService, IJwtService jwtService)
    {
        _doctorService = doctorService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register new user")]
    public ActionResult Register([FromBody] DoctorRegisterModel doctorRegisterModel) // todo токен возвращаемый тип
    {
        try
        {
            var userId = _doctorService.Register(doctorRegisterModel);
            var token = _jwtService.GenerateToken(userId);

            return Ok(new TokenResponseModel { Token = token });
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
    
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login in to the system")]
    public ActionResult Login([FromBody] LoginCredentialsModel loginCredentials)
    {
        try
        {
            var userId = _doctorService.Login(loginCredentials);
            var token = _jwtService.GenerateToken(userId);

            return Ok(new TokenResponseModel { Token = token });
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
    [HttpPost("logout")]
    [SwaggerOperation(Summary = "Log out system user")]
    public ActionResult Logout() //todo сменить тип
    {
        try
        {
            var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            _doctorService.Logout(doctorId);
            return Ok(new Response
            {
                Status = null,
                Message = "Logged out"
            });
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
    [HttpGet("profile")]
    [SwaggerOperation(Summary = "Get user profile")]
    public ActionResult<DoctorModel> GetProfile()
    {
        try
        {
            var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            return Ok(_doctorService.GetProfile(doctorId));
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
    [HttpPut("profile")]
    [SwaggerOperation(Summary = "Edit user profile")]
    public ActionResult EditProfile([FromBody] DoctorEditModel userEditModel)
    {
        try
        {
            var doctorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            _doctorService.EditProfile(userEditModel, doctorId);
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
    
}