namespace Evalr.Core;

internal static class Constants
{
    public const double EPSILON = 1e-10;

    public static IReadOnlyDictionary<string, string> NormalizationMap()
    {
        return new Dictionary<string, string>
        {
            { "≤", "<=" },
            { "≥", ">=" },
            { "≠", "!=" },
            { "×", "*" },
            { "÷", "/" },
        };
    }

    public static IReadOnlyDictionary<string, int> PrecedenceMap()
    {
        var precedence = new Dictionary<string, int>();

        foreach (var kvp in LogicalOperatorsMap())
        {
            precedence[kvp.Key] = kvp.Value;
        }
        foreach (var kvp in ComparisonOperatorsMap())
        {
            precedence[kvp.Key] = kvp.Value;
        }
        foreach (var kvp in AdditiveOperatorsMap())
        {
            precedence[kvp.Key] = kvp.Value;
        }
        foreach (var kvp in MultiplicativeOperatorsMap())
        {
            precedence[kvp.Key] = kvp.Value;
        }
        foreach (var kvp in ExponentianOperatorsMap())
        {
            precedence[kvp.Key] = kvp.Value;
        }
        foreach (var kvp in UnaryOperatorsMap())
        {
            precedence[kvp.Key] = kvp.Value;
        }

        return precedence;
    }

    public static IReadOnlyDictionary<string, int> LogicalOperatorsMap()
    {
        return new Dictionary<string, int>
        {
            { "or", 1 },
            { "and", 2 },
        };
    }

    public static IReadOnlyDictionary<string, int> ComparisonOperatorsMap()
    {
        return new Dictionary<string, int>
        {
            { "<", 4 },
            { "≤", 4 },
            { "<=", 4 },
            { ">", 4 },
            { "≥", 4 },
            { ">=", 4 },
            { "=", 4 },
            { "≠", 4 },
            { "!=", 4 },
        };
    }

    public static IReadOnlyDictionary<string, int> AdditiveOperatorsMap()
    {
        return new Dictionary<string, int>
        {
            { "+", 5 },
            { "-", 5 },
        };
    }

    public static IReadOnlyDictionary<string, int> MultiplicativeOperatorsMap()
    {
        return new Dictionary<string, int>
        {
            { "*", 6 },
            { "×", 6 },
            { "/", 6 },
            { "÷", 6 },
        };
    }

    public static IReadOnlyDictionary<string, int> ExponentianOperatorsMap()
    {
        return new Dictionary<string, int>
        {
            { "^", 7 },
        };
    }

    public static IReadOnlyDictionary<string, int> UnaryOperatorsMap()
    {
        return new Dictionary<string, int>
        {
            { "not", 3 },
            { "UNARY_MINUS", 8 },
            { "UNARY_PLUS", 8 },
        };
    }
}
