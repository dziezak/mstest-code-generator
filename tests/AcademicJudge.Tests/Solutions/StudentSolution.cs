namespace AcademicJudge.Tests;

public class StudentSolution
{
    public int Solve(int[] input)
    {
        bool all_zeros = true;
        int sum = 0;
        foreach (var x in input)
        {
            if (x != 0) all_zeros = false;
            sum += x;
        }

        if (all_zeros)
        {
            return 0;
        }
        return sum;
    }

    public int Solve(int n)
    {
        if (n <= 1) return 1;
        return n * Solve(n - 1);
    }
}