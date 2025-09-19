using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileManager.Core.Services;

public class AddresseeService(IUnitOfWork unitOfWork,
                                IAddresseeRepository addresseeRepository,
                                IUserLogService userLogService,
                                IHttpContextAccessor httpContextAccessor)
            : IAddresseeService
{
    private static readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };
    public async Task<bool> CreateAddressee(AddresseeEntity addressee)
    {
        return await unitOfWork.AddresseeRepository.CreateAddressee(addressee);
    }

    public async Task<bool> CreateAddresseeGroup(AddresseeGroupEntity group)
    {
        return await unitOfWork.AddresseeRepository.CreateAddresseeGroup(group);
    }

    public async Task<bool> DeleteAddressee(string number, int idGroup)
    {
        return await addresseeRepository.DeleteAddressee(number, idGroup);
    }

    public async Task<bool> DeleteAddresseeGroup(int id)
    {
        return await addresseeRepository.DeleteAddresseeGroup(id);
    }

    public async Task<bool> EditAddressee(AddresseeEntity addressee)
    {
        return await addresseeRepository.EditAddressee(addressee);
    }

    public Task<bool> EditAddresseeGroup(AddresseeGroupEntity group)
    {
        throw new NotImplementedException();
    }

    public async Task<AddresseeEntity?> GetAddresseeById(string id)
    {
        return await addresseeRepository.GetAddresseeById(id);
    }

    public async Task<AddresseeGroupEntity?> GetAddresseeGroupById(int id)
    {
        return await addresseeRepository.GetAddresseeGroupById(id);
    }

    public async Task<List<AddresseeGroupEntity>> GetAllAddresseeGroups()
    {
        return await addresseeRepository.GetAllAddresseeGroups();
    }

    public async Task<List<AddresseeEntity>> GetAllAddressees()
    {
        return await addresseeRepository.GetAllAddressees();
    }
}
