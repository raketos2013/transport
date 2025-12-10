using FileManager.Core.Entities;
using FileManager.Core.Exceptions;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FileManager.Core.Services;

public class AddresseeService(IUnitOfWork unitOfWork,
                                IHttpClientFactory httpClientFactory)
            : IAddresseeService
{
    public async Task<AddresseeEntity> CreateAddressee(AddresseeEntity addressee)
    {
        var createdAddressee = await unitOfWork.AddresseeRepository.CreateAddressee(addressee);
        return await unitOfWork.SaveAsync() > 0 ? createdAddressee 
            : throw new DomainException("Ошибка добавления адресата");
    }

    public async Task<AddresseeGroupEntity> CreateAddresseeGroup(AddresseeGroupEntity group)
    {
        var createdGroup = await unitOfWork.AddresseeRepository.CreateAddresseeGroup(group);
        return await unitOfWork.SaveAsync() > 0 ? createdGroup
            : throw new DomainException("Ошибка создания группы рассылки");
    }

    public async Task<bool> DeleteAddressee(string number, int idGroup)
    {
        var result = await unitOfWork.AddresseeRepository.DeleteAddressee(number, idGroup);
        return result && await unitOfWork.SaveAsync() > 0;
    }

    public async Task<bool> DeleteAddresseeGroup(int id)
    {
        var result = await unitOfWork.AddresseeRepository.DeleteAddresseeGroup(id);
        return result && await unitOfWork.SaveAsync() > 0;
    }

    public async Task<AddresseeEntity> EditAddressee(AddresseeEntity addressee)
    {
        var editedAddressee = unitOfWork.AddresseeRepository.EditAddressee(addressee);
        return await unitOfWork.SaveAsync() > 0 ? editedAddressee
            : throw new DomainException("Ошибка при обновлении адресата");
    }

    public async Task<AddresseeEntity?> GetAddresseeById(string id)
    {
        return await unitOfWork.AddresseeRepository.GetAddresseeById(id);
    }

    public async Task<AddresseeGroupEntity?> GetAddresseeGroupById(int id)
    {
        return await unitOfWork.AddresseeRepository.GetAddresseeGroupById(id);
    }

    public async Task<List<AddresseeGroupEntity>> GetAllAddresseeGroups()
    {
        return await unitOfWork.AddresseeRepository.GetAllAddresseeGroups();
    }

    public async Task<List<AddresseGroupViewModel>> GetAllAddresseeGroupsWithName()
    {
        var groups = await unitOfWork.AddresseeRepository.GetAllAddresseeGroups();
        List<AddresseGroupViewModel> list = [];
        foreach (var group in groups)
        {
            list.Add(new AddresseGroupViewModel
            {
                Id = group.Id,
                NameWithId = string.Concat(group.Id, ' ', group.Name)
            });
        }
        return list;
    }

    public async Task<List<AddresseeEntity>> GetAllAddressees()
    {
        return await unitOfWork.AddresseeRepository.GetAllAddressees();
    }

    public async Task<List<AddresseeWithFioViewModel>> GetUniqueAddresseesWithFio()
    {
        var addresses = await unitOfWork.AddresseeRepository.GetAllAddressees();
        var addressesGroup = addresses.GroupBy(x => x.PersonalNumber).Select(c => c.First()).ToList();
        List<AddresseeWithFioViewModel> list = [];
        foreach (var address in addressesGroup)
        {
            list.Add(new AddresseeWithFioViewModel { 
                PersonalNumber = address.PersonalNumber,
                NumberWithFio = string.Concat(address.PersonalNumber, ' ', address.Fio)
            });
        }
        return list;
    }

    public async Task<SapUser> GetSapUser(string persNumber)
    {
        SapUser? sapUser = new();
        var client = httpClientFactory.CreateClient("SapPip");
        var contentType = new MediaTypeWithQualityHeaderValue("application/json");
        client.DefaultRequestHeaders.Accept.Add(contentType);
        var data = JsonSerializer.Serialize(new SapUserRequest() { Pernr = persNumber});
        var contentData = new StringContent(data, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("sap/bc/zcustepdev/userdata_transp", contentData);
        
        if (response != null)
        {
            if (response.IsSuccessStatusCode)
            {
                if (response.Content != null)
                {
                    sapUser = await response.Content.ReadFromJsonAsync<SapUser>();
                    
                }
            }
        }
        return sapUser;
    }
}
