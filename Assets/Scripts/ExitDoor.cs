using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : Trigger {
    public override void OnPlayerEnter(Player player) {
        GameManager.Instance.LoadNext();
    }
}
