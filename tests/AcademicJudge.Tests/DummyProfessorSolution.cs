namespace AcademicJudge.Tests;

public class DummyProfessorSolution
{
    public int Solve(int[] numbers)
    {
        if (numbers == null) return 0;
        int sum = 0;
        foreach (var number in numbers)
        {
            sum += number;
        }
        return sum;
    }
}

public class FactorialProfessorSolution
{
    public int Solve(int n)
    {
        if(n < 0 ) throw new ArgumentOutOfRangeException(nameof(n));
        int result = 1;
        for (int i = 1; i <= n; i++)
        {
            result *= i;
        }

        return result;
    }
}