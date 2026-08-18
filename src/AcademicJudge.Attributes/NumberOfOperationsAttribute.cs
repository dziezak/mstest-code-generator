using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcademicJudge.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class NumberOfOperationsAttribute(long maxOperations) : TestPropertyAttribute("NumberOfOperations", maxOperations.ToString())
{
    public long MaxOperations { get; } = maxOperations;
}