using System.IO;
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
}