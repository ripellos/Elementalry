using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectablePiece : MonoBehaviour {

    private SpriteRenderer spriteRenderer;

    private bool selected = false;

    public bool Selected {
        get { return selected; }
        set {
            selected = value;
            Color color = spriteRenderer.color;
            color.r = value ? 0f : 1f;
            spriteRenderer.color = color;
        }
    }

    private void Awake() {
        spriteRenderer = transform.Find("piece").GetComponent<SpriteRenderer>();
    }
}
