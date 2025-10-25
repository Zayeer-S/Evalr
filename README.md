# Evalr - Expression Evaluator

A mathematical and boolean expression evaluation engine built in C# (.NET 9). This project explores compiler design concepts, data structures, and algorithms through implementing a custom expression parser and evaluator.

> **Academic Project**: Originally developed as coursework and refactored to explore advanced software engineering concepts including API design, containerization, and cloud deployment.

## What I Learned

### Core Computer Science Concepts

**1. Compiler Design & Parsing**
- Tokenization and lexical analysis
- Syntax validation and error reporting
- Converting between infix and postfix notation
- Abstract Syntax Trees (AST) construction and traversal

**2. Data Structures**
- Stack-based algorithms (Shunting Yard)
- Binary tree construction and evaluation
- Hash tables for operator precedence and variable storage

**3. Algorithms**
- **Dijkstra's Shunting Yard Algorithm** - Converting infix expressions to postfix notation
- **Recursive tree evaluation** - Post-order traversal for expression computation
- **Operator precedence handling** - Implementing correct order of operations

**4. Software Architecture**
- Separation of concerns (API layer vs Core logic)
- Single Responsibility Principle in class design
- Error handling and validation patterns
- RESTful API design principles

**5. Modern Development Practices**
- Containerization with Docker
- CI/CD pipelines with GitHub Actions
- AWS Lambda serverless architecture
- API Gateway integration

## Features

- Evaluates arithmetic expressions with proper operator precedence
- Supports boolean logic with `and`, `or`, `not` operators
- Handles comparison operators (`<`, `<=`, `>`, `>=`, `=`, `!=`)
- Variable substitution in expressions
- Comprehensive error handling and validation
- Expression analysis (extract variables without evaluation)

## How It Works

### The Evaluation Pipeline

```
User Input → Tokenizer → Normalizer → Validator → Classifier
                ↓
         Shunting Yard → Expression Tree → Evaluator → Result
```

**1. Tokenization** (`Tokenizer.cs`)
- Breaks input string into meaningful tokens (numbers, operators, variables, parentheses)
- Handles multi-character operators like `<=`, `>=`, `!=`
- Recognizes decimal numbers and variable names

**2. Normalization** (`NormalizeTokens.cs`)
- Converts unicode symbols to ASCII equivalents (e.g., `≤` → `<=`)
- Ensures consistent operator representation

**3. Validation** (`TokenValidator.cs`)
- Checks for syntax errors (missing operators, unbalanced parentheses)
- Validates number formats
- Ensures operators have required operands

**4. Classification** (`Classifier.cs`)
- Determines if expression is boolean or arithmetic
- Identifies expression characteristics for proper evaluation

**5. Shunting Yard Algorithm** (`ShuntingYard.cs`)
- Converts infix notation (e.g., `2 + 3 * 4`) to postfix (e.g., `2 3 4 * +`)
- Respects operator precedence and associativity
- Handles unary operators (`-x`, `not x`)

**6. Expression Tree** (`ExpressionTree.cs`)
- Builds a binary tree from postfix notation
- Each operator becomes a node with operands as children
- Extracts variable names during construction

**7. Evaluation** (`Evaluator.cs`)
- Recursively evaluates the tree using post-order traversal
- Handles all operators and variable substitution
- Implements error checking (division by zero, invalid operations)

## Supported Operations

### Arithmetic Operators
- Addition: `+`
- Subtraction: `-`
- Multiplication: `*`, `×`
- Division: `/`, `÷`
- Exponentiation: `^`
- Unary Plus/Minus: `+x`, `-x`

### Comparison Operators
- Less than: `<`
- Less than or equal: `<=`, `≤`
- Greater than: `>`
- Greater than or equal: `>=`, `≥`
- Equal: `=`
- Not equal: `!=`, `≠`

### Logical Operators
- AND: `and`
- OR: `or`
- NOT: `not`

## Example Expressions

```javascript
// Arithmetic
"2 + 3 * 4"                    // Result: 14 (multiplication before addition)
"(2 + 3) * 4"                  // Result: 20 (parentheses override precedence)
"2^3 + 1"                      // Result: 9 (exponentiation before addition)

// With variables
"x * (y + z)"                  // Requires: x, y, z values
"radius^2 * 3.14159"           // Requires: radius value

// Boolean expressions
"5 > 3"                        // Result: true (1.0)
"x <= 10 and y >= 5"           // Requires: x, y values
"not (a = b)"                  // Requires: a, b values

// Complex mixed expressions
"(price * quantity) > 1000"    // Arithmetic then comparison
"temp >= 20 and temp <= 30"    // Multiple comparisons with logic
```

