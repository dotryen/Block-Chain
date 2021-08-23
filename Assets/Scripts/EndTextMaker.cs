using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTextMaker : MonoBehaviour {
    public CanvasGroup alpha;
    public Text text;
    public float showTime;
    public float moveTime;
    public float moveAmount;
    public float pauseTime;

    private Vector3 originalPosition;
    private Vector3 velocity;

    private string[] queue;
    private int current = 0;
    private bool use = false;
    private bool begin = false;

    public void Start() {
        originalPosition = ((RectTransform)text.transform).anchoredPosition;
        alpha.alpha = 0f;
        queue = new string[] { "Block-Chain\n\nA submission for Brackeys Game Jam 2021",
            "Developers:\n\nLead Developer, Programmer, Sound Designer: riles\n\nLevel Designer, Sound Designer: JaNNN",
            "Playtesters:\n\nHubz\nwh4le\nTigerous1221\nFG_Ventcg\nSeals\nDuckMobster\nRick_",
            "Third Party Attribution:\n\nCredits Song: HOME - High Five",
            "Special Thanks to:\n\nAll the playtesters{2}",
            "Special Thanks to:\n\nUnity (for not crashing. Much.){2}",
            "Special Thanks to:\n\nBrackeys (For the awesome game Jam){2}",
            "Special Thanks to:\n\nYou, the player.{2}",
            "{20}",
            "Nothing else is going to happen.\nYou can leave now."
        };
    }

    public void Update() {
        if (!begin) return;
        if (use) return;
        if (current == queue.Length) {
            begin = false;
            return;
        }

        var text = queue[current];
        var show = showTime;

        // bracket time
        if (text.Contains("{")) {
            var indexFirst = text.IndexOf('{');
            var indexLast = text.IndexOf('}');

            string floatS = text.Substring(indexFirst + 1, (indexLast - 1) - indexFirst);
            show = float.Parse(floatS);
            text = text.Remove(indexFirst, indexLast - indexFirst + 1);
        }

        StartCoroutine(ShowTextInternal(text, moveTime, show));
        current++;
    }

    public void ShowText() {
        begin = true;
    }

    IEnumerator ShowTextInternal(string text, float move, float show) {
        use = true;

        var textTransform = (RectTransform)this.text.transform;
        var elapsedTime = 0f;

        var bottomPos = originalPosition;
        bottomPos.y -= moveAmount;

        var topPos = originalPosition;
        topPos.y += moveAmount;

        // prepare text
        this.text.text = text;
        textTransform.anchoredPosition = bottomPos;
        alpha.alpha = 0f;
        velocity = Vector3.zero;

        while (((move * 2) + show) >= elapsedTime) {
            if ((move + show) <= elapsedTime) {
                // transition to end
                textTransform.anchoredPosition = Vector3.SmoothDamp(textTransform.anchoredPosition, topPos, ref velocity, move);
                alpha.alpha -= Time.deltaTime / move;
            } else if (elapsedTime <= move) {
                // transition to show
                textTransform.anchoredPosition = Vector3.SmoothDamp(textTransform.anchoredPosition, originalPosition, ref velocity, move);
                alpha.alpha += Time.deltaTime / move;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        alpha.alpha -= Time.deltaTime / move;

        yield return new WaitForSeconds(pauseTime);
        use = false;
    }
}
