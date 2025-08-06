using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Services;
using FileManager.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Infrastructure.Repositories;

public class AddresseeRepository(AppDbContext appDbContext) : IAddresseeRepository
{
    public bool CreateAddressee(AddresseeEntity addressee)
    {
        appDbContext.Addressee.Add(addressee);
        var result = appDbContext.SaveChanges();
        return result > 0;
    }

    public bool CreateAddresseeGroup(AddresseeGroupEntity group)
    {
        appDbContext.AddresseeGroup.Add(group);
        var result = appDbContext.SaveChanges();
        return result > 0;
    }

    public bool DeleteAddressee(string number, int idGroup)
    {
        try
        {
            var addressee = appDbContext.Addressee.FirstOrDefault(x => x.PersonalNumber == number && x.AddresseeGroupId == idGroup);
            if (addressee != null)
            {
                appDbContext.Addressee.Remove(addressee);
                appDbContext.SaveChanges();
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeleteAddresseeGroup(int id)
    {
        try
        {
            var deletedGroup = GetAddresseeGroupById(id);
            appDbContext.AddresseeGroup.Remove(deletedGroup);
            List<AddresseeEntity> deletedAddresses = appDbContext.Addressee.Where(x => x.AddresseeGroupId == id).ToList();
            appDbContext.Addressee.RemoveRange(deletedAddresses);
            List<TaskEntity> tasks = appDbContext.Task.Where(x => x.AddresseeGroupId == id).ToList();
            foreach (TaskEntity task in tasks)
            {
                task.AddresseeGroupId = null;
            }
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool EditAddressee(AddresseeEntity addressee)
    {
        try
        {
            appDbContext.Addressee.Update(addressee);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
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
