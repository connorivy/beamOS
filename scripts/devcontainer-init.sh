#!/bin/bash
# This script runs after the dev container is created to set up the workspace.

set -e

dotnet dev-certs https --trust

# Make sure all scripts are executable and run deepClean
chmod +x ./scripts/*.sh
./scripts/deepClean.sh

dotnet tool restore

# Restore .NET dependencies
dotnet restore

# Set up local NuGet feed
mkdir -p /workspaces/beamOS/.nuget-local
dotnet nuget add source /workspaces/beamOS/.nuget-local --name local

python3 -m venv venv
source venv/bin/activate
# pip install --upgrade pip
# pip install -r requirements.txt

# The container creation script is executed in a new Bash instance
# so we exit at the end to avoid the creation process lingering.
exit