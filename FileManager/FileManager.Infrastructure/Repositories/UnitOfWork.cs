using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;

namespace FileManager.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private IAddresseeRepository? _addresseeRepository;
    private ILockRepository? _lockRepository;
    private IOperationRepository? _operationRepository;
    private IStepRepository? _stepRepository;
    private ITaskRepository? _taskRepository;
    private ITaskLogRepository? _taskLogRepository;
    private IUserLogRepository? _userLogRepository;

    public IAddresseeRepository AddresseeRepository
    {
        get { return _addresseeRepository ??= new AddresseeRepository(context); }
    }

    public ILockRepository LockRepository 
    { 
        get { return _lockRepository ??= new LockRepository(context); } 
    }

    public IOperationRepository OperationRepository
    {
        get { return _operationRepository ??= new OperationRepository(context); }
    }

    public IStepRepository StepRepository
    {
        get { return _stepRepository ??= new StepRepository(context); }
    }

    public ITaskRepository TaskRepository
    {
        get { return _taskRepository ??= new TaskRepository(context); }
    }

    public ITaskLogRepository TaskLogRepository
    {
        get { return _taskLogRepository ??= new TaskLogRepository(context); }
    }

    public IUserLogRepository UserLogRepository
    {
        get { return _userLogRepository ??= new UserLogRepository(context); }
    }

    public async Task<int> SaveAsync()
    {
        return await context.SaveChangesAsync();
    }

    private bool disposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
