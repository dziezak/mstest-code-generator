using AcademicJudge.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcademicJudge.Tests;

[TestClass]
public class SampleAcademicTests
{
    [AcademicTestMethod(TimeLimitMs = 500, IsHidden = false)]
    [Points(5)]
    [NumberOfOperations(100_000)]
    public void Test_SimpleAddition_Passes()
    {
        int a = 2;
        int b = 3;
        int result = a + b;
        Assert.AreEqual(5, result);
    }

    [AcademicTestMethod(TimeLimitMs = 1500, IsHidden = false)]
    [Points(5)]
    [TimeLimit(1500)]
    public void Test_exemplary_Simulation_Passes()
    {
        int sum = 0;
        for (int i = 0; i < 1_000; i++)
        {
            sum += i;
        }
        Assert.IsTrue(sum > 0);
    }
    
}