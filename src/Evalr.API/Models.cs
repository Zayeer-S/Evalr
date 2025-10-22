using System.Text.Json.Serialization;

namespace Evalr.API.Models;

/// <summary>
/// Request to evaluate a mathematical or boolean expression.
/// </summary>
public class EvaluateRequest
{
    /// <summary>
    /// The expression to evaluate (e.g., "2 + 3 * x", "a > b and c < d").
    /// </summary>
    [JsonPropertyName("expression")]
    public string Expression { get; set; } = string.Empty;

    /// <summary>
    /// Optional dictionary of variable values.
    /// Key: variable name, Value: numeric value.
    /// </summary>
    [JsonPropertyName("variables")]
    public Dictionary<string, double>? Variables { get; set; }
}

/// <summary>
/// Response containing the evaluation result.
/// </summary>
public class EvaluateResponse
{
    /// <summary>
    /// The computed numerical result.
    /// For boolean expressions: 0.0 = false, 1.0 = true.
    /// </summary>
    [JsonPropertyName("value")]
    public double Value { get; set; }

    /// <summary>
    /// Human-readable string representation of the result.
    /// For boolean expressions, returns "true" or "false".
    /// </summary>
    [JsonPropertyName("displayValue")]
    public string DisplayValue { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if this was a boolean expression.
    /// </summary>
    [JsonPropertyName("isBooleanExpression")]
    public bool IsBooleanExpression { get; set; }

    /// <summary>
    /// The variables that were used in the evaluation.
    /// </summary>
    [JsonPropertyName("variables")]
    public Dictionary<string, double> Variables { get; set; } = new();

    /// <summary>
    /// The original expression that was evaluated.
    /// </summary>
    [JsonPropertyName("expression")]
    public string Expression { get; set; } = string.Empty;

    /// <summary>
    /// The expression in postfix (Reverse Polish) notation.
    /// Useful for debugging and educational purposes.
    /// </summary>
    [JsonPropertyName("postfixNotation")]
    public string? PostfixNotation { get; set; }
}

/// <summary>
/// Request to extract variable names from an expression.
/// </summary>
public class ExtractVariablesRequest
{
    /// <summary>
    /// The expression to analyze for variables.
    /// </summary>
    [JsonPropertyName("expression")]
    public string Expression { get; set; } = string.Empty;
}

/// <summary>
/// Response containing extracted variable names.
/// </summary>
public class ExtractVariablesResponse
{
    /// <summary>
    /// List of variable names found in the expression.
    /// </summary>
    [JsonPropertyName("variables")]
    public List<string> Variables { get; set; } = new();

    /// <summary>
    /// The original expression that was analyzed.
    /// </summary>
    [JsonPropertyName("expression")]
    public string Expression { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the expression contains any variables.
    /// </summary>
    [JsonPropertyName("hasVariables")]
    public bool HasVariables { get; set; }
}

/// <summary>
/// Standard error response for all API endpoints.
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error type/category (e.g., "ValidationError", "EvaluationError").
    /// </summary>
    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;

    /// <summary>
    /// Detailed error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The expression that caused the error (if applicable).
    /// </summary>
    [JsonPropertyName("expression")]
    public string? Expression { get; set; }

    /// <summary>
    /// HTTP status code.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }
}

/// <summary>
/// Health check
/// </summary>
public class HealthResponse
{
    /// <summary>
    /// Service status (e.g., "healthy", "degraded", "unhealthy").
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Service version (from assembly).
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// Current timestamp.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Optional details about what was checked.
    /// </summary>
    [JsonPropertyName("checks")]
    public Dictionary<string, string>? Checks { get; set; }
}
