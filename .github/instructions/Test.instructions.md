---
applyTo: "**/tests/**"
---

# Test Instructions - beamOS

## Overview

beamOS uses **TUnit** as the primary testing framework across all test projects. The test suite is organized into multiple categories with specific purposes and requirements.

## Test Project Structure

### Core Test Projects
- `BeamOs.Tests.StructuralAnalysis.Unit/` - Core structural analysis unit tests
- `BeamOs.Tests.StructuralAnalysis.Integration/` - Integration tests with database/external services
- `BeamOs.Tests.WebApp.Integration/` - Web application integration tests (Playwright)
- `BeamOs.Tests.Architecture/` - Architecture validation tests
- `BeamOs.Tests.Common/` - Shared test utilities and base classes
- `BeamOs.Tests.Common.Integration/` - Common integration test helpers

### Runtime Test Projects
- `BeamOs.Tests.Runtime.TestRunner/` - Test execution runtime
- `BeamOs.Tests.StructuralAnalysis.Unit.Runtime/` - Runtime-specific unit tests
- `BeamOs.Tests.StructuralAnalysis.Integration.Runtime/` - Runtime-specific integration tests

## Running Tests

### Basic Test Execution

```bash
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test tests/StructuralAnalysis/BeamOs.Tests.StructuralAnalysis.Unit/

# Run tests without building (faster for repeated runs)
dotnet test --no-build

# Run tests in Release mode
dotnet test -c Release
```

### Test Filtering

TUnit uses the `--treenode-filter` flag for test filtering. The syntax is:
`/<Assembly>/<Namespace>/<Class name>/<Test name>`

```bash
# Filter by test class name (run all tests in OctreeTests class)
dotnet test -- --treenode-filter /*/*/OctreeTests/*

# Filter by specific test method
dotnet test -- --treenode-filter /*/*/OctreeTests/InsertedNode_CanBeFoundWithinTolerance

# Filter by partial class name using wildcards
dotnet test -- --treenode-filter /*/*/Octree*/*

# Filter by namespace (run all tests in specific namespace)
dotnet test -- --treenode-filter /*/BeamOs.Tests.StructuralAnalysis.Unit/*/*

# Filter by assembly (run all tests in specific assembly)
dotnet test -- --treenode-filter /BeamOs.Tests.StructuralAnalysis.Unit/*/*/*

# Run all tests containing 'Beam' in the name
dotnet test -- --treenode-filter /*/*/*/*Beam*

# Run all tests containing 'Diagram' in any part
dotnet test -- --treenode-filter /*/*/Diagram*/*
```

### Advanced Filtering Examples

```bash
# Run all tests in multiple classes (OR logic)
dotnet test -- --treenode-filter "/*/*/OctreeTests/* or /*/*/DiagramTests/*"

# Exclude specific tests (NOT logic)
dotnet test -- --treenode-filter "/*/*/*/* and not /*/*/IntegrationTest*/*"

# Filter by test name patterns
dotnet test -- --treenode-filter /*/*/*/*_Should*

# Filter by custom properties (if defined on tests)
dotnet test -- --treenode-filter /*/*/*/*[Category=Unit]

# Complex filters with multiple conditions
dotnet test -- --treenode-filter "/*/*/Octree*/* and /*/*/*/*_CanBeFound*"
```

## Test Categories & Expected Results

### Unit Tests ✅
- **Location**: `BeamOs.Tests.StructuralAnalysis.Unit/`
- **Expected**: Should pass consistently
- **Runtime**: ~1-2 seconds
- **Purpose**: Fast, isolated tests for core logic

### Integration Tests ⚠️
- **Location**: `BeamOs.Tests.StructuralAnalysis.Integration/`
- **Expected**: Should pass but may take longer (~10-15 seconds)
- **Requirements**: Database containers (handled automatically)
- **Purpose**: End-to-end testing with real dependencies

### Architecture Tests ❌ (Expected Failures)
- **Location**: `BeamOs.Tests.Architecture/`
- **Expected**: Some failures are normal due to missing project references
- **Error**: `DirectoryNotFoundException: Could not find a part of the path`
- **Action**: Ignore these specific failures - they're known issues

### Web Integration Tests 🌐
- **Location**: `BeamOs.Tests.WebApp.Integration/`
- **Requirements**: Playwright browsers (`npx playwright install --with-deps`)
- **Runtime**: Longer execution time due to browser automation

## Code Coverage

### Collecting Coverage

```bash
# Basic coverage collection
dotnet-coverage collect "dotnet test" -f cobertura -s ./CodeCov.runsettings

# Coverage for specific project
dotnet-coverage collect "dotnet test tests/StructuralAnalysis/BeamOs.Tests.StructuralAnalysis.Unit/" -f cobertura -s ./CodeCov.runsettings

# Generate coverage report
reportgenerator -reports:"output.cobertura.xml" -targetdir:"./tests/TestResults" -reporttypes:"lcov;html"
```

### Coverage Configuration
- **Settings file**: `CodeCov.runsettings`
- **Excludes**: Non-BeamOS assemblies, generated files (*.g.cs), test projects
- **Includes**: Only BeamOs.* assemblies for focused coverage reporting

## Debugging Tests

### List Tests Without Running

```bash
# List all tests in a project
dotnet test tests/StructuralAnalysis/BeamOs.Tests.StructuralAnalysis.Unit/ --list-tests

# List tests matching filter
dotnet test --list-tests --filter "FullyQualifiedName~Octree"
```

### Verbose Output

```bash
# Detailed test output
dotnet test -v detailed

# Minimal output for faster feedback
dotnet test -v minimal
```

### Test Isolation

```bash
# Run tests in isolated processes (slower but more reliable)
dotnet test --blame

# Collect crash dumps on failures
dotnet test --blame-crash

# Set timeout for hanging tests
dotnet test --blame-hang-timeout 5m
```

## Common Issues & Solutions

### Issue: Tests hang or take too long
**Solution**: Use `--blame-hang-timeout` to identify problematic tests

### Issue: Integration tests fail with database errors
**Solution**: Ensure Docker is running and database containers can start

### Issue: Architecture tests failing
**Solution**: Expected behavior - ignore DirectoryNotFoundException errors

### Issue: Playwright tests fail
**Solution**: Run `npx playwright install --with-deps` to install browser dependencies

### Issue: "No tests found" message
**Solution**: Build the project first with `dotnet build` or remove `--no-build` flag

### CI/CD Considerations
- Architecture test failures are expected and don't indicate problems
- Integration tests require container support
- Coverage collection adds ~2-3x execution time
- Playwright tests need browser installation in CI environments

## Mutation Testing

For comprehensive test quality analysis:

```bash
# Run mutation testing (takes ~15 minutes)
dotnet stryker

# Run mutation testing on specific project
dotnet stryker --project "tests/StructuralAnalysis/BeamOs.Tests.StructuralAnalysis.Unit/BeamOs.Tests.StructuralAnalysis.Unit.csproj"
```

Configuration in `stryker-config.json` includes thresholds and exclusions for generated code.

**Remember**: Focus test execution on relevant test projects to save time during development. Use filtering extensively to run only the tests you need.