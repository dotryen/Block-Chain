using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public UnityEngine.Audio.AudioMixer mixer;
    public UnityEngine.UI.Text sliderText;
    public UnityEngine.Rendering.Universal.UniversalAdditionalCameraData cameraData;
    public int level = 2;

    public void Update() {
        cameraData.renderPostProcessing = Globals.useBloom;
    }

    public void ToggleMenus() {
        mainMenu.SetActive(!mainMenu.activeInHierarchy);
        optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
    }

    public void Play() {
        GameManager.Instance.LoadNext();
    }

    public void BloomOption(bool value) {
        Globals.useBloom = value;
    }

    public void VolumeOption(float value) {
        Globals.volume = value;

        float valueCalc = Mathf.InverseLerp(0, 100, value);
        sliderText.text = value.ToString();
        mixer.SetFloat("Volume", Mathf.Lerp(-80, 0, valueCalc));
    }

    public void Quit() {
        Application.Quit();
    }
}
