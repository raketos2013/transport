using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Operations;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FileManager.Core.OperationFactory;

public abstract class CreatorFactoryMethod
{
    public abstract IStepOperation FactoryMethod(
                                                    TaskStepEntity step,
                                                    TaskOperation? operation,
                                                    IServiceScope scopeFactory);
}
