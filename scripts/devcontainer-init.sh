#!/bin/bash
# This script runs after the dev container is created to set up the workspace.

set -e

# Make sure deepClean is executable and run it
chmod +x ./deepClean
./deepClean

dotnet tool restore

# Restore .NET dependencies
dotnet restore
