namespace Evalr.Core;

internal class ExpressionTreeNode
{
    public string Value { get; set; }
    public ExpressionTreeNode? Left { get; set; }
    public ExpressionTreeNode? Right { get; set; }

    public ExpressionTreeNode(string value)
    {
        Value = value;
        Left = null;
        Right = null;
    }

    public bool IsOperator(IReadOnlyDictionary<string, int> precedenceMap)
    {
        return precedenceMap.ContainsKey(Value);
    }

    public bool IsUnaryOperator()
    {
        return Value == "not" || Value == "UNARY_MINUS" || Value == "UNARY_PLUS";
    }

    public bool IsOperand(IReadOnlyDictionary<string, int> precedenceMap)
    {
        return !IsOperator(precedenceMap) && Value != "(" && Value != ")";
    }
}

internal static class ExpressionTree
{
    public static ExpressionTreeNode BuildTree(List<string> postfixTokens, IReadOnlyDictionary<string, int> precedenceMap, out HashSet<string> variableNames)
    {
        var unaryOps = Constants.UnaryOperatorsMap().Keys;
        variableNames = [];
        var stack = new Stack<ExpressionTreeNode>();

        foreach (var token in postfixTokens)
        {
            var node = new ExpressionTreeNode(token);

            if (!precedenceMap.ContainsKey(token))
            {
                if (!double.TryParse(token, out _))
                {
                    variableNames.Add(token);
                }
                stack.Push(node);
            }
            else if (unaryOps.Contains(token))
            {
                node.Left = stack.Pop();
                stack.Push(node);
            }
            else
            {
                node.Right = stack.Pop();
                node.Left = stack.Pop();
                stack.Push(node);
            }
        }

        return stack.Pop();
    }
}
