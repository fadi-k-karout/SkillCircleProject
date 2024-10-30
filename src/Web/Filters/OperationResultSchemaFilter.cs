using Application.Common.Operation;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class OperationResultSchemaFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		// Check if the return type is a generic OperationResult<T>
		var returnType = context.MethodInfo.ReturnType;

		// Check for generic OperationResult<T>
		if (returnType.IsGenericType &&
			(returnType.GetGenericTypeDefinition() == typeof(Task<>) || returnType.GetGenericTypeDefinition() == typeof(ValueTask<>)))
		{
			var innerType = returnType.GetGenericArguments()[0]; // This should be OperationResult<T>

			// Check if the inner type is a generic OperationResult<T>
			if (innerType.IsGenericType && innerType.GetGenericTypeDefinition() == typeof(OperationResult<>))
			{
				var valueType = innerType.GetGenericArguments()[0]; // This is T

				// Generate the schema for the inner type T
				var innerSchema = context.SchemaGenerator.GenerateSchema(valueType, context.SchemaRepository);

				// Clear existing content types for all 2xx response codes and add application/json
				foreach (var key in operation.Responses.Keys.Where(key => key.StartsWith('2')).ToList())
				{
					operation.Responses[key].Content.Clear();
					operation.Responses[key].Content.Add("application/json", new OpenApiMediaType
					{
						Schema = innerSchema // Set schema to just the inner type
					});
				}
			}
		}
		// Check for non-generic OperationResult
		 if (returnType == typeof(Task<OperationResult>) || returnType == typeof(ValueTask<OperationResult>))
		{
			operation.Responses["204"].Content.Clear();
		}

		// Handle error responses (for both generic and non-generic)

		var validationErrorSchema = new OpenApiSchema
		{
			Type = "object",
			Properties = new Dictionary<string, OpenApiSchema>
			{
				["errors"] = new OpenApiSchema
				{
					Type = "object",
					AdditionalPropertiesAllowed = true, // Allow for dynamic property names
					AdditionalProperties = new OpenApiSchema
					{
						Type = "array",
						Items = new OpenApiSchema { Type = "string" } // Array of error messages for each field
					}
				}
			}
		};

			
	
			var errorSchema = new OpenApiSchema
			{
				Type = "object",
				Properties = new Dictionary<string, OpenApiSchema>
				{
					["errorMessage"] = new OpenApiSchema { Type = "string" }
				}
			};

			// Add schema for error responses
			// Clear existing content types for all 2xx response codes and add application/json
			foreach (var key in operation.Responses.Keys.Where(key => key.StartsWith("4")).ToList())
			{

				if (operation.Responses.ContainsKey("400"))
				{
					operation.Responses[key].Content.Clear();
					operation.Responses[key].Content.Add("application/json", new OpenApiMediaType
					{
						Schema = validationErrorSchema // Set schema to just the inner type
					});
					
				}
				else
				{


					operation.Responses[key].Content.Clear();
					operation.Responses[key].Content.Add("application/json", new OpenApiMediaType
					{
						Schema = errorSchema // Set schema to just the inner type
					});
				}
			}
			
	}


		
	
}
