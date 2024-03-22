using MedicalInformationSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalInformationSystem.Services;

public interface IDoctorService
{
    ActionResult Register( DoctorRegisterModel userRegisterModel);
    ActionResult Login( LoginCredentialsModel loginCredentials);
    ActionResult Logout();
    ActionResult<DoctorModel> GetProfile();
    ActionResult EditProfile(DoctorEditModel userEditModel);
}