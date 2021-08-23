using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour {
    public new Rigidbody2D rigidbody;
    public new BoxCollider2D collider;
    public Transform indicator;
    public bool usable = true;
    protected virtual bool ScaleOnStart => true;

    public void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
    }

    public void Start() {
        indicator.gameObject.SetActive(usable);
        if (ScaleOnStart && usable) {
            SetSpriteScale(new Vector3(0.4145926f, 0.4145926f, 0.4145926f));
        }
    }

    public void SetSpriteScale(Vector3 scale) {
        indicator.localScale = new Vector3(scale.x / transform.localScale.x, scale.y / transform.localScale.y, scale.z / transform.localScale.z);
    }
}