## Technical Deep Dive

### Operator Precedence Implementation

The engine uses a precedence map to correctly handle order of operations:

```
Level 8: Unary operators (-, +, not)
Level 7: Exponentiation (^)
Level 6: Multiplication, Division (*, /)
Level 5: Addition, Subtraction (+, -)
Level 4: Comparison (<, <=, >, >=, =, !=)
Level 2: Logical AND (and)
Level 1: Logical OR (or)
```

This is defined in `Constants.cs` and used throughout the parsing pipeline.

### The Shunting Yard Algorithm

Converts infix to postfix notation using two key data structures:
- **Output queue**: Stores tokens in postfix order
- **Operator stack**: Temporarily holds operators based on precedence

**Example transformation:**
```
Infix:   2 + 3 * 4
Postfix: 2 3 4 * +

Evaluation of postfix:
  2 3 4 * +
  → 2 (3 * 4) +
  → 2 12 +
  → 14
```

### Expression Tree Structure

For the expression `(2 + 3) * 4`:

```
      *
     / \
    +   4
   / \
  2   3
```

The tree is evaluated recursively:
1. Evaluate left subtree: `2 + 3 = 5`
2. Evaluate right subtree: `4`
3. Apply operator: `5 * 4 = 14`

## Project Structure

```
Evalr/
├── src/
│   ├── Evalr.API/              # AWS Lambda handler
│   │   ├── LambdaHandler.cs    # API Gateway integration
│   │   └── Models.cs           # Request/response DTOs
│   └── Evalr.Core/             # Core evaluation engine
│       ├── EvalrEngine.cs      # Main orchestrator
│       ├── Tokenizer.cs        # Lexical analysis
│       ├── TokenValidator.cs   # Syntax validation
│       ├── NormalizeTokens.cs  # Unicode handling
│       ├── Classifier.cs       # Expression classification
│       ├── ShuntingYard.cs     # Infix to postfix
│       ├── ExpressionTree.cs   # AST construction
│       ├── Evaluator.cs        # Tree evaluation
│       └── Constants.cs        # Operator definitions
├── tests/
│   └── Evalr.Tests/
├── Dockerfile                  # Container definition
└── docker-compose.yml          # Local development
```

## Running the Project

### Local Testing with Docker

```bash
# Start the container
docker-compose up --build

# In another terminal, run tests
./src/scripts/test-lambda.sh
```

### Testing Individual Expressions

```bash
# Simple arithmetic
curl -X POST http://localhost:9000/2015-03-31/functions/function/invocations \
  -d '{"path":"/evaluate","httpMethod":"POST","body":"{\"expression\":\"2 + 3 * 4\"}"}'

# With variables
curl -X POST http://localhost:9000/2015-03-31/functions/function/invocations \
  -d '{"path":"/evaluate","httpMethod":"POST","body":"{\"expression\":\"x^2 + y\",\"variables\":{\"x\":5,\"y\":3}}"}'

# Boolean expression
curl -X POST http://localhost:9000/2015-03-31/functions/function/invocations \
  -d '{"path":"/evaluate","httpMethod":"POST","body":"{\"expression\":\"5 > 3 and 2 < 4\"}"}'
```

## Key Challenges Solved

1. **Unary vs Binary Operators**: Distinguishing between `-` as subtraction vs negation
2. **Operator Precedence**: Implementing correct order of operations across arithmetic, comparison, and logical operators
3. **Parentheses Handling**: Proper grouping and precedence override
4. **Error Messages**: Providing helpful feedback for syntax errors
5. **Type Handling**: Representing boolean results as numeric values (0.0/1.0)

## Interesting Implementation Details

### Unary Operator Detection

The algorithm determines if `+` or `-` is unary by checking:
- Is it the first token in the expression?
- Is it preceded by `(` or another operator?

If yes, it's converted to `UNARY_PLUS` or `UNARY_MINUS` internally.

### Boolean to Numeric Conversion

Boolean expressions evaluate to doubles:
- `true` → `1.0`
- `false` → `0.0`

This allows mixing arithmetic and boolean operations while maintaining type consistency.

### Floating Point Comparison

Equality checking uses epsilon comparison to handle floating-point precision:
```csharp
Math.Abs(a - b) < EPSILON  // Instead of a == b
```