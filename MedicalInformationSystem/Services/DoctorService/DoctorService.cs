using MedicalInformationSystem.Data;
using MedicalInformationSystem.Data.Entities;
using MedicalInformationSystem.Exceptions;
using MedicalInformationSystem.Models;
using MedicalInformationSystem.Services.HashService;
using Microsoft.AspNetCore.Mvc;

namespace MedicalInformationSystem.Services.DoctorService;

public class DoctorService : IDoctorService
{
    private readonly AppDbContext _context;
    private readonly HashPasswordService _hashPasswordService;

    public DoctorService(AppDbContext context, HashPasswordService hashPasswordService)
    {
        _context = context;
        _hashPasswordService = hashPasswordService;
    }
    
    public Guid Register(DoctorRegisterModel doctorRegisterModel)
    {
        var salt = _hashPasswordService.GenerateSalt();
        var hashedPassword = _hashPasswordService.HashPassword(doctorRegisterModel.Password, salt);
        var doctorId = Guid.NewGuid();

        var existingDoctor = _context.Doctor.SingleOrDefault(x =>
            x.Email == doctorRegisterModel.Email || (x.Phone != null && x.Phone == doctorRegisterModel.Phone));
        
        if (existingDoctor != null)
        {
            throw new BadRequest("This username or phone number is already taken.");
        }

        var speciality = _context.Speciality.FirstOrDefault(x => x.Id == doctorRegisterModel.Speciality);

        if (speciality == null)
        {
            throw new NotFoundException("Speciality not found");
        }

        var newDoctor = new DoctorEntity
        {
            Id = doctorId,
            CreateTime = DateTime.Now,
            Email = doctorRegisterModel.Email,
            Name = doctorRegisterModel.Name,
            BirthDateTime = doctorRegisterModel.BirthDateTime,
            Gender = doctorRegisterModel.Gender,
            Phone = doctorRegisterModel.Phone,
            Speciality = speciality
        };

        var newPass = new PasswordEntity
        {
            Id = Guid.NewGuid(),
            Doctor = newDoctor,
            Salt = salt,
            HashedPassword = hashedPassword,
            TokenSeries = 0
        };

        _context.Doctor.Add(newDoctor);
        _context.Password.Add(newPass);
        _context.SaveChanges();

        return doctorId;
    }

    public Guid Login(LoginCredentialsModel loginCredentials)
    {
        var doctorId = _context.Doctor.FirstOrDefault(x => x.Email == loginCredentials.Email)?.Id;
        var passwordItem = _context.Password.FirstOrDefault(x => x.Doctor.Id == doctorId);
        
        if (doctorId == null || passwordItem == null)
        {
            throw new BadRequest("Login failed");
        }

        var hashedPassword = _hashPasswordService.HashPassword(loginCredentials.Password, passwordItem.Salt);

        if (hashedPassword == passwordItem.HashedPassword)
        {
            return (Guid)doctorId;
        }
        
        throw new BadRequest("Invalid password or username");
    }

    public Guid Logout(Guid doctorId)
    {
        var passwordItem = _context.Password.FirstOrDefault(x => x.Doctor.Id == doctorId);
        
        if (passwordItem == null)
        {
            throw new NotAuthorize("Unauthorized");
        }

        passwordItem.TokenSeries++;
        _context.Password.Update(passwordItem);
        _context.SaveChanges();

        return doctorId;
    }

    public DoctorModel GetProfile(Guid doctorId)
    {
        var doctor = _context.Doctor.FirstOrDefault(x => x.Id == doctorId);
        
        if (doctor == null)
        {
            throw new NotFoundException("User not found");
        }

        var doctorModel = new DoctorModel
        {
            Id = doctor.Id,
            CreateTime = doctor.CreateTime,
            Name = doctor.Name,
            BirthDateTime = doctor.BirthDateTime,
            Gender = doctor.Gender,
            Email = doctor.Email,
            Phone = doctor.Phone
        };
        
        return doctorModel;
    }

    public Guid EditProfile(DoctorEditModel userEditModel, Guid doctorId)
    {
        var doctor = _context.Doctor.FirstOrDefault(x => x.Id == doctorId);
        
        if (doctor == null)
        {
            throw new NotFoundException("User not found");
        }

        var existingEmail = _context.Doctor.FirstOrDefault(x => x.Email == userEditModel.Email && x.Id != doctorId);
        
        if (existingEmail != null)
        {
            throw new BadRequest($"User '{existingEmail.Email}' already exists");
        }
        
        var existingPhone = _context.Doctor.FirstOrDefault(x => x.Phone == userEditModel.Phone && x.Id != doctorId);

        if (existingPhone != null)
        {
            throw new BadRequest($"User with phone number '{existingPhone.Phone}' already exists");
        }
        
        doctor.Name = userEditModel.Name;
        doctor.BirthDateTime = userEditModel.BirthDateTime;
        doctor.Gender = userEditModel.Gender;
        doctor.Email = userEditModel.Email;
        doctor.Phone = userEditModel.Phone;

        _context.Doctor.Update(doctor);
        _context.SaveChanges();

        return doctorId;
    }
}