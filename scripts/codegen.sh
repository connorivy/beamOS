#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BEAMOS_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

dotnet run --project $BEAMOS_ROOT/codeGen/BeamOs.CodeGen.ApiGenerator/BeamOs.CodeGen.ApiGenerator.csproj /p:CODEGEN=true