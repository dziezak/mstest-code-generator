using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace AcademicJudge.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class AcademicTestMethodAttribute : TestMethodAttribute
{
    public bool passed { get; set; } = false;
    public int TimeLimitMs {get; set; } = 1000;
    public long MaxOperations { get; set; } = -1; // no limit
    public bool IsHidden { get; set; } = false;

    public AcademicTestMethodAttribute(
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        : base(filePath, lineNumber)
    {}

    public override async Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
    {
        //here we shall test the limits (time and number of operations)
        return await base.ExecuteAsync(testMethod);
    }

}