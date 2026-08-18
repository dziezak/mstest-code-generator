using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcademicJudge.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class PointsAttribute(int points): TestPropertyAttribute("Points", points.ToString())
{
    public int Value { get; } = points;
}