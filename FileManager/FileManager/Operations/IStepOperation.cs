﻿namespace FileManagerServer.Operations;

public interface IStepOperation
{
    void SetNext(IStepOperation nextStep);
    void Execute(List<string>? bufferFiles);
}
