using FileManager.Core.Entities;

namespace FileManager.ViewModels;

public class GroupsViewModel
{
    public List<TaskGroupEntity> TaskGroups { get; set; }
    public List<AddresseeGroupEntity> AddresseeGroups { get; set; }
}
