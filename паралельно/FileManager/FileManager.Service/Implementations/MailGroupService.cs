using FileManager.DAL.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Domain.Response;
using FileManager.Domain.ViewModels.Task;
using FileManager.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Service.Implementations
{
    public class MailGroupService: IMailGroupService
    {


        private readonly IBaseRepository<MailGroups> _mailGroupRepository;

        public MailGroupService(IBaseRepository<MailGroups> mailGroupRepository)
        {
            _mailGroupRepository = mailGroupRepository;

        }
        public async Task<IBaseResponse<MailGroups>> Create(MailGroups model)
        {
            try
            {
                await _mailGroupRepository.Create(model);

                return new BaseResponse<MailGroups>()
                {
                    Description = "Группа создана",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception)
            {
                return new BaseResponse<MailGroups>()
                {
                    Description = "Группа не создана",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }


        public async Task<IBaseResponse<MailGroups>> Update(MailGroups model)
        {
            try
            {
                await _mailGroupRepository.Update(model);

                return new BaseResponse<MailGroups>()
                {
                    Description = "Группа обновлена",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception)
            {
                return new BaseResponse<MailGroups>()
                {
                    Description = "Группа не обновлена",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }



        public IQueryable<MailGroups> GetAll()
        {
            return _mailGroupRepository.GetAll();
        }

        public async Task<IBaseResponse<MailGroups>> Delete(string idGroup)
        {
            try
            {
                await _mailGroupRepository.Delete(idGroup);

                return new BaseResponse<MailGroups>()
                {
                    Description = "Группа удалена",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception)
            {
                return new BaseResponse<MailGroups>()
                {
                    Description = "Группа не удалена",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }






    }
}
