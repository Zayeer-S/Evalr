public static class TokenValidator
{
    private static readonly HashSet<string> logicalOps = new HashSet<string>(Constants.LogicalOperatorsMap().Keys);

    public static bool ValidateTokens(List<string> tokens, IReadOnlyDictionary<string, int> PRECEDENCE_MAP)
    {
        if (tokens.Count == 0)
        {
            Console.WriteLine("Error: Expression cannot be empty");
            return false;
        }

        var COMPARISON_OPS = Constants.ComparisonOperatorsMap().Keys;
        var ADDITIVE_OPS = Constants.AdditiveOperatorsMap().Keys;
        var MULTIPLICATIVE_OPS = Constants.MultiplicativeOperatorsMap().Keys;
        var EXPONENTIAL_OPS = Constants.ExponentianOperatorsMap().Keys;
        var LOGICAL_OPS = Constants.LogicalOperatorsMap().Keys;

        var BINARY_OPS = new HashSet<string>(
            COMPARISON_OPS
            .Concat(ADDITIVE_OPS)
            .Concat(MULTIPLICATIVE_OPS)
            .Concat(EXPONENTIAL_OPS)
            .Concat(LOGICAL_OPS)
        );

        var ALL_OPS = new HashSet<string>(PRECEDENCE_MAP.Keys);

        var parenthesesCount = 0;
        foreach (var token in tokens)
        {
            if (token == "(")
            {
                parenthesesCount++;
            }
            if (token == ")")
            {
                parenthesesCount--;
                if (parenthesesCount < 0)
                {
                    Console.WriteLine("Error: Parantheses not balanced");
                    return false;
                }
            }
        }

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            var prevToken = i > 0 ? tokens[i - 1] : null;
            var nextToken = i < tokens.Count - 1 ? tokens[i + 1] : null;

            if (!IsValidToken(token, ALL_OPS))
            {
                Console.WriteLine($"Error: Invalid token '{token}'");
                return false;
            }

            if (IsNumber(token))
            {
                if (!IsValidNumber(token))
                {
                    Console.WriteLine($"Error: Invalid number format '{token}'");
                    return false;
                }

                if (nextToken != null && (IsNumber(nextToken) || IsVariable(nextToken)))
                {
                    Console.WriteLine($"Error: Missing operator between '{token}' and '{nextToken}'");
                    return false;
                }
            }

            if (IsVariable(token))
            {
                if (nextToken != null && (IsNumber(nextToken) || IsVariable(nextToken)))
                {
                    Console.WriteLine($"Error: Missing operator between '{token}' and '{nextToken}'");
                    return false;
                }
            }

            bool isUnaryPlusMinus = (token == "+" || token == "-") && 
                                    (prevToken == null || prevToken == "(" || PRECEDENCE_MAP.ContainsKey(prevToken));

            if (BINARY_OPS.Contains(token) && !isUnaryPlusMinus)
            {
                if (prevToken == null || prevToken == "(" || ALL_OPS.Contains(prevToken))
                {
                    Console.WriteLine($"Error: Binary operator '{token}' missing left operand");
                    return false;
                }
                if (nextToken == null || nextToken == ")")
                {
                    Console.WriteLine($"Error: Binary operator '{token}' missing right operand");
                    return false;
                }
            }

            if (isUnaryPlusMinus)
            {
                if (nextToken == null || nextToken == ")")
                {
                    Console.WriteLine($"Error: Unary operator '{token}' missing operand");
                    return false;
                }
            }

            if (token == "not")
            {
                if (nextToken == null || nextToken == ")")
                {
                    Console.WriteLine("Error: 'not' operator missing operand");
                    return false;
                }
            }

            if (token == "(")
            {
                if (nextToken != null && nextToken == ")")
                {
                    Console.WriteLine("Error: Empty parantheses '()'");
                    return false;
                }
                if (nextToken != null && BINARY_OPS.Contains(nextToken) && nextToken != "+" && nextToken != "-")
                {
                    Console.WriteLine($"Error: '(' cannot be followed by binary operator '{nextToken}'");
                    return false;
                }
            }

            if (token == ")")
            {
                if (nextToken != null && (IsNumber(nextToken) || IsVariable(nextToken)))
                {
                    Console.WriteLine($"Error: Missing operator between ')' and '{nextToken}'");
                    return false;
                }
            }
        }

        var firstToken = tokens[0];

        if (BINARY_OPS.Contains(firstToken) && !ADDITIVE_OPS.Contains(firstToken))
        {
            Console.WriteLine($"Error: Expression cannot start with binary operator '{firstToken}'");
            return false;
        }

        var lastToken = tokens[^1];
        if (ALL_OPS.Contains(lastToken))
        {
            Console.WriteLine($"Error: Expression cannot end with operator '{lastToken}'");
            return false;
        }

        return true;
    }

    private static bool IsValidToken(string token, HashSet<string> allOps)
    {
        if (token == "(" || token == ")")
        {
            return true;
        }
        if (allOps.Contains(token))
        {
            return true;
        }
        if (IsNumber(token))
        {
            return true;
        }
        if (IsVariable(token))
        {
            return true;
        }
        if (token == "≤" || token == "≥" || token == "≠" || token == "×" || token == "÷")
        {
            return true;
        }
        return false;
    }

    private static bool IsNumber(string token)
    {
        return double.TryParse(token, out _);
    }

    private static bool IsValidNumber(string token)
    {
        var decimalCount = token.Count(c => c == '.');
        if (decimalCount > 1)
        {
            return false;
        }

        if (token.StartsWith(".") || token.EndsWith("."))
        {
            return false;
        }

        return double.TryParse(token, out _);
    }

    private static bool IsVariable(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return false;
        }
        if (!char.IsLetter(token[0]))
        {
            return false;
        }

        return !logicalOps.Contains(token);
    }
}