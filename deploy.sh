#!/usr/bin/env bash
# Build WebGL headlessly and push to itch and GitHub Pages. Close the Unity editor first (it locks the project).
# Usage: ./deploy.sh            build + push
#        ./deploy.sh --no-build push existing WebBuild
set -e
cd "$(dirname "$0")"

ITCH_TARGET="acgamefolder/mr-ninja-mojo-jojos-ambush:html5"   # e.g. karlndebele/ninja-mojo-jojos-ambush:html5
UNITY="/x/Program Files/Unity/6000.0.23f1/Editor/Unity.exe"
BUTLER="$LOCALAPPDATA/butler/butler.exe"

if [ "$1" != "--no-build" ]; then
  echo "Building WebGL (takes a few minutes)..."
  "$UNITY" -batchmode -quit -projectPath "$(pwd -W)" -buildTarget WebGL \
    -executeMethod WebBuilder.Build -logFile build.log \
    || { echo "Build failed, see build.log"; exit 1; }
fi

"$BUTLER" push WebBuild "$ITCH_TARGET"

# Also publish to GitHub Pages (gh-pages branch), which the Amanzi site embeds.
PAGES=$(mktemp -d)
cp -r WebBuild/. "$PAGES"
git -C "$PAGES" init -q -b gh-pages
git -C "$PAGES" add -A
git -C "$PAGES" commit -q -m "WebGL build $(git rev-parse --short HEAD)"
git -C "$PAGES" push -q -f "$(git remote get-url origin)" gh-pages
rm -rf "$PAGES"
