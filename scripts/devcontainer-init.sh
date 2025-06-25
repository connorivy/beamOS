#!/bin/bash
# This script runs after the dev container is created to set up the workspace.

set -e

# Make sure deepClean is executable and run it
chmod +x /workspaces/beamOS/deepClean
/workspaces/beamOS/deepClean

dotnet tool restore

# Restore .NET dependencies
cd /workspaces/beamOS
dotnet restore

# Install npm dependencies for BeamOs.WebApp.Components
cd /workspaces/beamOS/src/WebApp/BeamOs.WebApp.Components
npm install
