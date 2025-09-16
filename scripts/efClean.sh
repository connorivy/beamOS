#!/bin/bash

cd $BEAMOS_ROOT/src/StructuralAnalysis/BeamOs.StructuralAnalysis.Api 
dotnet build -p:BeamOsUseSqlite=true -p:BeamOsUsePostgres=false
dotnet ef dbcontext optimize --output-dir ../BeamOs.StructuralAnalysis.Infrastructure/CompiledModels/Sqlite --nativeaot --precompile-queries --no-build 

TARGET_DIR="$BEAMOS_ROOT/src/StructuralAnalysis/BeamOs.StructuralAnalysis.Infrastructure/CompiledModels/Sqlite"
find "$TARGET_DIR" -type f -name "*.cs" | while read -r file; do
    # Add #if Sqlite to the top and #endif to the bottom of each .cs file
    sed -i '1i#if Sqlite' "$file"
    echo "#endif" >> "$file"
    echo "Wrapped with #if Sqlite: $file"
done
# Script to find all *Accessors.cs files in CompiledModels and change 'public static class' to 'internal static class'
find "$TARGET_DIR" -type f -name "*Accessors.cs" | while read -r file; do
    if grep -q "public static class" "$file"; then
        sed -i 's/public static class/internal static class/g' "$file"
        echo "Updated: $file"
    fi
done

dotnet build -p:BeamOsUseSqlite=false -p:BeamOsUsePostgres=true
dotnet ef dbcontext optimize --output-dir ../BeamOs.StructuralAnalysis.Infrastructure/CompiledModels/Postgres --nativeaot --precompile-queries --no-build

TARGET_DIR="$BEAMOS_ROOT/src/StructuralAnalysis/BeamOs.StructuralAnalysis.Infrastructure/CompiledModels/Postgres"
find "$TARGET_DIR" -type f -name "*.cs" | while read -r file; do
    # Add #if Postgres to the top and #endif to the bottom of each .cs file
    sed -i '1i#if Postgres' "$file"
    echo "#endif" >> "$file"
    echo "Wrapped with #if Postgres: $file"
done

find "$TARGET_DIR" -type f -name "*Accessors.cs" | while read -r file; do
    if grep -q "public static class" "$file"; then
        sed -i 's/public static class/internal static class/g' "$file"
        echo "Updated: $file"
    fi
done
