using System.Net;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Evalr.API.Models;
using Evalr.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Evalr.API;

/// <summary>
/// AWS lambda function handler for the Evalr expression evaluation API
/// </summary>
public class LambdaHandler
{
    private static readonly EvalrEngine Engine = new();
    private static readonly string Version = GetVersion();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };

    /// <summary>
    /// Main handler for API gateway proxy integrations
    /// </summary>
    /// <returns></returns>
    public APIGatewayProxyResponse HandleRequest(APIGatewayProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation($"Received {request.HttpMethod} request to {request.Path}");

        try
        {
            return request.Path switch
            {
                "/health" or "/api/health" => HandleHealth(context),
                "/evaluate" or "/api/evaluate" => HandleEvaluate(request, context),
                "/extract-variables" or "/api/extract-variables" => HandleExtractVariables(request, context),
                _ => CreateErrorResponse(HttpStatusCode.NotFound, "NotFound", $"Endpoint not found: {request.Path}")
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error: {ex}");
            return CreateErrorResponse(
                HttpStatusCode.InternalServerError,
                "InternalServerError",
                "An unexpected error occurred"
            );
        }
    }

    /// <summary>
    /// Health check endpoint.
    /// GET /health
    /// </summary>
    private APIGatewayProxyResponse HandleHealth(ILambdaContext context)
    {
        var checks = new Dictionary<string, string>();
        var isHealthy = true;

        try
        {
            // Test the engine works
            Engine.Evaluate("2 + 2");
            checks["engine"] = "ok";
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Health check failed: {ex}");
            checks["engine"] = "failed";
            isHealthy = false;
        }

        var response = new HealthResponse
        {
            Status = isHealthy ? "healthy" : "unhealthy",
            Version = Version,
            Checks = checks
        };

        return CreateSuccessResponse(response, isHealthy ? HttpStatusCode.OK : HttpStatusCode.ServiceUnavailable);
    }

    /// <summary>
    /// Evaluate expression endpoint.
    /// POST /evaluate
    /// </summary>
    private APIGatewayProxyResponse HandleEvaluate(APIGatewayProxyRequest request, ILambdaContext context)
    {
        if (request.HttpMethod != "POST")
        {
            return CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "MethodNotAllowed", "Only POST method is allowed");
        }

        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return CreateErrorResponse(HttpStatusCode.BadRequest, "BadRequest", "Request body is required");
        }

        try
        {
            var evaluateRequest = JsonSerializer.Deserialize<EvaluateRequest>(request.Body, JsonOptions);

            if (evaluateRequest == null || string.IsNullOrWhiteSpace(evaluateRequest.Expression))
            {
                return CreateErrorResponse(HttpStatusCode.BadRequest, "ValidationError", "Expression is required");
            }

            context.Logger.LogInformation($"Evaluating expression: {evaluateRequest.Expression}");

            var result = Engine.Evaluate(evaluateRequest.Expression, evaluateRequest.Variables);

            var response = new EvaluateResponse
            {
                Value = result.Value,
                DisplayValue = result.ToString(),
                IsBooleanExpression = result.IsBooleanExpression,
                Variables = result.Variables,
                Expression = result.OriginalExpression,
                PostfixNotation = result.PostfixNotation
            };

            return CreateSuccessResponse(response);
        }
        catch (InvalidOperationException ex)
        {
            context.Logger.LogWarning($"Validation error: {ex.Message}");
            return CreateErrorResponse(HttpStatusCode.BadRequest, "ValidationError", ex.Message, request.Body);
        }
        catch (DivideByZeroException ex)
        {
            context.Logger.LogWarning($"Division by zero: {ex.Message}");
            return CreateErrorResponse(HttpStatusCode.BadRequest, "EvaluationError", ex.Message, request.Body);
        }
        catch (JsonException ex)
        {
            context.Logger.LogWarning($"Invalid JSON: {ex.Message}");
            return CreateErrorResponse(HttpStatusCode.BadRequest, "InvalidJson", "Invalid JSON in request body");
        }
    }

    /// <summary>
    /// Extract variables endpoint.
    /// POST /extract-variables
    /// </summary>
    private APIGatewayProxyResponse HandleExtractVariables(APIGatewayProxyRequest request, ILambdaContext context)
    {
        if (request.HttpMethod != "POST")
        {
            return CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "MethodNotAllowed", "Only POST method is allowed");
        }

        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return CreateErrorResponse(HttpStatusCode.BadRequest, "BadRequest", "Request body is required");
        }

        try
        {
            var extractRequest = JsonSerializer.Deserialize<ExtractVariablesRequest>(request.Body, JsonOptions);

            if (extractRequest == null || string.IsNullOrWhiteSpace(extractRequest.Expression))
            {
                return CreateErrorResponse(HttpStatusCode.BadRequest, "ValidationError", "Expression is required");
            }

            context.Logger.LogInformation($"Extracting variables from: {extractRequest.Expression}");

            var variables = Engine.ExtractVariables(extractRequest.Expression);

            var response = new ExtractVariablesResponse
            {
                Variables = variables,
                Expression = extractRequest.Expression,
                HasVariables = variables.Count > 0
            };

            return CreateSuccessResponse(response);
        }
        catch (JsonException ex)
        {
            context.Logger.LogWarning($"Invalid JSON: {ex.Message}");
            return CreateErrorResponse(HttpStatusCode.BadRequest, "InvalidJson", "Invalid JSON in request body");
        }
    }

    /// <summary>
    /// Creates a successful API Gateway response.
    /// </summary>
    private APIGatewayProxyResponse CreateSuccessResponse<T>(T data, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = (int)statusCode,
            Body = JsonSerializer.Serialize(data, JsonOptions),
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" },
                { "Access-Control-Allow-Methods", "GET, POST, OPTIONS" },
                { "Access-Control-Allow-Headers", "Content-Type" }
            }
        };
    }

    /// <summary>
    /// Creates an error API Gateway response.
    /// </summary>
    private static APIGatewayProxyResponse CreateErrorResponse(
        HttpStatusCode statusCode,
        string error,
        string message,
        string? expression = null
    )
    {
        var errorResponse = new ErrorResponse
        {
            Error = error,
            Message = message,
            Expression = expression,
            StatusCode = (int)statusCode,
        };

        return new APIGatewayProxyResponse
        {
            StatusCode = (int)statusCode,
            Body = JsonSerializer.Serialize(errorResponse, JsonOptions),
            Headers = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                { "Access-Control-Allow-Origin", "*" },
                { "Access-Control-Allow-Methods", "GET, POST, OPTIONS" },
                { "Access-Control-Allow-Headers", "Content-Type" }
            }
        };
    }


    /// <summary>
    /// Gets the assembly version
    /// </summary>
    private static string GetVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?? assembly.GetName().Version?.ToString()
        ?? "unknown";
    }
}
