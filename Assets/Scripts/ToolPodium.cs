using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.VFX;

public class ToolPodium : Trigger {
    public SpriteRenderer tool;
    public VisualEffect effect;
    public AudioSource sound;
    public ToolType type;
    public float moveAmount;

    private Vector3 originalPosition;

    public void Start() {
        originalPosition = tool.transform.localPosition;
        tool.sprite = GameManager.Instance.hudSprites[(int)type - 1];
    }

    public void Update() {
        var pos = originalPosition;
        pos.y = pos.y + (Mathf.Sin(Time.time) * moveAmount);
        tool.transform.localPosition = pos;
    }

    public override void OnPlayerEnter(Player player) {
        if (!tool.gameObject.activeInHierarchy) return;

        Globals.SetToolState(type, true);
        if (type == ToolType.Fixed || type == ToolType.Spring) {
            Globals.SetToolState(ToolType.Knife, true);
        }

        tool.gameObject.SetActive(false);
        effect.SendEvent("OnPlay");
        sound.Play();
    }
}
