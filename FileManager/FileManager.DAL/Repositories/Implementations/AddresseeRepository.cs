using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.DAL.Repositories.Implementations
{
    public class AddresseeRepository : IAddresseeRepository
    {
        private readonly AppDbContext _appDbContext;
        public AddresseeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

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
            return _appDbContext.Addressee.FirstOrDefault(x => x.PersonalNumber == id); 
        }

        public AddresseeGroupEntity? GetAddresseeGroupById(int id)
        {
            return _appDbContext.AddresseeGroup.FirstOrDefault(x => x.Id == id);
        }

        public List<AddresseeGroupEntity> GetAllAddresseeGroups()
        {
            return _appDbContext.AddresseeGroup.ToList();
        }

        public List<AddresseeEntity> GetAllAddressees()
        {
            return _appDbContext.Addressee.ToList();
        }
    }
}
