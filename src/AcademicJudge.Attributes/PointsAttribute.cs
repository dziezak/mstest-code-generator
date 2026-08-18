using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcademicJudge.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class PointsAttribute: TestPropertyAttribute
{
    public int PointsValue { get; }

    public PointsAttribute(int points) : base("Points", points.ToString())
    {
        PointsValue = points;
    }
}