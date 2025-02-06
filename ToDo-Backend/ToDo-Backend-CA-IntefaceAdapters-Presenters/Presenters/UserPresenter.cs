using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_AplicationLayer.Interfaces.UserAplicationInterfaces;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_IntefaceAdapters_Presenters.Views;

namespace ToDo_Backend_CA_IntefaceAdapters_Presenters.Presenters
{
    public class UserPresenter : IAccountPresenter<UserEntity, UserViewModel>
    {
        public UserViewModel Present(UserEntity u)
            => new UserViewModel
            {
                FullName = u.FirstName + " " + u.LastName,
                IdentificationNumer = u.IdentificationNumber,
                Location = u.Country + ", " + u.City,
                PhoneNumber = u.Phone,
                Email = u.Email,
                CreatedTasks = $"Has creado {u.CreatedTasks} tareas",
                CompletedTasks = $"Has completado {u.CompletedTasks} tareas",
                PublishedTasks = $"Has publicado {u.PublishedTasks} tareas"
            };
    }
}
