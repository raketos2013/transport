namespace FileManager.Core.Interfaces.Operations;

public interface IStepOperation
{
    void SetNext(IStepOperation nextStep);
    Task Execute(List<string>? bufferFiles);
}
