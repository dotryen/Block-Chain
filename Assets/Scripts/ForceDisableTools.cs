using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDisableTools : MonoBehaviour {
    public void Awake() {
        Globals.toolsUnlocked = false;
    }
}
