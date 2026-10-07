#! /bin/sh
echo "Class and enum overview" \n > ./tmp/overview.md
find ./src -type d -name "obj" -prune -o -type f -name '*.cs' -exec scripts/write_overview_entry.sh {} \;
