namespace AcademicJudge.Tests;

public class StudentSolution
{
    public int Solve(int[] input)
    {
        int sum = 0;
        foreach (var x in input)
        {
            sum += x;
        }
        return sum;
    }
}