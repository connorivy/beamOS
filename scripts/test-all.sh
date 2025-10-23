#!/bin/bash
# test-all.sh: Run all tests in the beamOS repo, using the correct method for each test project.
set -e

"${BEAMOS_ROOT}/scripts/test-webapp.sh"

dotnet test ${BEAMOS_ROOT}/BeamOs.NonWebTests.slnf

echo "All tests completed."
