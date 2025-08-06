using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface IAddresseeService
{
    List<AddresseeGroupEntity> GetAllAddresseeGroups();
    AddresseeGroupEntity? GetAddresseeGroupById(int id);
    bool CreateAddresseeGroup(AddresseeGroupEntity group);
    bool EditAddresseeGroup(AddresseeGroupEntity group);
    bool DeleteAddresseeGroup(int id);

    List<AddresseeEntity> GetAllAddressees();
    AddresseeEntity? GetAddresseeById(string id);
    bool CreateAddressee(AddresseeEntity addressee);
    bool EditAddressee(AddresseeEntity addressee);
    bool DeleteAddressee(string number, int idGroup);
}
