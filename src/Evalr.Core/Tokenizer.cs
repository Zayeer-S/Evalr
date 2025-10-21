namespace Evalr.Core;

public static class Tokenizer
{
    public static List<string> GetTokens(string expression)
    {
        return TokenizeExpression(expression);
    }

    private static List<string> TokenizeExpression(string expression)
    {
        var tokens = new List<string>();
        var i = 0;

        while (i < expression.Length)
        {
            var currentChar = expression[i];

            if (currentChar == ' ')
            {
                i++;
                continue;
            }

            if (i + 1 < expression.Length)
            {
                string twoChar = expression.Substring(i, 2);

                if (twoChar == "<=" || twoChar == ">=" || twoChar == "!=")
                {
                    tokens.Add(twoChar);
                    i += 2;
                    continue;
                }
            }

            if (char.IsLetter(currentChar))
            {
                tokens.Add(GetKeywordTokens(expression, ref i));
                continue;
            }

            if (char.IsDigit(currentChar))
            {
                tokens.Add(GetDigitTokens(expression, ref i));
                continue;
            }

            tokens.Add(currentChar.ToString());
            i++;
        }

        return BalanceParantheses(tokens);
    }

    private static string GetKeywordTokens(string expression, ref int i)
    {
        var startPos = i;

        while (i < expression.Length && char.IsLetter(expression[i]))
        {
            i++;
        }

        return expression[startPos..i];
    }

    private static string GetDigitTokens(string expression, ref int i)
    {
        var startPos = i;
        var hasDecimal = false;

        while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
        {
            if (expression[i] == '.')
            {
                if (hasDecimal)
                {
                    break;
                }
                hasDecimal = true;
            }
            i++;
        }
        return expression[startPos..i];
    }

    private static List<string> BalanceParantheses(List<string> tokens)
    {
        var openedCount = tokens.Count(t => t == "(");
        var closedCount = tokens.Count(t => t == ")");
        for (var j = 0; j < openedCount - closedCount; j++)
        {
            tokens.Add(")");
        }
        return tokens;
    }
}
    