using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectablePiece : MonoBehaviour {

    private Animator animator;

    private bool selected = false;

    public bool Selected {
        get { return selected; }
        set {
            selected = value;
            animator.SetBool("selected", value);
        }
    }

    GamePiece piece;

    private void Awake() {
        piece = GetComponent<GamePiece>();
        animator = GetComponent<Animator>();
    }
}
