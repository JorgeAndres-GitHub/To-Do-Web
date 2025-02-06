using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_Backend_CA_AplicationLayer.Interfaces;
using ToDo_Backend_CA_EnterpriseLayer;
using ToDo_Backend_CA_IntefaceAdapters_Presenters.Views;

namespace ToDo_Backend_CA_IntefaceAdapters_Presenters.Presenters
{
    public class UserPresenter : IPresenter<UserEntity, UserViewModel>
    {
        public IEnumerable<UserViewModel> Present(IEnumerable<UserEntity> users)
            => users.Select(u => new UserViewModel
            {
                FullName = u.FirstName + u.LastName,
                IdentificationNumer = u.IdentificationNumber,
                Location = u.Country + ", " + u.City,
                PhoneNumber = u.Phone,
                Email = u.Email,
                CreatedTasks = users.Count() == 1 ? $"Has creado {u.CreatedTasks} tareas" : $"Ha creado {u.CreatedTasks} tareas",
                CompletedTasks = users.Count() == 1 ? $"Has completado {u.CompletedTasks} tareas" : $"Ha completado {u.CompletedTasks} tareas",
                PublishedTasks = users.Count() == 1 ? $"Has publicado {u.PublishedTasks} tareas" : $"Ha publicado {u.PublishedTasks} tareas"
            });
    }
}
