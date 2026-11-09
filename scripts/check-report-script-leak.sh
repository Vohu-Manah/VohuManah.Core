#!/usr/bin/env bash

set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
RESOURCES="$ROOT/src/Infrastructure/Reports/Html/Resources"

if [ ! -d "$RESOURCES" ]; then
    exit 0
fi

bad=0
shopt -s nullglob
for f in "$RESOURCES"/*.js; do
    if grep -En '<[a-zA-Z][^>]*>' "$f" >/dev/null 2>&1; then
        echo "leak: HTML tag found in $f"
        bad=1
    fi
done

if [ "$bad" -ne 0 ]; then
    echo ""
    echo "Fix: remove HTML tags from src/Infrastructure/Reports/Html/Resources/*.js"
    exit 1
fi

exit 0
