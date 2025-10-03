using FileManager.Core.Entities;
using FileManager.Core.Exceptions;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Core.Interfaces.Services;

namespace FileManager.Core.Services;

public class AddresseeService(IUnitOfWork unitOfWork)
            : IAddresseeService
{
    public async Task<AddresseeEntity> CreateAddressee(AddresseeEntity addressee)
    {
        var createdAddressee = await unitOfWork.AddresseeRepository.CreateAddressee(addressee);
        return await unitOfWork.SaveAsync() > 0 ? createdAddressee 
            : throw new DomainException("Ошибка добавления адресата");
    }

    public async Task<AddresseeGroupEntity> CreateAddresseeGroup(AddresseeGroupEntity group)
    {
        var createdGroup = await unitOfWork.AddresseeRepository.CreateAddresseeGroup(group);
        return await unitOfWork.SaveAsync() > 0 ? createdGroup
            : throw new DomainException("Ошибка создания группы рассылки");
    }

    public async Task<bool> DeleteAddressee(string number, int idGroup)
    {
        var result = await unitOfWork.AddresseeRepository.DeleteAddressee(number, idGroup);
        return result && await unitOfWork.SaveAsync() > 0;
    }

    public async Task<bool> DeleteAddresseeGroup(int id)
    {
        var result = await unitOfWork.AddresseeRepository.DeleteAddresseeGroup(id);
        return result && await unitOfWork.SaveAsync() > 0;
    }

    public async Task<AddresseeEntity> EditAddressee(AddresseeEntity addressee)
    {
        var editedAddressee = unitOfWork.AddresseeRepository.EditAddressee(addressee);
        return await unitOfWork.SaveAsync() > 0 ? editedAddressee
            : throw new DomainException("Ошибка при обновлении адресата");
    }

    public async Task<AddresseeEntity?> GetAddresseeById(string id)
    {
        return await unitOfWork.AddresseeRepository.GetAddresseeById(id);
    }

    public async Task<AddresseeGroupEntity?> GetAddresseeGroupById(int id)
    {
        return await unitOfWork.AddresseeRepository.GetAddresseeGroupById(id);
    }

    public async Task<List<AddresseeGroupEntity>> GetAllAddresseeGroups()
    {
        return await unitOfWork.AddresseeRepository.GetAllAddresseeGroups();
    }

    public async Task<List<AddresseeEntity>> GetAllAddressees()
    {
        return await unitOfWork.AddresseeRepository.GetAllAddressees();
    }
}
