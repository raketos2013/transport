using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FileManager.Core.Services;

public class AddresseeService(IAddresseeRepository addresseeRepository,
                                IUserLogService userLogService,
                                IHttpContextAccessor httpContextAccessor)
            : IAddresseeService
{
    private static readonly JsonSerializerOptions _options = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };
    public bool CreateAddressee(AddresseeEntity addressee)
    {
        return addresseeRepository.CreateAddressee(addressee);
    }

    public bool CreateAddresseeGroup(AddresseeGroupEntity group)
    {
        return addresseeRepository.CreateAddresseeGroup(group);
    }

    public bool DeleteAddressee(string number, int idGroup)
    {
        return addresseeRepository.DeleteAddressee(number, idGroup);
    }

    public bool DeleteAddresseeGroup(int id)
    {
        return addresseeRepository.DeleteAddresseeGroup(id);
    }

    public bool EditAddressee(AddresseeEntity addressee)
    {
        return addresseeRepository.EditAddressee(addressee);
    }

    public bool EditAddresseeGroup(AddresseeGroupEntity group)
    {
        throw new NotImplementedException();
    }

    public AddresseeEntity? GetAddresseeById(string id)
    {
        return addresseeRepository.GetAddresseeById(id);
    }

    public AddresseeGroupEntity? GetAddresseeGroupById(int id)
    {
        return addresseeRepository.GetAddresseeGroupById(id);
    }

    public List<AddresseeGroupEntity> GetAllAddresseeGroups()
    {
        return addresseeRepository.GetAllAddresseeGroups();
    }

    public List<AddresseeEntity> GetAllAddressees()
    {
        return addresseeRepository.GetAllAddressees();
    }
}
