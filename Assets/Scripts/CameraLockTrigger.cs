using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLockTrigger : Trigger {
    public Vector3 targetPosition;

    public override void OnPlayerEnter(Player player) {
        CameraRig.Instance.targetPosition = targetPosition;
        CameraRig.Instance.followPlayer = false;
    }

    public override void OnPlayerExit(Player player) {
        CameraRig.Instance.followPlayer = true;
    }
}
