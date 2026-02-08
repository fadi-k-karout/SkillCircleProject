using static Application.Common.Operation.ErrorType;

namespace Application.Common.Operation
{
	public class OperationResult
	{


		public bool IsSuccess { get; set; }
		public string ErrorMessage { get; set; }
		public ErrorType? ErrorType { get; set; } 
		public Dictionary<string, string[]> ValidationErrors { get; set; }
		public SuccessTypes? SuccessType { get; set; }


		public static OperationResult Success(SuccessTypes? successType = null) => new OperationResult { IsSuccess = true, SuccessType = successType };
     
		public static OperationResult Failure(string errorMessage, ErrorType errorType = InternalServerError)
		{
			return new OperationResult { IsSuccess = false, ErrorMessage = errorMessage, ErrorType = errorType };
		}

		public static OperationResult ResourceNotFound(string resourceName, object identifier)
		{
			string errorMessage = $"{resourceName} with ID {identifier} was not found.";

			return Failure(errorMessage, NotFound);
		}
		

		public static OperationResult ArgumentNullError(string argumentName)
		{
			string errorMessage = $"{argumentName} data is missing";
			return Failure(errorMessage, BadRequest);
		}
	}

	public class OperationResult<T> : OperationResult
	{
		public T Value { get; set; }
		public SuccessTypes? SuccessType { get; set; }

		public static OperationResult<T> Success(T value, SuccessTypes? successType = SuccessTypes.Success) =>
			new OperationResult<T> { IsSuccess = true, Value = value, SuccessType = successType };

		public static OperationResult<T> Failure(string errorMessage, ErrorType errorType = InternalServerError)
		{
			return new OperationResult<T> { IsSuccess = false, ErrorMessage = errorMessage, ErrorType = errorType };
		}

		public static OperationResult<T> ResourceNotFound(string resourceName, object identifier)
		{
			string errorMessage = $"{resourceName} with ID {identifier} was not found.";
			return Failure(errorMessage, NotFound);
		}

		public static OperationResult<T> ArgumentNullError(string resourceName)
		{
			string errorMessage = $"{resourceName} data is missing";
			return Failure(errorMessage, BadRequest);
		}

		public static OperationResult<T> AuthenticationError(string? errorMessage = null)
		{
			string authenticationErrorMessage = errorMessage ?? "Invalid login attempt.";
			return Failure(authenticationErrorMessage, Unauthorized);
		}
	}

}
