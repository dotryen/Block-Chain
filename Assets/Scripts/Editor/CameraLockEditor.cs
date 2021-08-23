using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraLockTrigger))]
public class CameraLockEditor : Editor {
    private void OnSceneGUI() {
        var lockComp = (CameraLockTrigger)target;
        var targetPosEnd = lockComp.targetPosition;
        targetPosEnd.z -= 10;

        Handles.DrawLine(lockComp.targetPosition, targetPosEnd);
    }
}
