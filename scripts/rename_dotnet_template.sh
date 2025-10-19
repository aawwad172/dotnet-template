#!/bin/bash
# rename_dotnet_template.sh
#
# Usage:
#   ./rename_dotnet_template.sh NewName
#
# This script renames all occurrences of "Dotnet.Template" in:
#   1. File names (base name only; directory names remain for later processing)
#   2. Directory names (deepest first)
#   3. File contents (text replacements)
#   4. The .sln file (with lowercase and '.' replaced by '-')

if [ "$#" -ne 1 ]; then
  echo "Usage: $0 NewName"
  exit 1
fi

OLD="Dotnet.Template"
NEW="$1"

echo "Replacing '$OLD' with '$NEW' throughout the project..."

########################################
# Rename Files (base name only)
########################################
echo "Renaming files..."
find . -type f -name "*${OLD}*" | while read -r file; do
  dir=$(dirname "$file")
  base=$(basename "$file")
  newbase=$(echo "$base" | sed "s/${OLD}/${NEW}/g")
  newfile="${dir}/${newbase}"
  if [ "$file" != "$newfile" ]; then
    echo "Renaming file: '$file' -> '$newfile'"
    mv "$file" "$newfile"
  fi
done

########################################
# Rename Directories (deepest first)
########################################
echo "Renaming directories..."
find . -depth -type d -name "*${OLD}*" | while read -r dir; do
  newdir=$(echo "$dir" | sed "s/${OLD}/${NEW}/g")
  if [ "$dir" != "$newdir" ]; then
    echo "Renaming directory: '$dir' -> '$newdir'"
    mv "$dir" "$newdir"
  fi
done

########################################
# Rename the .sln file
########################################
slnNewName=$(echo "$NEW" | tr '[:upper:]' '[:lower:]' | sed 's/\./-/g')

if [ -f "./dotnet-template.sln" ]; then
  echo "Renaming dotnet-template.sln to ${slnNewName}.sln"
  mv "./dotnet-template.sln" "./${slnNewName}.sln"
fi

########################################
# Replace text inside files
########################################
echo "Replacing text in file contents..."
find . -type f | while read -r file; do
  if file "$file" | grep -qE 'text|XML|JSON'; then
    echo "Processing file: '$file'"
    sed -i "s/${OLD}/${NEW}/g" "$file"
  fi
done

echo "Renaming complete."
