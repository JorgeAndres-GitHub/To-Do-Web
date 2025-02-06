using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_InterfaceAdapters_Mappers.DTOs.Requests.User;

namespace ToDo_Backend_InterfaceAdapters_Mappers.Mappers.UserMappers
{
    public class UserMapper : IAccountMapper<UserRegistrationRequestDTO, UserEntity>
    {
        public UserEntity ToEntity(UserRegistrationRequestDTO dto) => new UserEntity
        {

            FirstName = dto.FirstName,
            LastName = dto.LastName,
            IdentificationNumber = dto.IdentificationNumber,
            Country = dto.Country,
            City = dto.City,
            Phone = dto.Phone,
            Email = dto.Email,
            Password = dto.Password,
            CreatedTasks = 0,
            CompletedTasks = 0,
            PublishedTasks = 0,
            IsEmailConfirmed = false,
            VerificationCode = null,
            UpdateConfirmationCode = null,
            IdRol = 3
        };
    }
}
