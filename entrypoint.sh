#!/bin/bash
set -e

# Add here whatever want to run before the app start up

echo "Exec the container's main process (what's set as CMD in the Dockerfile)."
exec "$@"
