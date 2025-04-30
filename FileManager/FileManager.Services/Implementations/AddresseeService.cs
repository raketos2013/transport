using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;


namespace FileManager.Services.Implementations
{
    public class AddresseeService(IAddresseeRepository addresseeRepository) 
                : IAddresseeService
    {
        public bool CreateAddressee(AddresseeEntity addressee)
        {
            throw new NotImplementedException();
        }

        public bool CreateAddresseeGroup(AddresseeGroupEntity group)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAddressee(int id)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAddresseeGroup(int id)
        {
            throw new NotImplementedException();
        }

        public bool EditAddressee(AddresseeEntity addressee)
        {
            throw new NotImplementedException();
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
}
