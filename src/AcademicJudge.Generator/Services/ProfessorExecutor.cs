using System;
using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using AcademicJudge.Generator.Models;

namespace AcademicJudge.Generator.Services;

public static class ProfessorExecutor
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static object Execute(TaskConfig config, object input)
    {
        if (string.IsNullOrWhiteSpace(config.ProfessorAssemblyPath))
        {
            throw new InvalidOperationException("Brak skonfigurowanej ścieżki do pliku assembly profesora (ProfessorAssemblyPath).");
        }

        if (string.IsNullOrWhiteSpace(config.ProfessorClassName))
        {
            throw new InvalidOperationException("Brak skonfigurowanej nazwy klasy profesora (ProfessorClassName).");
        }

        if (string.IsNullOrWhiteSpace(config.MethodName))
        {
            throw new InvalidOperationException("Brak nazwy metody w TaskConfig (MethodName).");
        }

        Assembly assembly = Assembly.LoadFrom(config.ProfessorAssemblyPath);
        Type profType = assembly.GetType(config.ProfessorClassName)
            ?? throw new TypeLoadException($"Nie znaleziono klasy wzorcowej '{config.ProfessorClassName}' w assembly '{config.ProfessorAssemblyPath}'.");

        MethodInfo method = profType.GetMethod(config.MethodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
            ?? throw new MissingMethodException($"Nie znaleziono publicznej metody '{config.MethodName}' w klasie '{config.ProfessorClassName}'.");

        object? instance = method.IsStatic ? null : Activator.CreateInstance(profType);

        ParameterInfo[] parameters = method.GetParameters();
        object?[] args = MapInputToParameters(input, parameters);

        try
        {
            object? result = method.Invoke(instance, args);
            return result ?? throw new InvalidOperationException($"Metoda wzorcowa '{config.MethodName}' zwróciła wartość null.");
        }
        catch (TargetInvocationException ex)
        {
            throw new InvalidOperationException(
                $"Kod profesora rzucił wyjątek podczas wykonywania dla podanego wejścia: {ex.InnerException?.Message}", 
                ex.InnerException);
        }
    }

    private static object?[] MapInputToParameters(object input, ParameterInfo[] parameters)
    {
        if (parameters.Length == 0)
        {
            return Array.Empty<object?>();
        }

        if (parameters.Length == 1)
        {
            Type targetType = parameters[0].ParameterType;
            return new[] { ConvertToType(input, targetType) };
        }

        if (input is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Array)
        {
            if (jsonElement.GetArrayLength() != parameters.Length)
            {
                throw new ArgumentException(
                    $"Metoda '{parameters[0].Member.Name}' oczekuje {parameters.Length} argumentów, ale w JSON przekazano {jsonElement.GetArrayLength()}.");
            }

            var args = new object?[parameters.Length];
            int index = 0;
            foreach (var element in jsonElement.EnumerateArray())
            {
                args[index] = ConvertToType(element, parameters[index].ParameterType);
                index++;
            }
            return args;
        }

        if (input is object[] rawArray)
        {
            if (rawArray.Length != parameters.Length)
            {
                throw new ArgumentException(
                    $"Metoda oczekuje {parameters.Length} argumentów, ale przekazano tablicę z {rawArray.Length} elementami.");
            }

            var args = new object?[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = ConvertToType(rawArray[i], parameters[i].ParameterType);
            }
            return args;
        }

        throw new InvalidOperationException(
            $"Nie można zmapować wejścia na {parameters.Length} parametrów oczekiwanych przez metodę '{parameters[0].Member.Name}'.");
    }

private static object? ConvertToType(object? value, Type targetType)
    {
        if (value is null)
        {
            return targetType.IsValueType && Nullable.GetUnderlyingType(targetType) == null
                ? throw new InvalidCastException($"Nie można przypisać wartości null do typu wartościowego {targetType.Name}.")
                : null;
        }

        Type valueType = value.GetType();

        if (targetType.IsAssignableFrom(valueType))
        {
            return value;
        }

        if (value is JsonNode jsonNode)
        {
            return jsonNode.Deserialize(targetType, JsonOptions);
        }

        if (value is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.String)
            {
                string strVal = element.GetString() ?? string.Empty;
                return ConvertStringToType(strVal, targetType);
            }

            if (targetType.IsArray && element.ValueKind != JsonValueKind.Array)
            {
                Type elementType = targetType.GetElementType() ?? typeof(object);
                object? singleItem = ConvertToType(element, elementType);
                Array arr = Array.CreateInstance(elementType, 1);
                arr.SetValue(singleItem, 0);
                return arr;
            }

            return element.Deserialize(targetType, JsonOptions);
        }

        if (value is string str)
        {
            return ConvertStringToType(str, targetType);
        }

        if (value is IEnumerable enumerable && targetType.IsArray)
        {
            Type elementType = targetType.GetElementType() ?? typeof(object);
            var list = new List<object?>();
            foreach (var item in enumerable)
            {
                list.Add(ConvertToType(item, elementType));
            }

            Array array = Array.CreateInstance(elementType, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                array.SetValue(list[i], i);
            }
            return array;
        }

        try
        {
            var node = JsonSerializer.SerializeToNode(value, JsonOptions);
            return node?.Deserialize(targetType, JsonOptions);
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Nie można przekonwertować wartości typu '{valueType.Name}' na typ '{targetType.Name}'.", ex);
        }
    }

    private static object? ConvertStringToType(string str, Type targetType)
    {
        if (targetType == typeof(string)) return str;

        string trimmed = str.Trim();

        if ((trimmed.StartsWith("[") && trimmed.EndsWith("]")) || (trimmed.StartsWith("{") && trimmed.EndsWith("}")))
        {
            try
            {
                return JsonSerializer.Deserialize(trimmed, targetType, JsonOptions);
            }
            catch
            {
                // W razie niepowodzenia przechodzi do tradycyjnej konwersji
            }
        }

        try
        {
            return Convert.ChangeType(str, targetType);
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Nie można przekonwertować ciągu '{str}' na typ '{targetType.Name}'.", ex);
        }
    }
}