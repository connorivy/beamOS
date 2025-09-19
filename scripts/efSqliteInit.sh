#!/bin/bash

cd $BEAMOS_ROOT/src/StructuralAnalysis/BeamOs.StructuralAnalysis.Api

dotnet build -p:BeamOsUseSqlite=true -p:BeamOsUsePostgres=false

if [ $? -ne 0 ]; then
    echo "Sqlite migration creation failed. Exiting."
    exit 1
fi

# Rename existing files that end with Snapshot.cs to Snapshot.cs.copy to avoid conflicts
find ../BeamOs.StructuralAnalysis.Infrastructure/Migrations -name '*Snapshot.cs' -exec bash -c 'mv "$0" "${0}.copy"' {} \;

dotnet ef migrations add InitialCreateSqlite --no-build -p ../BeamOs.StructuralAnalysis.Infrastructure/BeamOs.StructuralAnalysis.Infrastructure.csproj -o Migrations/Sqlite --context StructuralAnalysisDbContext

dotnet build -p:BeamOsUseSqlite=true -p:BeamOsUsePostgres=false

dotnet ef migrations script --no-build -p ../BeamOs.StructuralAnalysis.Infrastructure/BeamOs.StructuralAnalysis.Infrastructure.csproj -o createSqliteDb.sql --context StructuralAnalysisDbContext

# Delete the generated migration files
rm ../BeamOs.StructuralAnalysis.Infrastructure/Migrations/Sqlite/*.cs

# Restore the original snapshot files
find ../BeamOs.StructuralAnalysis.Infrastructure/Migrations -name '*Snapshot.cs.copy' -exec bash -c 'mv "$0" "${0%.copy}"' {} \;