namespace mock_evaluator;

public class UnitTest1
{
    private static double EvaluateExpression(string expression, Dictionary<string, double>? variables = null)
    {
        var precedenceMap = Constants.PrecedenceMap();
        var tokens = Tokenizer.GetTokens(expression);
        
        if (!TokenValidator.ValidateTokens(tokens, precedenceMap))
        {
            throw new InvalidOperationException("Invalid expression");
        }
        
        var normalizedTokens = NormalizeTokens.Normalize(tokens);
        var postfix = ShuntingYard.InfixToPostfix(normalizedTokens, precedenceMap);
        var tree = ExpressionTree.BuildTree(postfix, precedenceMap, out _);
        
        return Evaluator.Evaluate(tree, variables ?? new Dictionary<string, double>());
    }

    #region Required Test Cases

    [Fact]
    public void TestCase1_ArithmeticExpression()
    {
        var variables = new Dictionary<string, double>
        {
            { "A", 10 },
            { "B", 2 },
            { "C", 3 },
            { "D", 4 },
            { "E", 5 }
        };
        
        var result = EvaluateExpression("A/B^C+D*E-A*C", variables);
        Assert.Equal(-8.75, result, 10);
    }

    [Fact]
    public void TestCase2_ArithmeticWithParentheses()
    {
        var variables = new Dictionary<string, double>
        {
            { "A", 10 },
            { "B", 2 },
            { "C", 3 },
            { "D", 4 },
            { "E", 5 }
        };
        
        var result = EvaluateExpression("A/B^(C+D)*(E-A)*C", variables);
        Assert.Equal(-1.171875, result, 10);
    }

