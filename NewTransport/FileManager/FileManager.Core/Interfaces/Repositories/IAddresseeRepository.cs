using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface IAddresseeRepository
{
    Task<List<AddresseeGroupEntity>> GetAllAddresseeGroups();
    Task<AddresseeGroupEntity?> GetAddresseeGroupById(int id);
    Task<bool> CreateAddresseeGroup(AddresseeGroupEntity group);
    Task<bool> EditAddresseeGroup(AddresseeGroupEntity group);
    Task<bool> DeleteAddresseeGroup(int id);

    Task<List<AddresseeEntity>> GetAllAddressees();
    Task<AddresseeEntity?> GetAddresseeById(string id);
    Task<bool> CreateAddressee(AddresseeEntity addressee);
    Task<bool> EditAddressee(AddresseeEntity addressee);
    Task<bool> DeleteAddressee(string number, int idGroup);
}
