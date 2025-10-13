# beamOS - Copilot Agent Instructions

## Project Overview

**beamOS** is an open-source, test-first structural analysis program written in C# with a Blazor WebAssembly frontend. It implements the Direct Stiffness Method for structural analysis and provides a web-based interface for creating and analyzing structural models.

### Technology Stack
- **Backend**: .NET 9.0 with C# and ASP.NET Core
- **Frontend**: Blazor Server Components with TypeScript/JavaScript
- **Database**: PostgreSQL (default), SQLite (optional), In-Memory (testing)
- **Testing**: TUnit and Playwright (integration tests)
- **Build Tools**: dotnet CLI, npm, and Rollup.js
- **Code Quality**: CSharpier (formatting) and Stryker.NET (mutation testing)

### Repository Structure
- **Size**: Large multi-project solution (~50+ projects)
- **Architecture**: Clean Architecture with Domain-Driven Design
- **Primary Language**: C# (~95%), TypeScript/JavaScript (~5%)

## Critical Build Requirements

### Prerequisites
- .NET 9.0 SDK (verify with `dotnet --version`)
- Node.js >= 20 (verify with `node --version`)
- Git

### Essential Build Order
**ALWAYS follow this exact sequence to avoid build failures:**

1. **First-time setup or after clean:**
   ```bash
   # Install .NET tools
   dotnet tool restore

   # Install Node.js dependencies
   cd src/WebApp/BeamOs.WebApp.Components
   npm install
   ```

2. **Build JavaScript components FIRST:**
   ```bash
   # CRITICAL: Must run before any .NET build
   cd src/WebApp/BeamOs.WebApp.Components
   npm run build
   ```

3. **Build .NET projects in Release mode:**
   ```bash
   cd /workspaces/beamOS
   dotnet build -c Release
   ```

### Common Build Issues & Solutions

**Error: BLAZOR106 - JS module file not found**
- **Cause**: JavaScript components not built before .NET build
- **Solution**: Run `npm run build` in `src/WebApp/BeamOs.WebApp.Components` first

**Error: Cannot find referenced project**
- **Cause**: Missing project references or clean build state
- **Solution**: Run `dotnet restore` then rebuild

**Error: Assembly conflicts with GenerateServiceRegistrationsAttribute**
- **Cause**: Code generation conflicts
- **Solution**: This is expected during code generation builds; warnings can be ignored

## Testing & Validation

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/StructuralAnalysis/BeamOs.Tests.StructuralAnalysis.Unit/

# Test filtering
dotnet test -- --treenode-filter /AssemblyName/Namespace/ClassName/TestMethodName/
# Test filtering example (run tests with 'Beam' in name)
dotnet test -- --treenode-filter /*/*/*/*Beam*

# Run tests with coverage
dotnet-coverage collect "dotnet test" -f cobertura -s ./CodeCov.runsettings
```

### Code Formatting
```bash
# Check formatting (WILL show many violations - project uses custom formatting)
dotnet csharpier --check .

# Auto-fix formatting
dotnet csharpier .
```

**Note**: The codebase has extensive formatting violations that are being gradually addressed. Format your code changes but don't be concerned about existing violations.

### Integration Tests
Some integration tests require additional setup:
- Playwright browsers: `npx playwright install --with-deps`
- PostgreSQL container (handled automatically in CI)

## Project Architecture

### Core Domain Areas
1. **StructuralAnalysis** (`src/StructuralAnalysis/`) - Core structural analysis engine
   - `BeamOs.StructuralAnalysis.Core` - Domain models and business logic
   - `BeamOs.StructuralAnalysis.Api` - REST API endpoints
   - `BeamOs.StructuralAnalysis.Infrastructure` - Data persistence
   - `BeamOs.StructuralAnalysis.Contracts` - DTOs and interfaces

2. **WebApp** (`src/WebApp/`) - User interface
   - `BeamOs.WebApp` - Main web application
   - `BeamOs.WebApp.Components` - Blazor components and UI logic

3. **Common** (`src/Common/`) - Shared utilities and base classes
4. **Identity** (`src/Identity/`) - User authentication and authorization
5. **AI** (`src/Ai/`) - AI-powered features
6. **SpeckleConnector** (`src/SpeckleConnector/`) - Speckle 3D integration

### Code Generation
- **Location**: `codeGen/` directory
- **Purpose**: API client generation, test model builders
- **Usage**: Run `./scripts/codegen.sh` to regenerate APIs

### Configuration Files
- `global.json` - .NET SDK version (9.0.0)
- `Directory.Build.props` - Global MSBuild properties
- `Directory.Packages.props` - Centralized package management
- `.editorconfig` - Code style rules (extensive formatting rules)
- `CodeCov.runsettings` - Test coverage configuration
- `stryker-config.json` - Mutation testing configuration

## Database Configuration

The project uses compile-time flags to select database provider:
- **PostgreSQL**: Default (`BeamOsUsePostgres=true`)
- **SQLite**: Alternative (`BeamOsUseSqlite=true`)
- **In-Memory**: Testing only (`BeamOsUseInMemoryInfra=true`)

Database configuration is in `Directory.Build.props` via MSBuild properties.

## Development Workflow

### Making Changes
1. Create feature branch from `main`
2. Build JavaScript components: `npm run build` in WebApp.Components
3. Build solution: `dotnet build -c Release`
4. Run relevant tests: `dotnet test [specific-test-project]`
5. Check formatting: `dotnet csharpier .`

### Key Patterns & Conventions
- **File-scoped namespaces**: All C# files use file-scoped namespace declarations
- **Primary constructors**: Preferred for simple classes
- **Expression-bodied members**: Used extensively
- **Clean Architecture**: Commands, queries, handlers pattern
- **Domain-Driven Design**: Aggregates, entities, value objects

### Important Notes
- The codebase is work-in-progress with many TODO comments and warnings
- Test failures in architecture tests are expected due to missing project references
- Some warning suppressions are intentional (see Directory.Build.props)
- Mutation testing with Stryker can take ~15 minutes to complete

## CI/CD Pipeline

### GitHub Actions
- **Build/Test** (`.github/workflows/build.yml`): Runs on PRs
  - Uses dev container for consistent environment
  - Runs tests with coverage collection
  - Checks code formatting with CSharpier
  - Publishes coverage to Coveralls

- **Release** (`.github/workflows/release.yml`): NuGet package publishing
- **Copilot Setup** (`.github/workflows/copilot-setup-steps.yml`): Environment preparation

### Dev Container
The project includes dev container configuration with pre-installed tools:
- .NET 9 SDK
- Node.js 22
- Python with MKL
- PowerShell
- Playwright browsers

## Troubleshooting

### Build Failures
1. **Check Node.js version**: Must be >= 20
2. **Clean state**: Run `./scripts/deepClean.sh` to remove all bin/obj folders
3. **Tool restoration**: Run `dotnet tool restore`
4. **JavaScript build**: Always run `npm run build` before .NET build

### Test Failures
- Architecture tests may fail due to project structure changes (expected)
- Integration tests require Docker for database containers
- Some tests depend on specific data seeding

### Performance
- Release builds are significantly faster than Debug
- Mutation testing is resource-intensive (limit concurrency if needed)
- Consider using `--no-restore` and `--no-build` flags for repeated test runs

**Trust these instructions**: They are based on comprehensive analysis of the actual codebase, build failures, and CI configuration. Only search for additional information if these instructions prove insufficient o