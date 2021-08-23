using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class EndTrigger : CameraLockTrigger {
    [HideInInspector]
    public CameraShakeInstance shakerInstance;
    public CameraShaker shaker;
    public Animator cutsceneAnimator;
    public EndTextMaker textMaker;
    public Transform parentTo;
    public new AudioSource audio;
    public UnityEngine.VFX.VisualEffect[] effects;

    public override void OnPlayerEnter(Player player) {
        base.OnPlayerEnter(player);

        player.rigidbody.velocity = Vector2.zero;
        player.rigidbody.angularVelocity = 0f;
        player.rigidbody.bodyType = RigidbodyType2D.Kinematic;
        player.enabled = false;

        player.transform.parent = parentTo;
        player.transform.localPosition = Vector3.up;

        StartCoroutine(StartCutscene());
    }

    public override void OnPlayerExit(Player player) {
        return;
    }

    public void StartShake() {
        shakerInstance = shaker.StartShake(4, 1, 0.1f);
    }

    public void PlayMusic() {
        audio.Play();
    }

    public void StartEffects() {
        foreach (var effect in effects) {
            effect.SendEvent("OnPlay");
        }
    }

    public void ShowText() {
        textMaker.ShowText();
    }

    IEnumerator StartCutscene() {
        // wait until camera is in position
        yield return new WaitUntil(() => Vector2.Distance(CameraRig.Instance.transform.position, targetPosition) <= 0.01f);

        CameraRig.Instance.cursor.gameObject.SetActive(false);
        CameraRig.Instance.enabled = false;

        CameraRig.Instance.transform.parent = parentTo;

        var point = parentTo.InverseTransformPoint(targetPosition);
        point.z = -10f;
        shaker.RestPositionOffset = point;
        shaker.enabled = true;

        cutsceneAnimator.Play("Play");
    }
}
