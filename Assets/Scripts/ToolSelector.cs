using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSelector : MonoBehaviour {
    public void Select(int tool) {
        Globals.ChangeTool((ToolType)tool);
    }
}
