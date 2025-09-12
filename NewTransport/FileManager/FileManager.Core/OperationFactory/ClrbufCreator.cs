using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Operations;
using FileManager.Core.Interfaces.Services;
using FileManager.Core.Operations;
using FileManager.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FileManager.Core.OperationFactory;

public class ClrbufCreator : CreatorFactoryMethod
{
    public override IStepOperation FactoryMethod(
                                                    TaskStepEntity step,
                                                    TaskOperation? operation,
                                                    //ITaskLogger taskLogger,
                                                    //IMailSender mailsender,
                                                    //IOptions<AuthTokenConfiguration> authTokenConfigurations,
                                                    //IOperationService operationService,
                                                    //IAddresseeService addresseeService,
                                                    //ITaskLogService taskLogService,
                                                    //IHttpClientFactory httpClientFactory,
                                                    IServiceScopeFactory scopeFactory)
    {
        return new Clrbuf(step, operation, 
                            //taskLogger, mailsender, authTokenConfigurations, 
                            //operationService, addresseeService, taskLogService, httpClientFactory
                            scopeFactory);
    }
}
