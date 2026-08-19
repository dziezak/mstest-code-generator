using System.Collections;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace AcademicJudge.Generator.Models;

public class TaskConfig
{
    public string TaskName { get; set; } = "GeneratedTaskTest";
    public string TargetNamespace { get; set; } = "AcademicJudge.Tests";
    public string StudentClassName { get; set; } = "StudentSolution";
    public string MethodName { get; set; } = "Solve";
    public double TimeMultiplier { get; set; } = 1.5;
    
    public string? ProfessorAssemblyPath { get; set; }
    public string? ProfessorClassName { get; set; }
    public List<TestCaseConfig> TestCases { get; set; } = new();
}

public class TestCaseConfig
{
    public string Name { get; set; } = "Test_01";
    public JsonNode? Input { get; set; }
    public JsonNode? ExpectedOutput { get; set; }
    public int Points { get; set; } = 1;
    public bool IsHidden { get; set; } = false;
    public int? CustomTimeLimitMs { get; set; }
    public long? MaxOperations { get; set; }
}