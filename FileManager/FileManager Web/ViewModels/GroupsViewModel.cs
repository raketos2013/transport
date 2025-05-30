using FileManager.Domain.Entity;

namespace FileManager_Web.ViewModels;

public class GroupsViewModel
{
    public List<TaskGroupEntity> TaskGroups { get; set; }
    public List<AddresseeGroupEntity> AddresseeGroups { get; set; }
}
