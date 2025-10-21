namespace Evalr.Core;

/// <summary>
/// Main orchestration engine for expression evaluation.
/// Coordinates tokenization, validation, parsing, and evaluation of mathematical and boolean expressions.
/// </summary>
public class EvalrEngine
{
    private readonly IReadOnlyDictionary<string, int> _precedenceMap;

    public EvalrEngine()
    {
        _precedenceMap = Constants.PrecedenceMap();
    }

    /// <summary>
    /// Evaluates a mathematical or boolean expression with optional variables.
    /// </summary>
    /// <param name="expression">The expression string to evaluate</param>
    /// <param name="variables">Optional dictionary of variable names and their values</param>
    /// <returns>The computed result as a double (0.0 for false, 1.0 for true in boolean expressions)</returns>
    /// <exception cref="InvalidOperationException">Thrown when expression is invalid or contains errors</exception>
    /// <exception cref="DivideByZeroException">Thrown when division by zero is attempted</exception>
    public EvaluationResult Evaluate(string expression, Dictionary<string, double>? variables = null)
    {
        var tokens = Tokenizer.GetTokens(expression);
        tokens = NormalizeTokens.Normalize(tokens);
        TokenValidator.ValidateTokens(tokens, _precedenceMap);

        var isBooleanExpression = Classifier.IsBooleanExpression(tokens);
        var hasNumericVariables = Classifier.HasNumericVariables(tokens);

        var postfixTokens = ShuntingYard.InfixToPostfix(tokens, _precedenceMap);

        var tree = ExpressionTree.BuildTree(postfixTokens, _precedenceMap, out var variableNames);

        variables ??= new Dictionary<string, double>();
        ValidateVariables(variableNames, variables);

        var result = Evaluator.Evaluate(tree, variables);

        return new EvaluationResult
        {
            Value = result,
            IsBooleanExpression = isBooleanExpression,
            HasNumericVariables = hasNumericVariables,
            Variables = new Dictionary<string, double>(variables),
            OriginalExpression = expression,
            PostfixNotation = string.Join(" ", postfixTokens),
        };
    }

    private void ValidateVariables(HashSet<string> requiredVariables, Dictionary<string, double> providedVariables)
    {
        var missingVariables = requiredVariables.Except(providedVariables.Keys).ToList();

        if (missingVariables.Any())
        {
            throw new InvalidOperationException(
                $"Error: Missing values for variables: {string.Join(", ", missingVariables)}");
        }
    }
}

/// <summary>
/// Represents the result of an expression evaluation.
/// </summary>
public class EvaluationResult
{
    /// <summary>
    /// The computed numerical result.
    /// For boolean expressions: 0.0 = false, 1.0 = true
    /// </summary>
    public double Value { get; init; }

    /// <summary>
    /// Indicates if the expression contains boolean/logical operators.
    /// </summary>
    public bool IsBooleanExpression { get; init; }

    /// <summary>
    /// Indicates if the expression uses numeric variables.
    /// </summary>
    public bool HasNumericVariables { get; init; }

    /// <summary>
    /// The variables used in the evaluation with their values.
    /// </summary>
    public Dictionary<string, double> Variables { get; init; } = new();

    /// <summary>
    /// The original expression string.
    /// </summary>
    public string OriginalExpression { get; init; } = string.Empty;

    /// <summary>
    /// The expression in postfix (RPN) notation.
    /// </summary>
    public string PostfixNotation { get; init; } = string.Empty;

    /// <summary>
    /// Returns the result formatted as a boolean if applicable.
    /// </summary>
    public bool AsBool() => Math.Abs(Value) > Constants.EPSILON;

    /// <summary>
    /// Returns a formatted string representation of the result.
    /// </summary>
    public override string ToString()
    {
        if (IsBooleanExpression && Math.Abs(Value - 1.0) < Constants.EPSILON)
        {
            return "true";
        }
        if (IsBooleanExpression && Math.Abs(Value) < Constants.EPSILON)
        {
            return "false";
        }
        return Value.ToString();
    }
}
