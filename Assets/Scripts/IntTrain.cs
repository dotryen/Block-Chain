using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntTrain : Interactable {
    public Vector3[] stops;
    public int stopCount;
    public float speed;
    bool back;

    private void Update() {
        Move();
    }

    private void FixedUpdate() {
        // MovePhysics();
    }

    public void Move() {
        var oldPos = transform.position;
        transform.position = Vector3.MoveTowards(transform.position, stops[stopCount], speed * Time.deltaTime);
        rigidbody.velocity = transform.position - oldPos;

        UpdateStops();
    }

    public void MovePhysics() {
        var direction = stops[stopCount] - transform.position;

        if (direction.magnitude < speed / Time.deltaTime) {
            rigidbody.velocity = direction;
        } else {
            rigidbody.velocity = direction.normalized * speed;
        }

        UpdateStops();
    }

    public void UpdateStops() {
        if (transform.position == stops[stopCount]) {
            if (stopCount == stops.Length - 1) {
                back = true;
            } else if (stopCount == 0) {
                back = false;
            }
    
            stopCount += back ? -1 : 1;
        }
    }
}
