#!/bin/bash
# This script runs after the dev container is created to set up the workspace.

set -e

dotnet dev-certs https --trust

cd $BEAMOS_ROOT

# Make sure all scripts are executable and run deepClean
chmod +x $BEAMOS_ROOT/scripts/*.sh
./scripts/deepClean.sh

dotnet tool restore

# Restore .NET dependencies
dotnet restore

npm ci --prefix $BEAMOS_ROOT/src/WebApp/beamos-webapp

# Set up local NuGet feed
mkdir -p $BEAMOS_ROOT/.nuget-local
dotnet nuget add source $BEAMOS_ROOT/.nuget-local --name local

# build webapp integration tests
dotnet build $BEAMOS_ROOT/tests/BeamOs.Tests.WebApp.Integration/BeamOs.Tests.WebApp.Integration.csproj

# install powershell
$BEAMOS_ROOT/scripts/installPwsh.sh

pwsh $BEAMOS_ROOT/tests/BeamOs.Tests.WebApp.Integration/bin/Debug/net9.0/playwright.ps1 install --with-deps --only-shell chromium

python3 -m venv venv
source venv/bin/activate

# Create or upgrade venv with latest pip/setuptools
python3 -m venv $BEAMOS_ROOT/venv --upgrade-deps
source $BEAMOS_ROOT/venv/bin/activate
# pip install --upgrade pip setuptools

# # Install requirements if requirements.txt exists
# if [ -f /workspaces/beamOS/requirements.txt ]; then
# 	pip install -r /workspaces/beamOS/requirements.txt
# fi

# The container creation script is executed in a new Bash instance
# so we exit at the end to avoid the creation process lingering.
exit
