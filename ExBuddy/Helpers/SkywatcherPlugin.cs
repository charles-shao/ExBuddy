﻿namespace ExBuddy.Helpers
{
	using System;
	using System.Linq;

	using ExBuddy.Logging;
	using ExBuddy.Plugins.Skywatcher;

	using ff14bot.Managers;

	public static class SkywatcherPlugin
	{
		public static readonly DateTime EorzeaStartTime = new DateTime(2010, 7, 13);

		public static TimeSpan GetEorzeaTimeTillNextInterval()
		{
			var timeOfDay = WorldManager.EorzaTime.TimeOfDay;

			var secondsLeft = 60 - timeOfDay.Seconds;
			var minutesLeft = 60 - timeOfDay.Minutes + (secondsLeft == 0 ? 0 : -1);
			var hoursLeft = 8 - (timeOfDay.Hours % 8) + (minutesLeft == 0 && secondsLeft == 0 ? 0 : -1);

			var timeleft = new TimeSpan(hoursLeft, minutesLeft, secondsLeft);

			return timeleft;
		}

		public static int GetIntervalNumber()
		{
			var interval = ((DateTime.UtcNow.ToUniversalTime().AddHours(8) - EorzeaStartTime).TotalSeconds / 1400);

			return Convert.ToInt32(interval);
		}

		public static double GetTimeTillNextInterval()
		{
			var timeOfDay = WorldManager.EorzaTime.TimeOfDay;

			var secondsLeft = 60 - timeOfDay.Seconds;
			var minutesLeft = 60 - timeOfDay.Minutes + (secondsLeft == 0 ? 0 : -1);
			var hoursLeft = 8 - (timeOfDay.Hours % 8) + (minutesLeft == 0 && secondsLeft == 0 ? 0 : -1);

			var timeLeft = (secondsLeft * 1000 + minutesLeft * 60 * 1000 + hoursLeft * 3600 * 1000) * (7.0 / 144.0);
			return timeLeft;
		}

		public static string GetWeatherNameById(byte weatherId)
		{
			string weatherName;
			WorldManager.WeatherDictionary.TryGetValue(weatherId, out weatherName);
			return weatherName;
		}

		public static bool IsWeather(byte weatherId)
		{
			return weatherId == WorldManager.CurrentWeatherId;
		}

		public static bool IsWeather(string weatherName)
		{
			return string.Equals(weatherName, WorldManager.CurrentWeather, StringComparison.InvariantCultureIgnoreCase);
		}

		private static bool CheckEnabled()
		{
			if (!Skywatcher.IsEnabled)
			{
				Logger.Instance.Error("Skywatcher is not enabled in plugins.  Enable it and restart the bot.");
				ff14bot.TreeRoot.Stop();

				return false;
			}

			return true;
		}

		public static bool IsWeatherInZone(int zoneId, params byte[] weatherIds)
		{
			if (!CheckEnabled())
			{
				return false;
			}

			var currentWeatherId = Skywatcher.WeatherProvider.GetCurrentWeatherByZone(zoneId);

			return weatherIds.Any(wid => wid == currentWeatherId);
		}

		public static bool IsWeatherInZone(int zoneId, params string[] weatherNames)
		{
			if (!CheckEnabled())
			{
				return false;
			}

			var weatherId = Skywatcher.WeatherProvider.GetCurrentWeatherByZone(zoneId);

			if (!weatherId.HasValue)
			{
				return false;
			}

			string weatherName;
			if (!WorldManager.WeatherDictionary.TryGetValue((byte)weatherId, out weatherName))
			{
				return false;
			}

			return weatherNames.Any(wn => string.Equals(wn, weatherName, StringComparison.InvariantCultureIgnoreCase));
		}

		public static bool PredictWeatherInZone(int zoneId, TimeSpan timeSpan, params byte[] weatherIds)
		{
			if (!CheckEnabled())
			{
				return false;
			}

			var weatherId = Skywatcher.WeatherProvider.GetForecastByZone(zoneId, timeSpan);

			return weatherIds.Any(wid => wid == weatherId);
		}

		public static bool PredictWeatherInZone(int zoneId, TimeSpan timeSpan, params string[] weatherNames)
		{
			if (!CheckEnabled())
			{
				return false;
			}

			var weatherId = Skywatcher.WeatherProvider.GetForecastByZone(zoneId, timeSpan);

			if (!weatherId.HasValue)
			{
				return false;
			}

			string weatherName;
			if (!WorldManager.WeatherDictionary.TryGetValue((byte)weatherId, out weatherName))
			{
				return false;
			}

			return weatherNames.Any(wn => string.Equals(wn, weatherName, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}