Expression-Evaluator/
├── .config/    
│   └── dotnet-tools.json
├── .github/                                # CI/CD
│   └── workflows/
│       └── dotnet.yml
├── .husky/       
│   ├── pre-commit
│   └── task-runner.json
├── docs/                                   
│   └── STRUCTURE.md
├── src/
│   ├── Evalr.API/
│   │   ├──
│   └── Evalr.Core/
│       ├── Classifier.cs                   # Determines whether input is boolean or arithmetic
│       ├── Constants.cs                    # Dictionaries containing normalization and precedence heirarchies
│       ├── Evalr.Core.csproj
│       ├── EvalrEngine.cs
│       ├── Evaluator.cs                    # Evaluates tree, calculating the result
│       ├── ExpressionTree.cs               # Builds the tree using postfix from ShuntingYard
│       ├── NormalizeTokens.cs              # Converts any unicode characters in tokens into ASCII
│       ├── ShuntingYard.cs                 # Converts infix notation into postfix notation
│       ├── Tokenizer.cs                    # Tokenizes user input
│       └── TokenValidator.cs               # Validates tokens
├── tests/                                  # Unit tests
│   └── Evalr.Tests/
│       ├── EvalrTests.csproj
│       └── UnitTest1.cs
├── .editorconfig
├── .gitignore                              # Files to exclude from version control
└── README.md                               # Project overview and setup guide