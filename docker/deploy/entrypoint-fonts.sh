#!/bin/sh
set -e
mkdir -p /usr/share/fonts/truetype/custom
if [ -d /opt/vohumanah-fonts ]; then
  cp -f /opt/vohumanah-fonts/*.ttf /usr/share/fonts/truetype/custom/ 2>/dev/null || true
fi
if [ -d /fonts-extra ]; then
  cp -f /fonts-extra/*.ttf /usr/share/fonts/truetype/custom/ 2>/dev/null || true
fi
fc-cache -f >/dev/null 2>&1 || true
exec dotnet VohuManah.Web.Api.dll
