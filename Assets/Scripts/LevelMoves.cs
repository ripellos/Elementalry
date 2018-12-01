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
        Debug.Log("Moves used: " + movesUsed);
        Debug.Log("Moves left: " + (numberOfMoves - movesUsed));

        if (numberOfMoves - movesUsed == 0) {
            if (currentScore < targetScore) {
                GameLose();
            }
            else {
                GameWin();
            }
        }
    }
}
