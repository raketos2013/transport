using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services.Implementations
{
    public class AddresseeService : IAddresseeService
    {
        private readonly IAddresseeRepository _addresseeRepository;
        public AddresseeService(IAddresseeRepository addresseeRepository)
        {
            _addresseeRepository = addresseeRepository;
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
            return _addresseeRepository.GetAddresseeById(id);
        }

        public AddresseeGroupEntity? GetAddresseeGroupById(int id)
        {
            return _addresseeRepository.GetAddresseeGroupById(id);
        }

        public List<AddresseeGroupEntity> GetAllAddresseeGroups()
        {
            return _addresseeRepository.GetAllAddresseeGroups();
        }

        public List<AddresseeEntity> GetAllAddressees()
        {
            return _addresseeRepository.GetAllAddressees();
        }
    }
}
