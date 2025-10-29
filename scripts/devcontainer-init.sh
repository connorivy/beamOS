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

npm ci --prefix ./src/WebApp/beamos-webapp

# Set up local NuGet feed
mkdir -p /workspaces/beamOS/.nuget-local
dotnet nuget add source /workspaces/beamOS/.nuget-local --name local

# build webapp integration tests
dotnet build ./tests/BeamOs.Tests.WebApp.Integration/BeamOs.Tests.WebApp.Integration.csproj

# install powershell
./scripts/installPwsh.sh

pwsh ./tests/BeamOs.Tests.WebApp.Integration/bin/Debug/net9.0/playwright.ps1 install --with-deps --only-shell chromium

python3 -m venv venv
source venv/bin/activate

# Create or upgrade venv with latest pip/setuptools
python3 -m venv /workspaces/beamOS/venv --upgrade-deps
source /workspaces/beamOS/venv/bin/activate
# pip install --upgrade pip setuptools

# # Install requirements if requirements.txt exists
# if [ -f /workspaces/beamOS/requirements.txt ]; then
# 	pip install -r /workspaces/beamOS/requirements.txt
# fi

# The container creation script is executed in a new Bash instance
# so we exit at the end to avoid the creation process lingering.
exit
