# Contributing

## How to Fork and Clone

1. Fork the repository on GitHub.
2. Clone your fork locally:

```
git clone https://github.com/YOUR_USERNAME/SVY_Traverse.git
cd SVY_Traverse
```

3. Add the upstream remote to stay in sync:

```
git remote add upstream https://github.com/ORIGINAL_OWNER/SVY_Traverse.git
```

## Setting Up the Development Environment

- Install [.NET SDK 10.0](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) or later.
- Build the project:

```
dotnet build
```

- Run the application:

```
dotnet run
```

## Running Tests

This project does not currently have an automated test suite. If you add tests, place them in a test project at the repository root and run:

```
dotnet test
```

## Code Style Expectations

- `Option Strict On` and `Option Explicit On` must be set at the top of every `.vb` file.
- XML doc comments (`''' <summary>`) are required on all public types and members.
- No emojis in code, comments, or commit messages.
- Follow the existing naming conventions: PascalCase for types and methods, camelCase for local variables, descriptive names.
- Keep the codebase clean of hardcoded personal paths, secrets, or environment-specific configuration.

## How to Submit a Pull Request

1. Create a feature branch from `main`: `git checkout -b my-feature`
2. Make your changes, ensuring the build succeeds (`dotnet build`).
3. Commit with a clear, descriptive message (no emojis).
4. Push to your fork: `git push origin my-feature`
5. Open a pull request against the `main` branch of the original repository.
6. Fill out the pull request template with all relevant details.

Please see [PULL_REQUEST_TEMPLATE.md](.github/PULL_REQUEST_TEMPLATE.md) for the required pull request format.
