using FileManager.Domain.Entity;

namespace FileManager.DAL.Repositories.Interfaces
{
    public interface IAddresseeRepository
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
        bool DeleteAddressee(int id);
    }
}
