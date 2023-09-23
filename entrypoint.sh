#!/bin/bash
set -e

echo "Exec the container's main process (what's set as CMD in the Dockerfile)."
exec "$@"
