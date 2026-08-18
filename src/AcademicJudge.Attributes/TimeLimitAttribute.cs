using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcademicJudge.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TimeLimitAttribute(int timeLimitMs) : TestPropertyAttribute("TimeLimit", timeLimitMs.ToString())
{
    public int TimeLimitMs { get; } = timeLimitMs;
    
}