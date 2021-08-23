using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : Interactable {
    public float speed;

    public void Update() {
        var rot = transform.eulerAngles;
        rot.z += speed * Time.deltaTime;
        rigidbody.angularVelocity = speed * Time.deltaTime;
        transform.eulerAngles = rot;
    }
}
