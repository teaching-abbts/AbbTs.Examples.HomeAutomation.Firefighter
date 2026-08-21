#!/usr/bin/env bash
set -euo pipefail

tilt up --file "$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)/Tiltfile" "$@"
