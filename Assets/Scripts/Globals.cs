using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals {
    public static bool useBloom = true;
    public static float volume = 100f;

    public static bool toolsUnlocked = false;
    public static bool toolsTouched = false;
    public static bool selectUnlocked = false;
    public static bool knifeUnlocked = false;
    public static bool fixedUnlocked = false;
    public static bool springUnlocked = false;
    public static ToolType currentTool = ToolType.None;

    static bool SAVEtoolsUnlocked = false;
    static bool SAVEtoolsTouched = false;
    static bool SAVEselectUnlocked = false;
    static bool SAVEknifeUnlocked = false;
    static bool SAVEfixedUnlocked = false;
    static bool SAVEspringUnlocked = false;
    static ToolType SAVEcurrentTool = ToolType.None;

    public static void SetToolState(ToolType type, bool state) {
        var fields = typeof(Globals).GetFields();

        foreach (var field in fields) {
            if (field.Name == System.Enum.GetName(typeof(ToolType), type).ToLower() + "Unlocked") {
                field.SetValue(null, state);
                Globals.toolsUnlocked = state;
            }
        }
    }

    public static bool ToolUnlocked(ToolType type) {
        var fields = typeof(Globals).GetFields();

        foreach (var field in fields) {
            if (field.Name == System.Enum.GetName(typeof(ToolType), type).ToLower() + "Unlocked") {
                return (bool)field.GetValue(null);
            }
        }
        return true;
    }

    public static void ChangeTool(ToolType tool) {
        if (!ToolUnlocked(tool)) return;

        currentTool = tool;
        CameraRig.Instance.hudText = System.Enum.GetName(typeof(ToolType), tool).ToUpper();
    }

    public static void Save() {
        SAVEtoolsUnlocked = toolsUnlocked;
        SAVEtoolsTouched = toolsTouched;
        SAVEselectUnlocked = selectUnlocked;
        SAVEknifeUnlocked = knifeUnlocked;
        SAVEfixedUnlocked = fixedUnlocked;
        SAVEspringUnlocked = springUnlocked;
        SAVEcurrentTool = currentTool;
    }

    public static void Load() {
        toolsUnlocked = SAVEtoolsUnlocked;
        toolsTouched = SAVEtoolsTouched;
        selectUnlocked = SAVEselectUnlocked;
        knifeUnlocked = SAVEknifeUnlocked;
        fixedUnlocked = SAVEfixedUnlocked;
        springUnlocked = SAVEspringUnlocked;
        currentTool = SAVEcurrentTool;
    }
}
