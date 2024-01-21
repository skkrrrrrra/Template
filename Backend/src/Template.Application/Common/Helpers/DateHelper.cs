namespace Template.Application.Common.Helpers
{
    public class DateHelper
    {
        public static DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow.AddHours(3);
        }
    }
}
