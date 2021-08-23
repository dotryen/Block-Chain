using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraRig : MonoBehaviour {
    // singleton
    public static CameraRig Instance { get; private set; }

    public Interactable InteractableOnMouse { get; private set; }
    public Vector2 WorldMousePosition { get; private set; }
    public Vector2 ScreenMousePosition { get; private set; }

    public bool followPlayer;
    public Vector2 targetPosition;
    public float speed;
    public Vector2 cameraVelocity;
    [Range(0f, 0.9f)]
    public float lerpAmount;
    public float mouseRadius;

    [Header("HUD Stuff")]
    public RectTransform cursor;
    public LineRenderer line;
    public Text cursorArrow;
    [Space]
    public RectTransform toolsPanel;
    public ToolSprite[] tools;
    public Text toolText;
    public float toolPanelSmoothTime;
    private Vector2 toolPanelTarget = new Vector2(0, -135.16f);
    private Vector3 toolPanelVelocity = Vector3.zero;
    [Space]
    public string hudText;

    bool lineDraw;
    bool lerpFix;

    public new Camera camera;
    Vector3 originalPosition;

    public void Awake() {
        Instance = this;
        if (!camera) camera = GetComponent<Camera>();
    }

    public void Start() {
        if (!Globals.useBloom) camera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>().renderPostProcessing = false;
        Cursor.visible = false;
    }

    void Update() {
        if (followPlayer) targetPosition = GameManager.Instance.currentPlayer.transform.position;
        UpdateCamera();
        GetInteractable();
        UpdateHud();
        cursor.anchoredPosition = ScreenMousePosition;
    }

    public void UpdateCamera() {
        if (lerpFix && !followPlayer) {
            originalPosition = transform.position;
            lerpFix = false;
        }

        // reset position
        Vector3 pos = Vector2.SmoothDamp(originalPosition, targetPosition, ref cameraVelocity, speed);
        pos.z = -10f;

        transform.position = pos;
        originalPosition = pos;

        // set screen mouse
        ScreenMousePosition = new Vector3(Mathf.Clamp(Input.mousePosition.x, 0, Screen.width), Mathf.Clamp(Input.mousePosition.y, 0, Screen.height));

        if (followPlayer) {
            lerpFix = true;

            var beforeRealMouse = camera.ScreenToWorldPoint(ScreenMousePosition);
            transform.position = Vector3.Lerp(originalPosition, beforeRealMouse, lerpAmount);
        }

        // resample mouse position
        WorldMousePosition = camera.ScreenToWorldPoint(ScreenMousePosition);
        Debug.DrawRay(WorldMousePosition, -(Vector3.forward * 15), Color.green, Time.deltaTime);
    }

    public void GetInteractable() {
        var cols = Physics2D.OverlapCircleAll(WorldMousePosition, mouseRadius);

        InteractableOnMouse = null;
        if (cols.Length != 0) {
            List<Interactable> interactables = new List<Interactable>();

            // check for all players in radius
            for (int i = 0; i < cols.Length; i++) {
                if (cols[i].gameObject.TryGetComponent<Interactable>(out Interactable interact)) {
                    if (interact is Player) {
                        if (!((Player)interact).active) interactables.Add(interact);
                    } else {
                        if (interact.usable) interactables.Add(interact);
                    }

                }
            }

            if (interactables.Count == 1) {
                InteractableOnMouse = interactables[0];
            } else {
                // find closest one
                Interactable closest = null;
                float distance = mouseRadius + 1f;

                foreach (Interactable interact in interactables) {
                    var interactPos = new Vector2(interact.transform.position.x, interact.transform.position.y);

                    if (distance > mouseRadius) {
                        closest = interact;
                        distance = Vector2.Distance(interactPos, WorldMousePosition);
                    } else {
                        var newDist = Vector2.Distance(interactPos, WorldMousePosition);
                        if (newDist < distance) {
                            distance = newDist;
                            closest = interact;
                        }
                    }
                }

                InteractableOnMouse = closest;
            }
        }
    }

    public void UpdateHud() {
        // update line
        lineDraw = false;
        if (InteractableOnMouse) {
            SetLine(ScreenMousePosition, camera.WorldToScreenPoint(InteractableOnMouse.transform.position));
        }
        line.gameObject.SetActive(lineDraw);

        // update tool panel
        if (Globals.toolsUnlocked) {
            toolText.text = "Tools - <b>" + hudText + "</b>";

            for (int i = 0; i < 4; i++) {
                tools[i].gameObject.SetActive(Globals.ToolUnlocked((ToolType)i + 1));

                if (tools[i].gameObject.activeInHierarchy) {
                    if ((int)Globals.currentTool == i + 1) {
                        tools[i].frame.sprite = GameManager.Instance.hudSprites[5];
                        tools[i].toolSprite.color = Color.black;
                    } else {
                        tools[i].frame.sprite = GameManager.Instance.hudSprites[4];
                        tools[i].toolSprite.color = Color.white;
                    }
                }
            }
            
            // update tool panel position
            if (MouseInsideRect(toolsPanel)) {
                toolPanelTarget.y = -1f;
                Globals.toolsTouched = true;
            } else {
                toolPanelTarget.y = -103.7f;
            }

            // update arrow
            if (Globals.toolsUnlocked && !Globals.toolsTouched) {
                cursorArrow.gameObject.SetActive(true);
                SetArrow(toolsPanel.anchoredPosition);
            } else {
                cursorArrow.gameObject.SetActive(false);
            }

            toolsPanel.anchoredPosition = Vector3.SmoothDamp(toolsPanel.anchoredPosition, toolPanelTarget, ref toolPanelVelocity, toolPanelSmoothTime);
        }
    }

    public void SetLine(Vector2 start, Vector2 end) {
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        lineDraw = true;
    }

    public void SetArrow(Vector2 pointTo) {
        var rot = cursorArrow.transform.eulerAngles;
        rot.z = -Vector2.Angle(Vector2.right, (pointTo - cursor.anchoredPosition)) + 180;
        cursorArrow.transform.eulerAngles = rot;
    }

    public bool MouseInsideRect(RectTransform rect) {
        return RectTransformUtility.RectangleContainsScreenPoint(rect, ScreenMousePosition, camera);
    }
}
