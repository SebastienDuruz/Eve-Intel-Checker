#!/bin/bash

projects=("EveIntelChecker" "EveIntelCheckerLib" "EveIntelCheckerPages")

echo "Clean..."

for project in "${projects[@]}"; do
  if [ -d "$project" ]; then
    echo "Clean the project : $project"

    find "$project" -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} +

    echo "✔ $project clear."
  else
    echo "❌ Folder not found : $project"
  fi
done

dotnet restore

echo "Finish."