using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Infrastructure.Repositories;

public class AddresseeRepository(AppDbContext appDbContext) : IAddresseeRepository
{
    public async Task<AddresseeEntity> CreateAddressee(AddresseeEntity addressee)
    {
        await appDbContext.Addressee.AddAsync(addressee);
        return addressee;
    }

    public async Task<AddresseeGroupEntity> CreateAddresseeGroup(AddresseeGroupEntity group)
    {
        await appDbContext.AddresseeGroup.AddAsync(group);
        return group;
    }

    public async Task<bool> DeleteAddressee(string number, int idGroup)
    {
        try
        {
            var addressee = await appDbContext.Addressee.FirstOrDefaultAsync(x => x.PersonalNumber == number &&
                                                                                  x.AddresseeGroupId == idGroup);
            if (addressee != null)
            {
                appDbContext.Addressee.Remove(addressee);
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAddresseeGroup(int id)
    {
        try
        {
            var deletedGroup = await GetAddresseeGroupById(id);
            if (deletedGroup == null)
            {
                return false;
            }
            appDbContext.AddresseeGroup.Remove(deletedGroup);
            var deletedAddresses = await appDbContext.Addressee.Where(x => x.AddresseeGroupId == id).ToListAsync();
            appDbContext.Addressee.RemoveRange(deletedAddresses);
            var tasks = await appDbContext.Task.Where(x => x.AddresseeGroupId == id).ToListAsync();
            foreach (TaskEntity task in tasks)
            {
                task.AddresseeGroupId = null;
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public AddresseeEntity EditAddressee(AddresseeEntity addressee)
    {
        appDbContext.Addressee.Update(addressee);
        return addressee;
    }

    public async Task<AddresseeEntity?> GetAddresseeById(string id)
    {
        return await appDbContext.Addressee.FirstOrDefaultAsync(x => x.PersonalNumber == id);
    }

    public async Task<AddresseeGroupEntity?> GetAddresseeGroupById(int id)
    {
        return await appDbContext.AddresseeGroup.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<AddresseeGroupEntity>> GetAllAddresseeGroups()
    {
        return await appDbContext.AddresseeGroup.ToListAsync();
    }

    public async Task<List<AddresseeEntity>> GetAllAddressees()
    {
        return await appDbContext.Addressee.ToListAsync();
    }
}
