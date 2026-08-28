#!/usr/bin/env bash

set -euo pipefail

if [ "$#" -ne 2 ]; then
  echo "Usage: ./scripts/verify-packages.sh <artifacts-directory> <version>"
  exit 1
fi

ARTIFACTS_DIRECTORY="$1"
EXPECTED_VERSION="$2"
ARTIFACTS_SOURCE="$(cd "${ARTIFACTS_DIRECTORY}" && pwd)"

echo "Verifying packages from artifacts directory: ${ARTIFACTS_SOURCE}"
echo "Expected package version: ${EXPECTED_VERSION}"

packages=(
  Nestgrid.Response
  Nestgrid.Response.Http
  Nestgrid.Response.Http.Client
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

echo "Creating temporary consumer project: ${consumer_directory}"
dotnet new console --framework net8.0 --output "${consumer_directory}" --no-restore >/dev/null

echo "Creating temporary NuGet configuration"
dotnet new nugetconfig --output "${consumer_directory}" --force >/dev/null

nuget_config="${consumer_directory}/nuget.config"

echo "Configuring local package source: ${ARTIFACTS_SOURCE}"
dotnet nuget add source "${ARTIFACTS_SOURCE}" \
  --name nestgrid-response-artifacts \
  --configfile "${nuget_config}" >/dev/null

echo "Adding package references to temporary consumer"
for package in "${packages[@]}"; do
  dotnet add "${consumer_directory}" package "${package}" \
    --version "${EXPECTED_VERSION}" \
    --no-restore >/dev/null
done

echo "Restoring temporary consumer from configured package sources"
dotnet restore "${consumer_directory}" \
  --configfile "${nuget_config}" \
  --nologo

echo "Building temporary consumer"
dotnet build "${consumer_directory}" --configuration Release --no-restore --nologo

echo "Package contents and consumer installation verified for version ${EXPECTED_VERSION}."
