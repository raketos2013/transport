﻿namespace FileManager.Domain.ViewModels.Step
{
    public class CopyStepViewModel
    {
        public bool IsCopy { get; set; }
        public int StepNumber { get; set; }
        public string? Description { get; set; }
        public bool IsCopyOperation { get; set; }
    }
}
