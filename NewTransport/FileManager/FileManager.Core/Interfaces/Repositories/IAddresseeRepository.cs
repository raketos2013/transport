using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface IAddresseeRepository
{
    Task<List<AddresseeGroupEntity>> GetAllAddresseeGroups();
    Task<AddresseeGroupEntity?> GetAddresseeGroupById(int id);
    Task<AddresseeGroupEntity> CreateAddresseeGroup(AddresseeGroupEntity group);
    Task<bool> DeleteAddresseeGroup(int id);

    Task<List<AddresseeEntity>> GetAllAddressees();
    Task<AddresseeEntity?> GetAddresseeById(string id);
    Task<AddresseeEntity> CreateAddressee(AddresseeEntity addressee);
    AddresseeEntity EditAddressee(AddresseeEntity addressee);
    Task<bool> DeleteAddressee(string number, int idGroup);
}
