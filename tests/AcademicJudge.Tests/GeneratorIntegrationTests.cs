using System.IO;
using System.Reflection;
using AcademicJudge.Generator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AcademicJudge.Tests;
// form OI2 tripple A (xD)
[TestClass]
public class GeneratorIntegrationTests
{
    [TestMethod]
    public void TestCodeGenerator_GeneratesValidCSharpFileFromConfig()
    {
        // Arrange
        var generator = new TestCodeGenerator();
        string configPath = Path.Combine(Directory.GetCurrentDirectory(), "task_config.json");
        
        string outputDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\GeneratedTests"));
        string outputPath = Path.Combine(outputDirectory, "GeneratedTask01Tests.g.cs");

        // Act
        generator.GenerateToFile(configPath, outputPath);

        // Assert
        Assert.IsTrue(File.Exists(outputPath), "Plik wygenerowanego kodu C# nie został utworzony.");
        string generatedContent = File.ReadAllText(outputPath);
        
        StringAssert.Contains(generatedContent, "[TestClass]");
        StringAssert.Contains(generatedContent, "public class GeneratedTask01Tests");
        StringAssert.Contains(generatedContent, "public void Test_SumArray_Basic()");
    }
    
    [TestMethod]
    public void TestCodeGenerator_GeneratesFactorialCSharpFileFromConfig()
    {
      // Arrange
      var generator = new TestCodeGenerator();
      string configPath = Path.Combine(Directory.GetCurrentDirectory(), "factorial_config.json");
    
      string outputDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\GeneratedTests"));
    
      string outputPath = Path.Combine(outputDirectory, "GeneratedFactorialTests.g.cs");

      // Act
      generator.GenerateToFile(configPath, outputPath);

      // Assert
      Assert.IsTrue(File.Exists(outputPath), "Plik wygenerowanego kodu C# nie został utworzony.");
      string generatedContent = File.ReadAllText(outputPath);
    
      StringAssert.Contains(generatedContent, "[TestClass]");
      StringAssert.Contains(generatedContent, "public class GeneratedFactorialTests");
      StringAssert.Contains(generatedContent, "public void Test_Factorial_Zero()");
    }


    [TestMethod]
    public void TestCodeGenerator_CalculatesExpectedOutputFromProfessorSolutionWhenNull()
    {
        var generator = new TestCodeGenerator();
        
        string currentAssemblyPath = Assembly.GetExecutingAssembly().Location;
        string escapedAssemblyPath = currentAssemblyPath.Replace("\\", "/");
        
        string jsonConfig = $$"""
        {
          "taskName": "GeneratedTaskProfessorTests",
          "targetNamespace": "AcademicJudge.Tests",
          "studentClassName": "StudentSolution",
          "methodName": "Solve",
          "professorAssemblyPath": "{{escapedAssemblyPath}}",
          "professorClassName": "AcademicJudge.Tests.DummyProfessorSolution",
          "testCases": [
            {
              "name": "Test_Explicit",
              "input": [50, 50],
              "expectedOutput": 100,
              "points": 2
            },
            {
              "name": "Test_FromProfessor",
              "input": [10, 20, 30],
              "expectedOutput": null,
              "points": 5
            }
          ]
        }
        """;
        
        string generatedCode = generator.GenerateFromConfigJson(jsonConfig);
        Assert.IsFalse(string.IsNullOrWhiteSpace(generatedCode));
        
        StringAssert.Contains(generatedCode, "public void Test_Explicit()");
        StringAssert.Contains(generatedCode, "var expectedOutput = 100;");
        StringAssert.Contains(generatedCode, "public void Test_FromProfessor()");
        StringAssert.Contains(generatedCode, "var expectedOutput = 60;");
    }

    [TestMethod]
    public void TestCodeGenerator_GeneratesFactorialTestsWithProfessor()
    {
        var generator = new TestCodeGenerator();
        string currentAssemblyPath = Assembly.GetExecutingAssembly().Location.Replace("\\", "/");

        string jsonConfig = $$"""
                               {
                               "taskName": "GeneratedFactorialTests",
                               "targetNamespace": "AcademicJudge.Tests",
                               "studentClassName": "StudentSolution",
                               "methodName": "Solve",
                               "professorAssemblyPath": "{{currentAssemblyPath}}",
                                 "professorClassName": "AcademicJudge.Tests.FactorialProfessorSolution",
                                 "testCases": [
                                   {
                                     "name": "Test_Factorial_Five",
                                     "input": 5,
                                     "expectedOutput": null,
                                     "points": 5
                                   }
                                 ]
                               }
                               """;
        string generatedCode = generator.GenerateFromConfigJson(jsonConfig);
        Assert.IsFalse(string.IsNullOrWhiteSpace(generatedCode));
        
        StringAssert.Contains(generatedCode, "public void Test_Factorial_Five()");
        StringAssert.Contains(generatedCode, "var expectedOutput = 120;");
    }
}