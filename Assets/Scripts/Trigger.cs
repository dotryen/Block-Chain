using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Trigger : MonoBehaviour {
    public void Awake() {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player") {
            var player = collision.gameObject.GetComponent<Player>();
            if (player.active) {
                OnPlayerEnter(player);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision) {
        if (collision.tag == "Player") {
            var player = collision.gameObject.GetComponent<Player>();
            if (player.active) {
                OnPlayerExit(player);
            }
        }
    }

    public virtual void OnPlayerEnter(Player player) {

    }

    public virtual void OnPlayerExit(Player player) {

    }
}
