# StateCraft

## CSharpier

This project uses [CSharpier](https://csharpier.com/) to format code.
A [pre-commit](https://pre-commit.com/) script runs before commiting code to ensure the code formatting adheres to the
project's style.
In order to use CSharpier some steps must be taken:

1. Install CSharpier using: `dotnet tool install csharpier`
2. Install python from https://www.python.org/downloads/
3. Install `pre-commit` (version 25.3 has been tested) by running `pip install pre-commit`
4. Install the hooks using `pre-commit install`
5. Optionally: run the pre-commit checks using `pre-commit run --all-files`
    ```
    X:\source\Common>pre-commit run --all-files
    Install .NET tools.......................................................Passed
    Run CSharpier on C# files................................................Passed
    ```
6. Optionally: Install the CSharpier plugin from https://csharpier.com/docs/Editors and set a keyboard shortcut
7. All done! Before committing code you will be notified if there are any formatting issues and CSharpier will fix them
