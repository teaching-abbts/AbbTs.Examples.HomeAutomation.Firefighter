#!/usr/bin/env bash
set -euo pipefail

action="start"
mode="local"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --action|-a)
      action="${2:-}"
      shift 2
      ;;
    --mode|-m)
      mode="${2:-}"
      shift 2
      ;;
    *)
      echo "Unknown argument: $1" >&2
      exit 1
      ;;
  esac
done

case "$mode:$action" in
  local:start) target="Run-Start" ;;
  local:stop) target="Run-Stop" ;;
  local:status) target="Run-Status" ;;
  artifacts:start) target="Artifacts-Run-Start" ;;
  artifacts:stop) target="Artifacts-Run-Stop" ;;
  artifacts:status) target="Artifacts-Run-Status" ;;
  *)
    echo "Unsupported mode/action combination: $mode/$action" >&2
    exit 1
    ;;
esac

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
if [[ -f "$script_dir/build/Build.csproj" ]]; then
  repo_root="$script_dir"
elif [[ -f "$script_dir/../build/Build.csproj" ]]; then
  repo_root="$(cd "$script_dir/.." && pwd)"
else
  echo "Could not locate build/Build.csproj from '$script_dir'." >&2
  exit 1
fi

build_project="$repo_root/build/Build.csproj"
dotnet run --project "$build_project" --target "$target" -- --repo-root "$repo_root"
