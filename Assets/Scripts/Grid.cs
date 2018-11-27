using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// Touch
public class Grid : MonoBehaviour {

	public enum PieceType
	{
		EMPTY,
		NORMAL,
		BUBBLE,
		COUNT,
	};

	[System.Serializable]
	public struct PiecePrefab
	{
		public PieceType type;
		public GameObject prefab;
	};

	public int xDim;
	public int yDim;
	public float fillTime;

	public PiecePrefab[] piecePrefabs;
	public GameObject backgroundPrefab;

	private Dictionary<PieceType, GameObject> piecePrefabDict;

	private GamePiece[,] pieces;
    private Stack<GamePiece> selectedPieces;

    private ColorPiece.ColorType selectedColor;
    private bool selecting = false;

    // Use this for initialization
    void Start () {
		piecePrefabDict = new Dictionary<PieceType, GameObject> ();
        selectedPieces = new Stack<GamePiece>();

		for (int i = 0; i < piecePrefabs.Length; i++) {
			if (!piecePrefabDict.ContainsKey (piecePrefabs [i].type)) {
				piecePrefabDict.Add (piecePrefabs [i].type, piecePrefabs [i].prefab);
			}
		}

		for (int x = 0; x < xDim; x++) {
			for (int y = 0; y < yDim; y++) {
				GameObject background = (GameObject)Instantiate (backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
				background.transform.parent = transform;
			}
		}

		pieces = new GamePiece[xDim, yDim];
		for (int x = 0; x < xDim; x++) {
			for (int y = 0; y < yDim; y++) {
				SpawnNewPiece (x, y, PieceType.EMPTY);
			}
		}

		StartCoroutine(Fill ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public IEnumerator Fill() {
        bool needsRefill = true;
        while (needsRefill) {
            while (FillStep()) {
                yield return new WaitForSeconds(fillTime);
            }
            // Right now there will never be more matches. We only ever clear
            // what the player selects which will always be nothing after fill.

            needsRefill = ClearAllValidMatches();
        }
	}

	public bool FillStep()
	{
		bool movedPiece = false;

		for (int y = yDim-2; y >= 0; y--)
		{
			for (int x = 0; x < xDim; x++)
			{
				GamePiece piece = pieces [x, y];

				if (piece.IsMovable ())
				{
					GamePiece pieceBelow = pieces [x, y + 1];

					if (pieceBelow.Type == PieceType.EMPTY)
					{
						Destroy (pieceBelow.gameObject);
						piece.MovableComponent.Move (x, y + 1, fillTime);
						pieces [x, y + 1] = piece;
						SpawnNewPiece (x, y, PieceType.EMPTY);
						movedPiece = true;
					}
				}
			}
		}

		for (int x = 0; x < xDim; x++)
		{
			GamePiece pieceBelow = pieces [x, 0];

			if (pieceBelow.Type == PieceType.EMPTY)
			{
				Destroy (pieceBelow.gameObject);
				GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
				newPiece.transform.parent = transform;

				pieces [x, 0] = newPiece.GetComponent<GamePiece> ();
				pieces [x, 0].Init (x, -1, this, PieceType.NORMAL);
				pieces [x, 0].MovableComponent.Move (x, 0, fillTime);
				pieces [x, 0].ColorComponent.SetColor ((ColorPiece.ColorType)Random.Range (0, pieces [x, 0].ColorComponent.NumColors));
				movedPiece = true;
			}
		}

		return movedPiece;
	}

	public Vector2 GetWorldPosition(int x, int y)
	{
		return new Vector2 (transform.position.x - xDim / 2.0f + x,
			transform.position.y + yDim / 2.0f - y);
	}

	public GamePiece SpawnNewPiece(int x, int y, PieceType type)
	{
		GameObject newPiece = (GameObject)Instantiate (piecePrefabDict [type], GetWorldPosition (x, y), Quaternion.identity);
		newPiece.transform.parent = transform;

		pieces [x, y] = newPiece.GetComponent<GamePiece> ();
		pieces [x, y].Init (x, y, this, type);

		return pieces [x, y];
	}

    public bool ClearAllValidMatches() {
        bool needsRefill = false;
        while(selectedPieces.Count > 0) {
            GamePiece piece = selectedPieces.Pop();
            if (ClearPiece(piece.X, piece.Y)){
                needsRefill = true;
            }
        }
        Debug.Log("needsRefill: " + needsRefill);
        return needsRefill;
    }

    bool ClearPiece(int x, int y){
        if(pieces[x,y].IsClearable() && !pieces[x,y].ClearableComponent.IsBeingCleared) {
            pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);
            return true;
        }
        return false;
    }

    public void StartingDragging(GamePiece piece) {
        if (!selecting) {
            selectedPieces = new Stack<GamePiece>();
            selecting = true;
            selectedColor = piece.ColorComponent.Color;
            SelectPiece(piece);
        }
    }
    public bool EnteredPiece(GamePiece piece) {
        if (!selecting) {
            return false;
        }
        if (piece.ColorComponent.Color != selectedColor) {
            return false;
        }
        if (selectedPieces.Contains(piece)) {
            return false;
        }       
        if (!IsAdjacent(piece, selectedPieces.Peek())) {
            return false;
        }
        SelectPiece(piece);

        return true;
    }
    public void StopDragging() {
        selecting = false;
        if (selectedPieces.Count >= 3) {
            ClearAllValidMatches();
            StartCoroutine(Fill());
        } else {
            foreach (GamePiece piece in selectedPieces) {
                piece.SelectableComponent.Selected = false;
            }
        }
       
        /*while (selectedPieces.Count > 0)
        {
            GamePiece seenPiece = selectedPieces.Pop();
            seenPiece.SelectableComponent.Selected = false;
        }*/

        // Regardless of whether or not the dragging stops on the last piece in
        // the series, we clear the selected pieces. This prevents erroneously
        // clearing the selected pieces if accidentally released on an
        // incompatible piece.

        // It's not clear if we can safely clear the stack if the GameObjects
        // are destroyed (which occurs when they are cleared from the grid.
       //selectedPieces.Clear();
        selecting = false;
    }

    bool IsAdjacent(GamePiece piece1, GamePiece piece2) {
        return ((int)Mathf.Abs(piece1.X - piece2.X) <= 1
                && (int)Mathf.Abs(piece1.Y - piece2.Y) <= 1);

    }
    void SelectPiece(GamePiece piece) {
        piece.SelectableComponent.Selected = true;
        selectedPieces.Push(piece);
    }
    void DeselectLastPiece() {
        GamePiece piece = selectedPieces.Pop();
        piece.SelectableComponent.Selected = false;
    }
}
