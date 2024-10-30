using System.Security.Claims;
using Application.Common.Operation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
// For IAuthorizationService

// For ClaimsPrincipal
namespace Web.Extensions
{
	public static class OperationResultExtensions
	{
		private static  ILogger? _logger;

		// You can pass the logger instance through a method if you're instantiating this in a service context
		public static void SetLogger(ILogger logger)
		{
			_logger = logger;
		}
		
		public static ActionResultBuilder ToActionResultBuilder(
			this OperationResult result, 
			IAuthorizationService authorizationService, 
			ClaimsPrincipal user)
		{
			return new ActionResultBuilder(result, authorizationService, user);
		}
		

		public static ActionResultBuilder<T> ToActionResultBuilder<T>(
			this OperationResult<T> genericResult, 
			IAuthorizationService authorizationService, 
			ClaimsPrincipal user)
		{
			return new ActionResultBuilder<T>(genericResult, authorizationService, user);
		}
		

		public static IActionResult ToActionResult(this OperationResult result)
		{
			if (result.IsSuccess)
			{
				return new NoContentResult();
			}

			// Log the error before returning the response
			LogError(result);

			return result.ErrorType switch
			{
				ErrorType.BadRequest => new BadRequestObjectResult(new { Errors = result.ErrorMessage }),
				ErrorType.NotFound => new NotFoundObjectResult(new { Errors = result.ErrorMessage }),
				ErrorType.Unauthorized => new UnauthorizedResult(),
				ErrorType.Forbidden => new ForbidResult(),
				_ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
			};
		}

		public static IActionResult ToActionResult<T>(this OperationResult<T> result)
		{
			if (result.IsSuccess)
			{
				return result.SuccessType switch
				{
					SuccessTypes.Created => new CreatedResult("", result.Value), // 201 Created
					SuccessTypes.Updated => new OkObjectResult(result.Value),    // 200 OK
					SuccessTypes.Deleted => new NoContentResult(),              // 204 No Content
					_ => new OkObjectResult(result.Value)                       // 200 OK (default for other success types)
				}; 
			}

			// Log the error before returning the response
			LogError(result);

			return result.ErrorType switch
			{
				ErrorType.BadRequest => new BadRequestObjectResult(new { Errors = result.ErrorMessage }),
				ErrorType.NotFound => new NotFoundObjectResult(new { Errors = result.ErrorMessage }),
				ErrorType.Unauthorized => new UnauthorizedResult(),
				ErrorType.Forbidden => new ForbidResult(),
				_ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
			};
		}

		private static void LogError(OperationResult result)
		{
			// Ensure that the logger is initialized
			if (_logger != null && !result.IsSuccess)
			{
				_logger.LogError("Operation failed: {ErrorType}, {ErrorMessage}", result.ErrorType, result.ErrorMessage);
			}
		}

		private static void LogError<T>(OperationResult<T> result)
		{
			// Ensure that the logger is initialized
			if (_logger != null && !result.IsSuccess)
			{
				_logger.LogError("Operation failed: {ErrorType}, {ErrorMessage}", result.ErrorType, result.ErrorMessage);
			}
		}
	}

}
