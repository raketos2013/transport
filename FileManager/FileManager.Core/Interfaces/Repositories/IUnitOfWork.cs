namespace FileManager.Core.Interfaces.Repositories;

public interface IUnitOfWork
{
    IAddresseeRepository AddresseeRepository { get; }
    ILockRepository LockRepository { get; }
    IOperationRepository OperationRepository { get; }
    IStepRepository StepRepository { get; }
    ITaskRepository TaskRepository { get; }
    ITaskLogRepository TaskLogRepository { get; }
    IUserLogRepository UserLogRepository { get; }
    Task<int> SaveAsync();
}
