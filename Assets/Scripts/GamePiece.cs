using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {

	private int x;
	private int y;

	public int X
	{
		get { return x; }
		set {
			if (IsMovable ()) {
				x = value;
			}
		}
	}

	public int Y
	{
		get { return y; }
		set {
			if (IsMovable ()) {
				y = value;
			}
		}
	}

	private Grid.PieceType type;

	public Grid.PieceType Type
	{
		get { return type; }
	}

	private Grid grid;

	public Grid GridRef
	{
		get { return grid; }
	}

	private MovablePiece movableComponent;

	public MovablePiece MovableComponent
	{
		get { return movableComponent; }
	}

	private ColorPiece colorComponent;

	public ColorPiece ColorComponent
	{
		get { return colorComponent; }
	}

    private SelectablePiece selectableComponent;

    public SelectablePiece SelectableComponent {
        get { return selectableComponent;  }
    }

	void Awake()
	{
		movableComponent = GetComponent<MovablePiece> ();
		colorComponent = GetComponent<ColorPiece> ();
        selectableComponent = GetComponent<SelectablePiece>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Init(int _x, int _y, Grid _grid, Grid.PieceType _type)
	{
		x = _x;
		y = _y;
		grid = _grid;
		type = _type;
	}

	public bool IsMovable()
	{
		return movableComponent != null;
	}

	public bool IsColored()
	{
		return colorComponent != null;
	}

    public bool IsSelectable() {
        return selectableComponent != null;
    }

    private void OnMouseDown() {
        if (IsSelectable()) {
            grid.StartingDragging(this);
        }
    }
    private void OnMouseEnter() {
        if (IsSelectable()) {
            grid.EnteredPiece(this);
        }
    }
    private void OnMouseUp() {

        if (IsSelectable()) {
            grid.StopDragging();
        }
    }
}
