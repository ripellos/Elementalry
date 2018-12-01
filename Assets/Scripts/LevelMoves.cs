using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMoves : Level {

    public int numberOfMoves;
    public int targetScore;

    private int movesUsed = 0;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
    public override void OnMove() {
        movesUsed++;

        if (movesUsed >= numberOfMoves) {
            if (currentScore < targetScore) {
                GameLose();
            }
            else {
                GameWin();
            }
        }
    }


}
