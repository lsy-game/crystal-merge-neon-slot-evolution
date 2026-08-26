#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(cd "${SCRIPT_DIR}/.." && pwd)"
UNITY_APP="${UNITY_APP:-/Applications/Unity/Hub/Editor/2022.3.62f3c1/Unity.app/Contents/MacOS/Unity}"

if [[ ! -x "${UNITY_APP}" ]]; then
  echo "Unity executable not found: ${UNITY_APP}" >&2
  exit 1
fi

echo "Building Fate Weaver integration prefabs and scenes..."
"${UNITY_APP}" \
  -batchmode \
  -quit \
  -projectPath "${PROJECT_DIR}" \
  -executeMethod DestinyRanger.EditorTools.FateWeaverPrefabBuilder.BuildIntegrationPrefabsAndScenesBatch \
  -logFile -

echo "Validating Fate Weaver full delivery..."
"${UNITY_APP}" \
  -batchmode \
  -quit \
  -projectPath "${PROJECT_DIR}" \
  -executeMethod DestinyRanger.EditorTools.FateWeaverDeliveryValidator.ValidateBuiltIntegrationBatch \
  -logFile -

"${UNITY_APP}" \
  -batchmode \
  -quit \
  -projectPath "${PROJECT_DIR}" \
  -executeMethod DestinyRanger.EditorTools.FateWeaverDeliveryValidator.ValidateFullDeliveryBatch \
  -logFile -

echo "Running static audit..."
"${PROJECT_DIR}/../.venv/bin/python3" "${PROJECT_DIR}/Tools/fate_weaver_static_audit.py" 2>/dev/null \
  || python3 "${PROJECT_DIR}/Tools/fate_weaver_static_audit.py"

echo "Running requirement audit..."
"${PROJECT_DIR}/../.venv/bin/python3" "${PROJECT_DIR}/Tools/fate_weaver_requirement_audit.py" --write-md "${PROJECT_DIR}/Assets/DestinyRanger/Docs/REQUIREMENT_AUDIT.md" 2>/dev/null \
  || python3 "${PROJECT_DIR}/Tools/fate_weaver_requirement_audit.py" --write-md "${PROJECT_DIR}/Assets/DestinyRanger/Docs/REQUIREMENT_AUDIT.md"

echo "Fate Weaver integration build and validation completed."
