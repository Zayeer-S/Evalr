namespace Evalr.Core;

internal static class Evaluator
{
    public static double Evaluate(ExpressionTreeNode node, Dictionary<string, double> variables)
    {
        if (double.TryParse(node.Value, out var number))
        {
            return number;
        }

        var result = node.Value switch
        {
            "+" => Evaluate(node.Left!, variables) + Evaluate(node.Right!, variables),
            "-" => Evaluate(node.Left!, variables) - Evaluate(node.Right!, variables),
            "*" => Evaluate(node.Left!, variables) * Evaluate(node.Right!, variables),
            "/" => DivideWithCheck(node, variables),
            "^" => PowerWithCheck(node, variables),

            "UNARY_MINUS" => -Evaluate(node.Left!, variables),
            "UNARY_PLUS" => Evaluate(node.Left!, variables),

            "<" => Evaluate(node.Left!, variables) < Evaluate(node.Right!, variables) ? 1.0 : 0.0,
            "<=" => Evaluate(node.Left!, variables) <= Evaluate(node.Right!, variables) ? 1.0 : 0.0,
            ">" => Evaluate(node.Left!, variables) > Evaluate(node.Right!, variables) ? 1.0 : 0.0,
            ">=" => Evaluate(node.Left!, variables) >= Evaluate(node.Right!, variables) ? 1.0 : 0.0,
            "=" => Math.Abs(Evaluate(node.Left!, variables) - Evaluate(node.Right!, variables)) < Constants.EPSILON ? 1.0 : 0.0,
            "!=" => Math.Abs(Evaluate(node.Left!, variables) - Evaluate(node.Right!, variables)) >= Constants.EPSILON ? 1.0 : 0.0,

            "and" => (IsTrue(Evaluate(node.Left!, variables)) && IsTrue(Evaluate(node.Right!, variables))) ? 1.0 : 0.0,
            "or" => (IsTrue(Evaluate(node.Left!, variables)) || IsTrue(Evaluate(node.Right!, variables))) ? 1.0 : 0.0,
            "not" => IsTrue(Evaluate(node.Left!, variables)) ? 0.0 : 1.0,

            _ when char.IsLetter(node.Value[0]) => variables[node.Value],

            _ => throw new InvalidOperationException($"Unknown operator: {node.Value}")
        };

        return result;
    }

    private static double DivideWithCheck(ExpressionTreeNode node, Dictionary<string, double> variables)
    {
        var divisor = Evaluate(node.Right!, variables);

        if (Math.Abs(divisor) < Constants.EPSILON)
        {
            throw new DivideByZeroException("Error: Division by zero");
        }

        return Evaluate(node.Left!, variables) / divisor;
    }

    private static double PowerWithCheck(ExpressionTreeNode node, Dictionary<string, double> variables)
    {
        var baseValue = Evaluate(node.Left!, variables);
        var exponent = Evaluate(node.Right!, variables);

        if (baseValue < 0 && Math.Abs(exponent - Math.Round(exponent)) > Constants.EPSILON)
        {
            throw new InvalidOperationException("Error: Cannot raise negative number to non-integer power");
        }

        var result = Math.Pow(baseValue, exponent);

        if (double.IsNaN(result))
        {
            throw new InvalidOperationException("Error: Invalid power operation");
        }
        if (double.IsInfinity(result))
        {
            throw new InvalidOperationException("Error: Result is too large (infinity)");
        }

        return result;
    }

    private static bool IsTrue(double value)
    {
        return Math.Abs(value) > Constants.EPSILON;
    }
}
