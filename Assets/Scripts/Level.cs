using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public enum LevelType {
        TIMER,
        SCORE,
        OBSTACLE,
    };

    public Grid gridRef;

    public int score1Star;
    public int score2Star;
    public int score3Star;

    protected int currentScore;

    protected LevelType type;

    public LevelType Type {
        get { return type; }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void GameWin() {
        gridRef.GameOver();
        Debug.Log("You win!");
    }

    public virtual void GameLose() {
        gridRef.GameOver();
        Debug.Log("You lose!");
    }
    public virtual void OnMove() {   
    }
    public virtual void OnPieceCleared(GamePiece piece) {
        // Determine score from piece.
        currentScore += piece.score;
        Debug.Log("Current score: " + currentScore);
    }

}
