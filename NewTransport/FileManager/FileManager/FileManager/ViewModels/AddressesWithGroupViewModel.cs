using FileManager.Core.Entities;

namespace FileManager.ViewModels;

public class AddressesWithGroupViewModel
{
    public AddresseeEntity? Addressee { get; set; }
    public AddresseeGroupEntity? AddresseeGroup { get; set; }
}
