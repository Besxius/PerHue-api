using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Net.Http;

namespace PerHue.Infrastructure.Policies
{
	public static class RetryPolicies
	{
		/// <summary>
		/// Retry policy cho AI services với exponential backoff
		/// </summary>
		public static AsyncRetryPolicy CreateAiServiceRetryPolicy(ILogger logger, int maxRetryAttempts = 3)
		{
			return Policy
				.Handle<HttpRequestException>()
				.Or<TaskCanceledException>()
				.Or<TimeoutException>()
				.Or<InvalidOperationException>(ex => ex.Message.Contains("API") || ex.Message.Contains("timeout"))
				.WaitAndRetryAsync(
					retryCount: maxRetryAttempts,
					sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2s, 4s, 8s
					onRetry: (exception, timeSpan, retryCount, context) =>
					{
						logger.LogWarning(
							exception,
							"[Virtual Try-On Retry] Attempt {RetryCount} of {MaxRetries} after {Delay}s. Error: {ExceptionMessage}",
							retryCount,
							maxRetryAttempts,
							timeSpan.TotalSeconds,
							exception.Message
						);
					}
				);
		}

		/// <summary>
		/// Retry policy với generic return type
		/// </summary>
		public static AsyncRetryPolicy<T> CreateAiServiceRetryPolicy<T>(ILogger logger, int maxRetryAttempts = 3)
		{
			return Policy<T>
				.Handle<HttpRequestException>()
				.Or<TaskCanceledException>()
				.Or<TimeoutException>()
				.Or<InvalidOperationException>(ex => ex.Message.Contains("API") || ex.Message.Contains("timeout"))
				.WaitAndRetryAsync(
					retryCount: maxRetryAttempts,
					sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
					onRetry: (outcome, timeSpan, retryCount, context) =>
					{
						logger.LogWarning(
							outcome.Exception,
							"[Virtual Try-On Retry] Attempt {RetryCount} of {MaxRetries} after {Delay}s. Error: {ExceptionMessage}",
							retryCount,
							maxRetryAttempts,
							timeSpan.TotalSeconds,
							outcome.Exception?.Message ?? "Unknown error"
						);
					}
				);
		}
	}
}