    [Fact]
    public void TestCase3_PureBooleanExpression()
    {
        var variables = new Dictionary<string, double>
        {
            { "p", 1.0 },
            { "q", 0.0 }
        };
        
        var result = EvaluateExpression("(p and q) and not (p or q)", variables);
        Assert.Equal(0.0, result);
        
        variables["p"] = 1.0;
        variables["q"] = 1.0;
        result = EvaluateExpression("(p and q) and not (p or q)", variables);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void TestCase4_MixedComparisonAndLogical()
    {
        var variables = new Dictionary<string, double>
        {
            { "A", 5 },
            { "C", 3 }
        };
        
        var result = EvaluateExpression("(A>0) or not (A<=C)", variables);
        Assert.Equal(1.0, result);
        
        variables["A"] = -5;
        result = EvaluateExpression("(A>0) or not (A<=C)", variables);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void TestCase5_UnaryMinusWithArithmetic()
    {
        var variables = new Dictionary<string, double>
        {
            { "X", 3 }
        };
        
        var result = EvaluateExpression("-(2*X^2+5)", variables);
        Assert.Equal(-23.0, result);
    }

    [Fact]
    public void TestCase6_DoubleUnaryMinus()
    {
        var result = EvaluateExpression("-(5)-(-5)");
        Assert.Equal(0.0, result);
    }

    #endregion

    #region Arithmetic Operations

    [Fact]
    public void Addition_SimpleNumbers()
    {
        var result = EvaluateExpression("5+3");
        Assert.Equal(8.0, result);
    }

    [Fact]
    public void Subtraction_SimpleNumbers()
    {
        var result = EvaluateExpression("10-4");
        Assert.Equal(6.0, result);
    }

    [Fact]
    public void Multiplication_SimpleNumbers()
    {
        var result = EvaluateExpression("6*7");
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void Division_SimpleNumbers()
    {
        var result = EvaluateExpression("20/4");
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void Power_SimpleNumbers()
    {
        var result = EvaluateExpression("2^10");
        Assert.Equal(1024.0, result);
    }

    [Fact]
    public void Power_RightAssociative()
    {
        var result = EvaluateExpression("2^3^2");
        Assert.Equal(512.0, result);
    }

    [Fact]
    public void Division_ByZero_ThrowsException()
    {
        Assert.Throws<DivideByZeroException>(() => EvaluateExpression("5/0"));
    }

    [Fact]
    public void Power_NegativeBaseNonIntegerExponent_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => EvaluateExpression("(-4)^0.5"));
    }

    [Fact]
    public void Decimals_Operations()
    {
        var result = EvaluateExpression("3.14+2.86");
        Assert.Equal(6.0, result, 10);
    }

    #endregion

    #region Operator Precedence

    [Fact]
    public void Precedence_MultiplicationBeforeAddition()
    {
        var result = EvaluateExpression("2+3*4");
        Assert.Equal(14.0, result);
    }

    [Fact]
    public void Precedence_DivisionBeforeSubtraction()
    {
        var result = EvaluateExpression("20-10/2");
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void Precedence_PowerBeforeMultiplication()
    {
        var result = EvaluateExpression("2*3^2");
        Assert.Equal(18.0, result);
    }

    [Fact]
    public void Precedence_ParenthesesOverride()
    {
        var result = EvaluateExpression("(2+3)*4");
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void Precedence_ComplexExpression()
    {
        var result = EvaluateExpression("2+3*4^2-10/5");
        Assert.Equal(48.0, result);
    }

    #endregion

    #region Unary Operators

    [Fact]
    public void UnaryMinus_SingleNumber()
    {
        var result = EvaluateExpression("-5");
        Assert.Equal(-5.0, result);
    }

    [Fact]
    public void UnaryPlus_SingleNumber()
    {
        var result = EvaluateExpression("+5");
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void UnaryMinus_Double()
    {
        var result = EvaluateExpression("--5");
        Assert.Equal(5.0, result);
    }

    [Fact]
    public void UnaryMinus_WithParentheses()
    {
        var result = EvaluateExpression("-(3+2)");
        Assert.Equal(-5.0, result);
    }

    [Fact]
    public void UnaryMinus_InMiddleOfExpression()
    {
        var result = EvaluateExpression("5*-3");
        Assert.Equal(-15.0, result);
    }

    [Fact]
    public void UnaryOperators_Complex()
    {
        var result = EvaluateExpression("-(-5)+(-3)");
        Assert.Equal(2.0, result);
    }

    #endregion

    #region Comparison Operators

    [Fact]
    public void Comparison_LessThan_True()
    {
        var result = EvaluateExpression("3<5");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Comparison_LessThan_False()
    {
        var result = EvaluateExpression("5<3");
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Comparison_GreaterThan_True()
    {
        var result = EvaluateExpression("7>2");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Comparison_LessThanOrEqual_Equal()
    {
        var result = EvaluateExpression("5<=5");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Comparison_GreaterThanOrEqual_Greater()
    {
        var result = EvaluateExpression("10>=5");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Comparison_Equal_True()
    {
        var result = EvaluateExpression("5=5");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Comparison_Equal_False()
    {
        var result = EvaluateExpression("5=3");
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Comparison_NotEqual_True()
    {
        var result = EvaluateExpression("5!=3");
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Comparison_NotEqual_False()
    {
        var result = EvaluateExpression("5!=5");
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Comparison_WithArithmetic()
    {
        var result = EvaluateExpression("2+3>4");
        Assert.Equal(1.0, result);
    }

    #endregion

    #region Logical Operators

    [Fact]
    public void Logical_And_TrueTrue()
    {
        var vars = new Dictionary<string, double> { { "a", 1.0 }, { "b", 1.0 } };
        var result = EvaluateExpression("a and b", vars);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Logical_And_TrueFalse()
    {
        var vars = new Dictionary<string, double> { { "a", 1.0 }, { "b", 0.0 } };
        var result = EvaluateExpression("a and b", vars);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Logical_Or_FalseFalse()
    {
        var vars = new Dictionary<string, double> { { "a", 0.0 }, { "b", 0.0 } };
        var result = EvaluateExpression("a or b", vars);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Logical_Or_TrueFalse()
    {
        var vars = new Dictionary<string, double> { { "a", 1.0 }, { "b", 0.0 } };
        var result = EvaluateExpression("a or b", vars);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Logical_Not_True()
    {
        var vars = new Dictionary<string, double> { { "a", 1.0 } };
        var result = EvaluateExpression("not(a)", vars);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Logical_Not_False()
    {
        var vars = new Dictionary<string, double> { { "a", 0.0 } };
        var result = EvaluateExpression("not(a)", vars);
        Assert.Equal(1.0, result);
    }

    [Fact]
    public void Logical_ComplexExpression()
    {
        var vars = new Dictionary<string, double> { { "a", 1.0 }, { "b", 0.0 }, { "c", 1.0 } };
        var result = EvaluateExpression("(a or b) and c", vars);
        Assert.Equal(1.0, result);
    }

    #endregion

    #region Variables

    [Fact]
    public void Variables_CaseSensitive()
    {
        var vars = new Dictionary<string, double> { { "A", 10.0 }, { "a", 5.0 } };
        var result = EvaluateExpression("A+a", vars);
        Assert.Equal(15.0, result);
    }

    [Fact]
    public void Variables_SingleLetter()
    {
        var vars = new Dictionary<string, double> { { "x", 7.0 } };
        var result = EvaluateExpression("x*2", vars);
        Assert.Equal(14.0, result);
    }

    [Fact]
    public void Variables_MultiLetter()
    {
        var vars = new Dictionary<string, double> { { "value", 100.0 } };
        var result = EvaluateExpression("value/10", vars);
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void Variables_MultipleInExpression()
    {
        var vars = new Dictionary<string, double> 
        { 
            { "x", 3.0 }, 
            { "y", 4.0 }, 
            { "z", 5.0 } 
        };
        var result = EvaluateExpression("x^2+y^2-z", vars);
        Assert.Equal(20.0, result);
    }

    #endregion

    #region Parentheses

    [Fact]
    public void Parentheses_Simple()
    {
        var result = EvaluateExpression("(5+3)*2");
        Assert.Equal(16.0, result);
    }

    [Fact]
    public void Parentheses_Nested()
    {
        var result = EvaluateExpression("((2+3)*4)+5");
        Assert.Equal(25.0, result);
    }

    [Fact]
    public void Parentheses_MultipleGroups()
    {
        var result = EvaluateExpression("(2+3)*(4+5)");
        Assert.Equal(45.0, result);
    }

    [Fact]
    public void Parentheses_WithUnaryMinus()
    {
        var result = EvaluateExpression("-(5+3)");
        Assert.Equal(-8.0, result);
    }

    [Fact]
    public void Parentheses_AutoBalance()
    {
        var precedenceMap = Constants.PrecedenceMap();
        var tokens = Tokenizer.GetTokens("(5+3");
        
        var closingParens = tokens.Count(t => t == ")");
        var openingParens = tokens.Count(t => t == "(");
        Assert.Equal(openingParens, closingParens);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void EdgeCase_SingleNumber()
    {
        var result = EvaluateExpression("42");
        Assert.Equal(42.0, result);
    }

    [Fact]
    public void EdgeCase_SingleVariable()
    {
        var vars = new Dictionary<string, double> { { "x", 99.0 } };
        var result = EvaluateExpression("x", vars);
        Assert.Equal(99.0, result);
    }

    [Fact]
    public void EdgeCase_LargeNumbers()
    {
        var result = EvaluateExpression("1000000+2000000");
        Assert.Equal(3000000.0, result);
    }

    [Fact]
    public void EdgeCase_VerySmallDecimals()
    {
        var result = EvaluateExpression("0.0001+0.0002");
        Assert.Equal(0.0003, result, 10);
    }

    [Fact]
    public void EdgeCase_ZeroInOperations()
    {
        var result = EvaluateExpression("5*0+3");
        Assert.Equal(3.0, result);
    }

    [Fact]
    public void EdgeCase_NegativeResults()
    {
        var result = EvaluateExpression("5-10");
        Assert.Equal(-5.0, result);
    }

    #endregion

    #region Token Validation

    [Fact]
    public void Validation_EmptyExpression_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => EvaluateExpression(""));
    }

    [Fact]
    public void Validation_InvalidToken_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => EvaluateExpression("5@3"));
    }

    [Fact]
    public void Validation_MissingOperator_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => EvaluateExpression("5 3"));
    }

    [Fact]
    public void Validation_TrailingOperator_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => EvaluateExpression("5+"));
    }

    [Fact]
    public void Validation_LeadingBinaryOperator_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => EvaluateExpression("*5"));
    }

    #endregion

    #region Classifier Tests

    [Fact]
    public void Classifier_ArithmeticExpression()
    {
        var tokens = Tokenizer.GetTokens("A+B*C");
        Assert.False(Classifier.IsBooleanExpression(tokens));
        Assert.True(Classifier.HasNumericVariables(tokens));
    }

    [Fact]
    public void Classifier_BooleanExpression()
    {
        var tokens = Tokenizer.GetTokens("A and B");
        Assert.True(Classifier.IsBooleanExpression(tokens));
        Assert.False(Classifier.HasNumericVariables(tokens));
    }

    [Fact]
    public void Classifier_MixedExpression()
    {
        var tokens = Tokenizer.GetTokens("(A>5) and B");
        Assert.True(Classifier.IsBooleanExpression(tokens));
        Assert.True(Classifier.HasNumericVariables(tokens));
    }

    [Fact]
    public void Classifier_SingleVariable()
    {
        var tokens = Tokenizer.GetTokens("X");
        Assert.False(Classifier.IsBooleanExpression(tokens));
        Assert.True(Classifier.HasNumericVariables(tokens));
    }

    #endregion
}