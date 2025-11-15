namespace PerHue.Application.Models
{
	public class ServiceResponse
	{
		public int Code { get; set; } = 200;
		public object? Result { get; set; }
		public string Message { get; set; } = "Operation successful";
		public bool Success { get; set; } = true;

		public ServiceResponse()
		{
		}

		public ServiceResponse(object? result, string message = "Operation successful", int code = 200, bool success = true)
		{
			Code = code;
			Result = result;
			Message = message;
			Success = success;
		}

		// Static factory methods for common responses
		public static ServiceResponse Ok(object? result = null, string message = "Operation successful")
		{
			return new ServiceResponse(result, message, 200, true);
		}

		public static ServiceResponse Error(string message = "An error occurred", int code = 500, object? result = null)
		{
			return new ServiceResponse(result, message, code, false);
		}

		public static ServiceResponse NotFound(string message = "Resource not found", object? result = null)
		{
			return new ServiceResponse(result, message, 404, false);
		}

		public static ServiceResponse BadRequest(string message = "Bad request", object? result = null)
		{
			return new ServiceResponse(result, message, 400, false);
		}

		public static ServiceResponse Unauthorized(string message = "Unauthorized access", object? result = null)
		{
			return new ServiceResponse(result, message, 401, false);
		}

		public static ServiceResponse Forbidden(string message = "Forbidden access", object? result = null)
		{
			return new ServiceResponse(result, message, 403, false);
		}
	}

	public class ServiceResponse<T>
	{
		public int Code { get; set; } = 200;
		public T? Result { get; set; }
		public string Message { get; set; } = "Operation successful";
		public bool Success { get; set; } = true;

		public ServiceResponse()
		{
		}

		public ServiceResponse(T? result, string message = "Operation successful", int code = 200, bool success = true)
		{
			Code = code;
			Result = result;
			Message = message;
			Success = success;
		}

		// Static factory methods for common responses
		public static ServiceResponse<T> Ok(T? result = default, string message = "Operation successful")
		{
			return new ServiceResponse<T>(result, message, 200, true);
		}

		public static ServiceResponse<T> Error(string message = "An error occurred", int code = 500, T? result = default)
		{
			return new ServiceResponse<T>(result, message, code, false);
		}

		public static ServiceResponse<T> NotFound(string message = "Resource not found", T? result = default)
		{
			return new ServiceResponse<T>(result, message, 404, false);
		}

		public static ServiceResponse<T> BadRequest(string message = "Bad request", T? result = default)
		{
			return new ServiceResponse<T>(result, message, 400, false);
		}

		public static ServiceResponse<T> Unauthorized(string message = "Unauthorized access", T? result = default)
		{
			return new ServiceResponse<T>(result, message, 401, false);
		}

		public static ServiceResponse<T> Forbidden(string message = "Forbidden access", T? result = default)
		{
			return new ServiceResponse<T>(result, message, 403, false);
		}

		// Conversion from non-generic to generic
		public static implicit operator ServiceResponse<T>(ServiceResponse response)
		{
			return new ServiceResponse<T>
			{
				Code = response.Code,
				Result = response.Result is T result ? result : default,
				Message = response.Message,
				Success = response.Success
			};
		}
	}
}