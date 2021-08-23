using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetZone : Trigger {
    public override void OnPlayerEnter(Player player) {
        GameManager.Instance.LoadScene(GameManager.Instance.currentLevel);
    }
}
