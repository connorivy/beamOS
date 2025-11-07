# WebApp Testing Guide

## Overview

This document explains how to run the webapp integration tests for beamOS using the provided test-webapp.sh script.

## Prerequisites

Before running the webapp tests, ensure you have:

1. **Required Tools:**
   - .NET 9.0 SDK
   - Node.js >= 20
   - xvfb (X Virtual Framebuffer) for headless browser testing
   - Playwright browsers installed
   - Docker (for TestContainers/PostgreSQL)

2. **Environment Setup:**
   ```bash
   # Install Playwright browsers
   npx playwright install --with-deps
   
   # Install node dependencies for the webapp
   cd src/WebApp/beamos-webapp
   npm install
   cd ../../..
   ```

## Running the Tests

### Quick Start

The simplest way to run webapp tests is using the provided script:

```bash
export BEAMOS_ROOT=/path/to/beamOS
./scripts/test-webapp.sh
```

### Step-by-Step Process

If you need more control or want to understand what's happening:

1. **Start the WebApp (for e2e tests):**
   ```bash
   cd src/WebApp/beamos-webapp
   npm run dev:e2e &
   ```
   
   This starts the Vite development server on port 3000 with the backend URL configured for testing.

2. **Run the Tests:**
   ```bash
   export BEAMOS_ROOT=/path/to/beamOS
   xvfb-run --auto-servernum --server-args='-screen 0 1280x1024x24' \
     dotnet test tests/BeamOs.Tests.WebApp.Integration/BeamOs.Tests.WebApp.Integration.csproj
   ```

3. **Cleanup:**
   ```bash
   # Stop the webapp server
   pkill -f "vite --port 3000"
   ```

## Test Architecture

The webapp integration tests use several components:

### Backend Setup
- Tests automatically spin up the StructuralAnalysis API on port 7071 using `BlazorApplicationFactory`
- TestContainers manages PostgreSQL instances for database operations
- See `AssemblySetup.cs` for backend initialization

### Frontend Setup
- Tests expect the React webapp to be running on http://localhost:3000
- The webapp is started separately (not managed by the test framework)
- See `test-webapp.sh` for the xvfb-wrapped test execution

### Test Structure
- **Playwright Integration:** Browser automation for UI testing
- **Test Fixtures:** `ReactPageTest` and `BlazorPageTest` base classes
- **Page Extensions:** Helper methods in `PageContextExtensions.cs`

## Understanding Test Results

### Expected Test Results

As of the current codebase state:
- Some tests may fail due to timeout issues
- Check CI/CD pipeline for baseline test success rates
- Skipped tests are intentional (see test attributes)

### Debugging Failed Tests

When tests fail, Playwright generates trace files:

```bash
# View trace for a specific test
npx playwright show-trace ./tests/BeamOs.Tests.WebApp.Integration/bin/Debug/net9.0/trace-{TestName}.zip
```

Common failure reasons:
1. **Navigation Timeouts:** Webapp not fully loaded or backend not responding
2. **Element Not Found:** UI changes or timing issues
3. **TestContainer Issues:** Docker not running or resource constraints

## CI/CD Integration

The GitHub Actions workflow (`.github/workflows/build.yml`) runs these tests:

```yaml
- name: Run tests with coverage
  run: |
    nohup npm --prefix ./src/WebApp/beamos-webapp run dev:e2e &
    dotnet-coverage collect "./scripts/test-all.sh" -f cobertura -s ./CodeCov.runsettings
```

## Troubleshooting

### Issue: "BEAMOS_ROOT environment variable is not set"
**Solution:** Export the BEAMOS_ROOT variable:
```bash
export BEAMOS_ROOT=$(pwd)
```

### Issue: "xvfb-run: command not found"
**Solution:** Install xvfb:
```bash
# Ubuntu/Debian
sudo apt-get install xvfb

# Or run tests without xvfb (requires X server)
dotnet test tests/BeamOs.Tests.WebApp.Integration/BeamOs.Tests.WebApp.Integration.csproj
```

### Issue: "Playwright browser not found"
**Solution:** Install Playwright browsers:
```bash
npx playwright install --with-deps
```

### Issue: Tests timeout waiting for navigation
**Possible causes:**
- Webapp not running on port 3000
- Backend API not responding on port 7071
- Network/firewall issues
- Insufficient timeout values (default: 3000ms)

**Debug steps:**
1. Verify webapp is running: `curl http://localhost:3000`
2. Check backend health: `curl http://localhost:7071/health` (if endpoint exists)
3. Review Playwright traces for detailed error information

### Issue: Docker/TestContainers errors
**Solution:** Ensure Docker is running:
```bash
docker ps
```

## Test Filtering

To run specific tests or test categories:

```bash
# Run specific test class
dotnet test tests/BeamOs.Tests.WebApp.Integration/ --filter "FullyQualifiedName~TutorialTests"

# Run with TUnit tree node filter
dotnet test tests/BeamOs.Tests.WebApp.Integration/ -- --treenode-filter /*/*/TutorialTests/*
```

## Additional Resources

- [Playwright Documentation](https://playwright.dev/dotnet/)
- [TUnit Testing Framework](https://github.com/thomhurst/TUnit)
- [TestContainers Documentation](https://dotnet.testcontainers.org/)
- Repository test instructions: `.github/instructions/Test.instructions.md`

## Contributing

When adding new webapp tests:

1. Extend `ReactPageTest` or `BlazorPageTest` base class
2. Use `PageContext` for Playwright operations
3. Follow existing test patterns in `ModelEditorTests/` and `BimFirstTutorialTests/`
4. Add helper methods to `PageContextExtensions` for reusable UI operations
5. Generate Playwright traces for debugging (automatically done on test failure)

## Notes

- Tests run in parallel by default (may cause resource contention)
- Each test gets its own browser context for isolation
- Database state is managed per test class using TestContainers
- Frontend must be rebuilt if TypeScript/React source files change
