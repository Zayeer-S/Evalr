namespace Evalr.Core;

internal static class ShuntingYard
{
    public static List<string> InfixToPostfix(List<string> tokens, IReadOnlyDictionary<string, int> precedenceMap)
    {
        var output = new List<string>();
        var operatorStack = new Stack<string>();

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            if (IsOperand(token, precedenceMap))
            {
                output.Add(token);
                continue;
            }

            if (token == "(")
            {
                operatorStack.Push(token);
                continue;
            }

            if (token == ")")
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    output.Add(operatorStack.Pop());
                }
                if (operatorStack.Count > 0)
                {
                    operatorStack.Pop();
                }
                continue;
            }

            if (IsUnaryOperator(token, i, tokens, precedenceMap))
            {
                var unaryToken = token == "-" ? "UNARY_MINUS" : "UNARY_PLUS";

                while (operatorStack.Count > 0 &&
                       operatorStack.Peek() != "(" &&
                       precedenceMap.ContainsKey(operatorStack.Peek()) &&
                       precedenceMap[operatorStack.Peek()] > precedenceMap[unaryToken])
                {
                    output.Add(operatorStack.Pop());
                }
                operatorStack.Push(unaryToken);
                continue;
            }

            if (precedenceMap.ContainsKey(token))
            {
                while (operatorStack.Count > 0 &&
                       operatorStack.Peek() != "(" &&
                       precedenceMap.ContainsKey(operatorStack.Peek()))
                {
                    var topPrecedence = precedenceMap[operatorStack.Peek()];
                    var currentPrecedence = precedenceMap[token];

                    if (token == "^")
                    {
                        if (topPrecedence > currentPrecedence)
                        {
                            output.Add(operatorStack.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (topPrecedence >= currentPrecedence)
                        {
                            output.Add(operatorStack.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                operatorStack.Push(token);
            }
        }

        while (operatorStack.Count > 0)
        {
            output.Add(operatorStack.Pop());
        }

        return output;
    }

    private static bool IsOperand(string token, IReadOnlyDictionary<string, int> precedenceMap)
    {
        return token != "(" && token != ")" && !precedenceMap.ContainsKey(token);
    }

    private static bool IsUnaryOperator(string token, int index, List<string> tokens, IReadOnlyDictionary<string, int> precedenceMap)
    {
        if (token != "+" && token != "-")
        {
            return false;
        }

        if (index == 0)
        {
            return true;
        }

        var prevToken = tokens[index - 1];

        return prevToken == "(" ||
               precedenceMap.ContainsKey(prevToken);
    }
}
