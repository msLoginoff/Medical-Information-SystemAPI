using MedicalInformationSystem.Models;

namespace MedicalInformationSystem.Services.DoctorService;

public interface IDoctorService
{
    Guid Register( DoctorRegisterModel doctorRegisterModel);
    Guid Login( LoginCredentialsModel loginCredentials);
    Guid Logout(Guid doctorId);
    DoctorModel GetProfile(Guid doctorId);
    Guid EditProfile(DoctorEditModel userEditModel, Guid doctorId);
}