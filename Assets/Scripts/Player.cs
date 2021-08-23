using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Interactable {
    public enum JointType { Fixed, Spring }

    public new Light light;
    public AudioSource audioSource;
    public LineRenderer originalRenderer;
    public SpriteRenderer radiusCircle;
    [Space]
    public bool grounded;
    public bool active;

    // joints
    List<Joint2D> joints = new List<Joint2D>();
    List<Interactable> jointMemory = new List<Interactable>();
    List<LineRenderer> jointRenderers = new List<LineRenderer>();

    // staying on objects
    BoxCollider2D onGround;

    bool jumpReady = true;
    bool walking;
    bool intInRadius;
    Vector3 originalRadiusScale;

    protected override bool ScaleOnStart => false;

    public new void Start() {
        originalRadiusScale = radiusCircle.transform.localScale;
        indicator.parent.localPosition = Vector3.zero;
    }

    public void Update() {
        UpdateJoints();
        UpdateEyes();

        // active player loop
        if (!active) return;

        // update radius circle
        radiusCircle.transform.localScale = originalRadiusScale * Radius();
        var color = radiusCircle.color;

        if (intInRadius) color.a += Time.deltaTime;
        else color.a -= Time.deltaTime;
        color.a = Mathf.Clamp01(color.a);

        radiusCircle.color = color;
    }

    public void FixedUpdate() {
        var vec = active ? UpdateInput() : Vector2.zero;
        Move(vec);

        if (active) {
            var objects = Physics2D.OverlapCircleAll(transform.position, Radius() + 1);
            intInRadius = false;

            foreach (var obj in objects) {
                var comp = obj.gameObject.GetComponent<Interactable>();
                if (comp) {
                    if (comp == this) continue;
                    else intInRadius = true;
                }

                if (intInRadius) break;
            }
        }
    }

    public void SetActive(bool active) {
        if (!this.active && GameManager.Instance.currentPlayer) {
            GameManager.Instance.currentPlayer.SetActive(false);
            GameManager.Instance.currentPlayer = this;
        }

        this.active = active;
        light.gameObject.SetActive(active);
        radiusCircle.gameObject.SetActive(active);
    }

    public void UpdateEyes() {
        var currentX = transform.position.x;
        var currentY = transform.position.y;
        var target = active ? (Vector3)CameraRig.Instance.WorldMousePosition : GameManager.Instance.currentPlayer.transform.position;

        indicator.transform.eulerAngles = Vector3.zero;
        indicator.transform.position = Vector3.MoveTowards(indicator.transform.position, target, PlayerSettings.eyeSpeed * Time.deltaTime * 0.9f);
        indicator.transform.position = new Vector3(Mathf.Clamp(indicator.transform.position.x, currentX - 0.273f, currentX + 0.273f), Mathf.Clamp(indicator.transform.position.y, currentY - 0.2f, currentY + 0.25f), -1f);
    }

    public void LandEffect(Vector2 velocity) {
        audioSource.PlayOneShot(audioSource.clip, Mathf.InverseLerp(6, 12, velocity.y));
    }
        
    public Vector2 UpdateInput() {
        var vec = Vector2.zero;
        if (Input.GetKey(KeyCode.A)) vec.x--;
        if (Input.GetKey(KeyCode.D)) vec.x++;
        walking = Input.GetKey(KeyCode.LeftShift);
        vec.y = Input.GetKey(KeyCode.Space) ? 1f : 0;

        // update tools
        for (int i = 1; i <= 4; i++) {
            if (Input.GetKeyDown("" + i)) Globals.ChangeTool((ToolType)i);
        }
        if (Input.GetMouseButtonDown(0)) UseTool();

        // reset on R
        if (Input.GetKeyDown(KeyCode.R)) GameManager.Instance.LoadScene(GameManager.Instance.currentLevel);

        return vec;
    }

    public void Move(Vector2 vec) {
        var realVelocity = rigidbody.velocity;
        var groundVelocity = Vector3.zero;

        if (onGround) {
            if (onGround.attachedRigidbody) {
                realVelocity -= onGround.attachedRigidbody.velocity;
                groundVelocity = onGround.attachedRigidbody.velocity;
            }
        }

        var delta = Vector2.zero;
        var target = vec * ((walking ? PlayerSettings.walkSpeed : PlayerSettings.speed) + (joints.Count * PlayerSettings.boostAmount));

        if (vec.x != 0 || grounded) delta.x = (target.x - realVelocity.x) * (grounded ? PlayerSettings.acceleration : PlayerSettings.airControl);

        rigidbody.AddForce(delta, ForceMode2D.Force);
        rigidbody.AddForce(groundVelocity, ForceMode2D.Force);

        if (vec.y == 1 && grounded && jumpReady) {
            rigidbody.AddForce(Vector2.up * PlayerSettings.jumpSpeed, ForceMode2D.Impulse);
            jumpReady = false;
            Invoke(nameof(JReady), Time.fixedDeltaTime * 5);
        }
    }

    public void JReady() {
        jumpReady = true;
    }

    #region Tools

    public void UseTool() {
        var interactable = CameraRig.Instance.InteractableOnMouse;
        var toolInt = (int)Globals.currentTool;

        if (interactable) {
            if (InteractableInRange(interactable)) {
                if (toolInt == 1) {
                    if (IsPlayer(interactable, out Player player)) {
                        player.SetActive(true);
                    }
                } else if (toolInt == 2) {
                    ClearJoint(interactable);
                } else if (toolInt == 3) {
                    AddJoint(JointType.Fixed, interactable);
                } else if (toolInt == 4) {
                    AddJoint(JointType.Spring, interactable);
                }
            }
        }
    }

    public void UpdateJoints() {
        if (joints.Count == 0) return;

        for (int i = 0; i < joints.Count; i++) {
            jointRenderers[i].SetPosition(0, transform.position);
            jointRenderers[i].SetPosition(1, joints[i].connectedBody.position);
        }
    }

    public void AddJoint(JointType type, Interactable other) {
        if (jointMemory.Contains(other)) return;
        Material rendMat = null;

        if (type == JointType.Fixed) {
            // var relative = gameObject.AddComponent<RelativeJoint2D>();
            // relative.connectedBody = other.rigidbody;
            // relative.maxForce = 1000;
            // relative.maxTorque = 1000;
            // relative.autoConfigureOffset = false;

            var fixedJoint = gameObject.AddComponent<FixedJoint2D>();
            fixedJoint.connectedBody = other.rigidbody;
            fixedJoint.autoConfigureConnectedAnchor = false;
            fixedJoint.dampingRatio = 1;
            rendMat = GameManager.Instance.fixedJoint;

            joints.Add(fixedJoint);
        } else {
            var springJoint = gameObject.AddComponent<SpringJoint2D>();
            springJoint.connectedBody = other.rigidbody;
            springJoint.enableCollision = true;
            springJoint.frequency = 1.25f;
            springJoint.autoConfigureDistance = false;
            rendMat = GameManager.Instance.spring;

            joints.Add(springJoint);
        }

        jointMemory.Add(other);
        if (jointRenderers.Count == 0) {
            jointRenderers.Add(originalRenderer);
        } else {
            jointRenderers.Add(Instantiate(originalRenderer));
        }
        jointRenderers[jointRenderers.Count - 1].material = rendMat;
    }

    public void ClearJoint(Interactable other) {
        if (!jointMemory.Contains(other)) return;

        var index = jointMemory.IndexOf(other);
        jointMemory.RemoveAt(index);

        Destroy(joints[index]);
        joints.RemoveAt(index);

        if (index != 0) {
            Destroy(jointRenderers[index].gameObject);
        } else {
            jointRenderers[index].SetPosition(0, Vector3.zero);
            jointRenderers[index].SetPosition(1, Vector3.zero);
        }

        jointRenderers.RemoveAt(index);
    }

    #endregion

    #region Utilities

    public float Radius() {
        var radius = PlayerSettings.playerRange;
        if (joints.Count != 0) radius += PlayerSettings.rangeAdd;
        return radius;
    }

    public bool InteractableInRange(Interactable toTest) {
        return Vector3.Distance(transform.position, toTest.transform.position) <= Radius();
    }

    public bool IsPlayer(Interactable interact) {
        return interact is Player;
    }

    public bool IsPlayer(Interactable interact, out Player player) {
        var valid = interact is Player;
        if (valid) {
            player = (Player)interact;
        } else {
            player = null;
        }
        return valid;
    }

    #endregion

    #region Ground Check

    public void OnCollisionEnter2D(Collision2D collision) {
        LandEffect(collision.relativeVelocity);
    }

    public void OnCollisionStay2D(Collision2D collision) {
        if (grounded || collision.contactCount == 0) return;

        var contacts = new ContactPoint2D[collision.contactCount];
        int count = collision.GetContacts(contacts);

        for (int i = 0; i < count; i++) {
            ContactPoint2D contact = contacts[i];

            if (IsFloor(contact.normal, PlayerSettings.slopeLimit)) {
                grounded = true;
                onGround = (BoxCollider2D)contact.collider;
                break;
            }
        }
    }

    public void OnCollisionExit2D(Collision2D collision) {
       if (collision.collider == onGround) {
           grounded = false;
           onGround = null;
       }
    }

    protected bool IsFloor(Vector2 v, float slope) {
        float angle = Vector2.Angle(Vector2.up, v);
        return angle < slope;
    }

    #endregion
}
