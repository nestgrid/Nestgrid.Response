#!/usr/bin/env bash

set -euo pipefail

if [ "$#" -ne 2 ]; then
  echo "Usage: ./scripts/verify-packages.sh <artifacts-directory> <version>"
  exit 1
fi

ARTIFACTS_DIRECTORY="$1"
EXPECTED_VERSION="$2"
ARTIFACTS_SOURCE="$(cd "${ARTIFACTS_DIRECTORY}" && pwd)"

packages=(
  Nestgrid.Response
  Nestgrid.Response.Http
  Nestgrid.Response.AspNetCore
  Nestgrid.Response.Mvc
  Nestgrid.Response.Extensions.Validation
)

for package in "${packages[@]}"; do
  nupkg="${ARTIFACTS_DIRECTORY}/${package}.${EXPECTED_VERSION}.nupkg"
  snupkg="${ARTIFACTS_DIRECTORY}/${package}.${EXPECTED_VERSION}.snupkg"

  if [ ! -f "${nupkg}" ]; then
    echo "Missing package: ${nupkg}"
    exit 1
  fi

  if [ ! -f "${snupkg}" ]; then
    echo "Missing symbol package: ${snupkg}"
    exit 1
  fi

  if ! unzip -l "${nupkg}" | grep '^.*README.md$' >/dev/null; then
    echo "Package README is missing from: ${nupkg}"
    exit 1
  fi
done

consumer_directory="$(mktemp -d)"
trap 'rm -rf "${consumer_directory}"' EXIT
export NUGET_PACKAGES="${consumer_directory}/packages"

dotnet new console --framework net8.0 --output "${consumer_directory}" --no-restore >/dev/null
dotnet new nugetconfig --output "${consumer_directory}" --force >/dev/null
dotnet nuget add source "${ARTIFACTS_SOURCE}" \
  --name nestgrid-response-artifacts \
  --configfile "${consumer_directory}/NuGet.Config" >/dev/null
for package in "${packages[@]}"; do
  dotnet add "${consumer_directory}" package "${package}" \
    --version "${EXPECTED_VERSION}" \
    --no-restore >/dev/null
done

dotnet restore "${consumer_directory}" \
  --configfile "${consumer_directory}/NuGet.Config" \
  --nologo

dotnet build "${consumer_directory}" --configuration Release --no-restore --nologo

echo "Package contents and consumer installation verified for version ${EXPECTED_VERSION}."
