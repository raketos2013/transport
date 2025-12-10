using FileManager.Core.Entities;
using FileManager.Core.ViewModels;

namespace FileManager.Core.Interfaces.Services;

public interface IAddresseeService
{
    Task<List<AddresseeGroupEntity>> GetAllAddresseeGroups();
    Task<List<AddresseGroupViewModel>> GetAllAddresseeGroupsWithName();
    Task<AddresseeGroupEntity?> GetAddresseeGroupById(int id);
    Task<AddresseeGroupEntity> CreateAddresseeGroup(AddresseeGroupEntity group);
    Task<bool> DeleteAddresseeGroup(int id);
    Task<SapUser> GetSapUser(string persNumber);

    Task<List<AddresseeEntity>> GetAllAddressees();
    Task<List<AddresseeWithFioViewModel>> GetUniqueAddresseesWithFio();
    Task<AddresseeEntity?> GetAddresseeById(string id);
    Task<AddresseeEntity> CreateAddressee(AddresseeEntity addressee);
    Task<AddresseeEntity> EditAddressee(AddresseeEntity addressee);
    Task<bool> DeleteAddressee(string number, int idGroup);
}
