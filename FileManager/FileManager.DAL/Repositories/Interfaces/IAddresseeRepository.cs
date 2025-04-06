using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
