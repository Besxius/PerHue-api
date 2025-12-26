using Microsoft.Extensions.Configuration;
using TimeZoneConverter;

namespace PerHue.Infrastructure.Utils
{
	public interface IDateTimeService
	{
		DateTime GetCurrentTime();
		DateTime ConvertToLocalTime(DateTime utcDateTime);
	}

	public class DateTimeService : IDateTimeService
	{
		private readonly TimeZoneInfo _timeZone;

		public DateTimeService(IConfiguration configuration)
		{
			string timeZoneId = configuration["Region:TimeZoneId"];

			if (string.IsNullOrEmpty(timeZoneId))
			{
				_timeZone = TimeZoneInfo.Utc;
			}
			else
			{
				try
				{
					_timeZone = TZConvert.GetTimeZoneInfo(timeZoneId);
				}
				catch
				{
					_timeZone = TimeZoneInfo.Utc;
				}
			}
		}

		public DateTime GetCurrentTime()
		{
			return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZone);
		}

		public DateTime ConvertToLocalTime(DateTime utcDateTime)
		{
			return TimeZoneInfo.ConvertTime(utcDateTime, _timeZone);
		}
	}
}
