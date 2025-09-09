#!/bin/bash
# This script runs after the dev container is created to set up the workspace.

set -e

# Make sure deepClean is executable and run it
chmod +x ./scripts/deepClean.sh
./scripts/deepClean.sh

dotnet tool restore

# Restore .NET dependencies
dotnet restore

# Set up local NuGet feed
mkdir -p /workspaces/beamOS/.nuget-local
dotnet nuget add source /workspaces/beamOS/.nuget-local --name local