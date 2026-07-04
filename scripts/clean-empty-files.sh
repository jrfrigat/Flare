#!/usr/bin/env bash
# Remove empty (0-byte) UNTRACKED files from the working tree.
#
# These are junk left by stray shell redirects (files named ",", "0", "errors", etc.).
# Safe by construction: only files that git does NOT track AND are exactly 0 bytes are
# removed. Tracked files, non-empty files, and git-ignored build output (bin/obj) are
# never touched. Run from anywhere inside the repo; run before every commit.
set -euo pipefail

root="$(git rev-parse --show-toplevel)"
cd "$root"

removed=0
while IFS= read -r -d '' f; do
  if [ -f "$f" ] && [ ! -s "$f" ]; then
    rm -f -- "$f"
    echo "removed empty file: $f"
    removed=$((removed + 1))
  fi
done < <(git ls-files --others --exclude-standard -z)

echo "clean-empty-files: removed $removed empty untracked file(s)"
