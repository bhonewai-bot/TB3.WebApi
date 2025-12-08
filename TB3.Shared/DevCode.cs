namespace TB3.Shared;

public static class DevCode
{
    public static int CalculateAge(DateTime birthDate)
    {
        DateTime now = DateTime.Today;
        TimeSpan ageDiff = now - birthDate;
        int age = (int)(ageDiff.TotalDays / 365);
        return age;
    }
}