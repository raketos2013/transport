using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;


namespace FileManager.DAL.Repositories.Implementations
{
    public class AddresseeRepository(AppDbContext appDbContext) : IAddresseeRepository
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
            return appDbContext.Addressee.FirstOrDefault(x => x.PersonalNumber == id);
        }

        public AddresseeGroupEntity? GetAddresseeGroupById(int id)
        {
            return appDbContext.AddresseeGroup.FirstOrDefault(x => x.Id == id);
        }

        public List<AddresseeGroupEntity> GetAllAddresseeGroups()
        {
            return appDbContext.AddresseeGroup.ToList();
        }

        public List<AddresseeEntity> GetAllAddressees()
        {
            return appDbContext.Addressee.ToList();
        }
    }
}
