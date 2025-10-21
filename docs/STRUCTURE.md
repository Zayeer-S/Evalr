Expression-Evaluator/
├── .github/                                # CI/CD
│   └── workflows/
│       └── dotnet.yml
├── src/
│   └── expression_evaluator/
│       ├── Classifier.cs                   # Determines whether input is boolean or arithmetic
│       ├── Constants.cs                    # Dictionaries containing normalization and precedence heirarchies
│       ├── Evaluator.cs                    # Evaluates tree, calculating the result
│       ├── ExpressionTree.cs               # Builds the tree using postfix from ShuntingYard
│       ├── NormalizeTokens.cs              # Converts any unicode characters in tokens into ASCII
│       ├── OutputFormatter.cs              # Prints results and information used for calculations
│       ├── Parser.cs                       # Parses user input
│       ├── Program.cs                      # Console App and orchestrator
│       ├── ShuntingYard.cs                 # Converts infix notation into postfix notation
│       ├── Tokenizer.cs                    # Tokenizes user input
│       ├── TokenValidator.cs               # Validates tokens
│       └── VariableInput.cs                # Gets variable values and reprompts user if they want to try different variable values
├── tests/                                  # Unit tests
│   └── mock_evaluator/
│       ├── mock_evaluator.csproj
│       └── UnitTest1.cs
├── .gitignore                              # Files to exclude from version control
└── README.md                               # Project overview and setup guide