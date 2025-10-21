namespace Evalr.Core;

internal static class Classifier
{
    public static bool IsBooleanExpression(List<string> tokens)
    {
        var logicalOps = Constants.LogicalOperatorsMap().Keys;
        var comparisonOps = Constants.ComparisonOperatorsMap().Keys;

        return tokens.Any(t => logicalOps.Contains(t) || comparisonOps.Contains(t));
    }

    public static bool HasNumericVariables(List<string> tokens)
    {
        var comparisonOps = Constants.ComparisonOperatorsMap().Keys;
        var arithmeticOps = Constants.AdditiveOperatorsMap().Keys
            .Concat(Constants.MultiplicativeOperatorsMap().Keys)
            .Concat(Constants.ExponentianOperatorsMap().Keys);
        var logicalOps = Constants.LogicalOperatorsMap().Keys;

        bool hasArithmetic = tokens.Any(t => arithmeticOps.Contains(t));
        bool hasComparison = tokens.Any(t => comparisonOps.Contains(t));
        bool hasLogical = tokens.Any(t => logicalOps.Contains(t));

        if (hasArithmetic && !hasComparison && !hasLogical)
        {
            return true;
        }

        if (!hasArithmetic && !hasComparison && hasLogical)
        {
            return false;
        }

        if (hasComparison)
        {
            return true;
        }

        if (!hasArithmetic && !hasComparison && !hasLogical)
        {
            return true;
        }

        return true;
    }
}
