using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEffects : MonoBehaviour {
    public Image indicator;
    public new AudioSource audio;
    bool hovering;

    public void Awake() {
        if (!indicator) hovering = true;
    }

    public void Update() {
        if (!hovering) {
            var color = indicator.color;
            color.a = Mathf.Clamp01(color.a - (10 * Time.unscaledDeltaTime));
            indicator.color = color;
        }
    }

    public void PointerEnter() {
        audio.PlayOneShot(audio.clip);

        if (indicator) {
            indicator.color = Color.white;
            hovering = true;
        }
    }

    public void PointerExit() {
        if (indicator) hovering = false;
    }
}
