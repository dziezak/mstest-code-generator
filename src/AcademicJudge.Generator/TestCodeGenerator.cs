using System;
using System.IO;
using System.Text.Json;
using AcademicJudge.Generator.Models;
using AcademicJudge.Generator.Services;

namespace AcademicJudge.Generator;

public class TestCodeGenerator
{
    private readonly CodeEmitter _codeEmitter = new();

    public string GenerateFromConfigJson(string jsonContent)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var config = JsonSerializer.Deserialize<TaskConfig>(jsonContent, options)
                     ?? throw new InvalidOperationException("Couldn't deserialize JSON file with current configuration");

        return _codeEmitter.Emit(config);
    }

    public void GenerateToFile(string configJsonFilePath, string outputCSharpFilePath)
    {
        if (!File.Exists(configJsonFilePath))
        {
            throw new FileNotFoundException($"Configuration file does not exist: {configJsonFilePath}");
        }

        string jsonContent = File.ReadAllText(configJsonFilePath);
        string generatedCode = GenerateFromConfigJson(jsonContent);

        string? directory = Path.GetDirectoryName(outputCSharpFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(outputCSharpFilePath, generatedCode);
    }
}