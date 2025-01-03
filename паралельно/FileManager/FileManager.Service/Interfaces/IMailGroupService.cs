using FileManager.Domain.Entity;
using FileManager.Domain.Response;
using FileManager.Domain.ViewModels.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Service.Interfaces
{
    public interface IMailGroupService
    {
        Task<IBaseResponse<MailGroups>> Create(MailGroups groupModel);

        IQueryable<MailGroups> GetAll();

        Task<IBaseResponse<MailGroups>> Delete(string idGroup);

        Task<IBaseResponse<MailGroups>> Update(MailGroups groupModel);

    }
}
