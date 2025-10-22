#!/bin/bash
# Run dotnet test for BeamOs.Tests.WebApp.Integration under xvfb with forwarded arguments

if [ -z "$BEAMOS_ROOT" ]; then
	echo "Error: BEAMOS_ROOT environment variable is not set."
	exit 1
fi

PROJECT_PATH="$BEAMOS_ROOT/tests/BeamOs.Tests.WebApp.Integration/BeamOs.Tests.WebApp.Integration.csproj"
xvfb-run --auto-servernum --server-args='-screen 0 1280x1024x24' dotnet test "$PROJECT_PATH" "$@"
