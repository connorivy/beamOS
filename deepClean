#!/bin/bash
# Recursively delete all bin and obj folders, excluding those in node_modules

find . \( -path '*/node_modules/*' -prune \) -o \( -type d \( -name bin -o -name obj \) -print \) | xargs rm -rf
