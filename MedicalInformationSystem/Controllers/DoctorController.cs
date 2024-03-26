using System.IdentityModel.Tokens.Jwt;
using MedicalInformationSystem.Jwt;
using MedicalInformationSystem.Models;
using MedicalInformationSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MedicalInformationSystem.Controllers;

public class DoctorController : ControllerBase
{
    //private readonly IDoctorService _doctorService;
    private readonly IJwtService _jwtService;

    public DoctorController(/*IDoctorService doctorService,*/ IJwtService jwtService)
    {
        //_doctorService = doctorService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public ActionResult Register([FromBody] DoctorRegisterModel userRegisterModel) // todo токен возвращаемый тип
    {
            throw new NotImplementedException();
    }
    
    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginCredentialsModel loginCredentials)
    {
        /*var response = new
        {
            access_token = encodedJwt,
            username = identity.Name
        };

        return new JsonResult(response);*/
        throw new NotImplementedException();
    }
    
    //[Authorize]//todo сделать функцию для авторизированного пользователя
    [HttpPost("logout")]
    public ActionResult Logout() //todo сменить тип
    {
        throw new NotImplementedException();
    }
    
    //[Authorize]//todo сделать функцию для авторизированного пользователя
    [HttpGet("profile")]
    public ActionResult<DoctorModel> GetProfile()
    {
        throw new NotImplementedException();
    }
    
    //[Authorize]//todo сделать функцию для авторизированного пользователя
    [HttpPut("profile")]
    public ActionResult EditProfile([FromBody] DoctorEditModel userEditModel)
    {
        throw new NotImplementedException();
    }
    
}