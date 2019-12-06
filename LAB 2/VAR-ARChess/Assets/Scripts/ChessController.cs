using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.UI;


public class ChessController : MonoBehaviour
{
	private string MESSAGE_WHITE_TURN = "It is the turn of white.";
	private string MESSAGE_BLACK_TURN = "It is the turn of black.";
	private string MESSAGE_NOT_YOUR_TURN = "It is not your turn.";
	private string MESSAGE_WHITE_WINS = "Checkmated. White wins. Congrats.";
	private string MESSAGE_BLACK_WINS = "Checkmated. Black wins. Congrats.";
	private string MESSAGE_INVALID_MOVE = "This is not a valid move.";
	private string MESSAGE_CHECK = "Check !";

	private int amountOfTurns = 0;
	private Figure[,] chessBoard;

    private GameObject chessGO;
    private Vector3 rcHit;
	private GameObject selectedGameObject;
	private Figure selectedFigureObject;

    public GameObject highlightprefab;

    public float speed;
	public Text turnDisplayText;
	public Text errorDisplayText;

	Ray ray;
	RaycastHit hit;
    Vector3 input = Vector3.zero;

    void Start() {
        chessGO = GameObject.FindGameObjectWithTag("Chess"); //Top Level Prefab which holds scaling and position properties.
		speed = 100000000.0f;
		initChessboard();
		turnDisplayText.text = MESSAGE_WHITE_TURN;
	}

    void Update()
    {

//#if !UNITY_ANDROID
//        if (Input.GetMouseButtonDown(0))
//                input = Input.mousePosition;

//        if (input != Vector3.zero)
//            processInput(input);
//#endif
    }

    public void processInput(Vector3 input)
    {
        displayMessage("");
        dehighlightOptions();

        ray = Camera.main.ScreenPointToRay(input);
        if (Physics.Raycast(ray, out hit))
        {
            rcHit = chessGO.transform.InverseTransformPoint(hit.point); //we always consider the Raycast hit in respect to the Chessboard scale
            Vector3Int integerHit = Vector3Int.RoundToInt(rcHit); // x and z are the respective board indices (also works when clicking figures!)
            
            string tag = hit.transform.gameObject.tag; 

            if (tag != "Chess" && tag != "Board")
                onClickOnFigure(hit.transform.gameObject, integerHit); 
            else
                onClickOnBoard(hit, integerHit); 
        }
       
    }

    private void onClickOnFigure(GameObject hitGameObject, Vector3Int indexVec) {
    	if(hasGameObjectColorOfCurrentPlayer(hitGameObject)) {
    		selectFigure(hitGameObject, indexVec);
    	} else {
    		if(selectedGameObject) {
    			makeTurnBeating(hitGameObject, indexVec);
    		} else {
    			displayMessage(MESSAGE_NOT_YOUR_TURN);
    		}
    	}
    }


    private void selectFigure(GameObject hitGameObject, Vector3Int indexVec) {
    	// deselect last figure
		if(selectedGameObject) {
			selectedGameObject.transform.Translate(new Vector3(0, -15, 0));
		}

		// select figure
		selectedGameObject = hitGameObject;
		selectedFigureObject = chessBoard[indexVec.x, indexVec.z];
		hit.transform.gameObject.transform.Translate(new Vector3(0, 15, 0));
        highlightOptionsForSelectedFigure();
    }


    private void onClickOnBoard(RaycastHit hit, Vector3Int indexVec) {
    	if(selectedGameObject && isNotFieldOfSelectedFigure(indexVec)) {
    		makeTurnMoving(hit, indexVec);
    	}
    }

    private void makeTurnMoving(RaycastHit hit, Vector3Int indexVec) {
    	int i = indexVec.x;
        int j = indexVec.z ;
    
    	if(chessBoard[i,j] != null) {
    		GameObject gameObjectOnField = null; 
    		if(hasFigureColorOfCurrentPlayer(chessBoard[i,j])) {
    			selectFigure(gameObjectOnField, indexVec);
    		} else {
    			makeTurnBeating(gameObjectOnField, indexVec);
    		}
    	}

		 if(!selectedFigureObject.isValidMove(i, j, chessBoard)) {
			displayMessage(MESSAGE_INVALID_MOVE);
			return;
		}   

		float step =  speed * Time.deltaTime;
        Vector3 localTargetPosition = new Vector3(i, 0.5f, j);
        Vector3 globalTargetPosition = chessGO.transform.TransformPoint(localTargetPosition);
        selectedGameObject.transform.position = Vector3.MoveTowards(selectedGameObject.transform.position, globalTargetPosition, step);

		moveFigureTo(selectedFigureObject, i, j);
		nextTurn();
	}

    private void makeTurnBeating(GameObject hitGameObject, Vector3Int indexVec) {
    	int i = indexVec.x;
    	int j = indexVec.z;
	
		if(! selectedFigureObject.isValidMove(i, j, chessBoard)) {
			displayMessage(MESSAGE_INVALID_MOVE);
			return;
		}  

    	checkWhetherKingGotBeaten(hitGameObject);
    	hitGameObject.SetActive(false);
    	selectedGameObject.transform.position = hitGameObject.transform.position;
    	moveFigureTo(selectedFigureObject, i, j);
    	nextTurn();

    	//TODO: animation
    }


    private void checkWhetherKingGotBeaten(GameObject hitGameObject) {
    	if(hitGameObject.tag == "King") {
    		string message = hasWhiteTurn() ? MESSAGE_WHITE_WINS : MESSAGE_BLACK_WINS;
    		displayMessage(message);
    	}
    }


	private void moveFigureTo(Figure figure, int x, int y) {
		chessBoard[figure.positionInFieldX,figure.positionInFieldY] = null;
		figure.positionInFieldX = x;
		figure.positionInFieldY = y;
		chessBoard[x,y] = figure;
	}

    private void nextTurn() {
    	// TODO validate whether opposite King is threaten // MESSAGE_CHECK

    	selectedGameObject = null;
    	selectedFigureObject = null;
    	amountOfTurns += 1;
    	turnDisplayText.text = hasWhiteTurn() ? MESSAGE_WHITE_TURN : MESSAGE_BLACK_TURN;
    }

    private void highlightOptionsForSelectedFigure()
    {
        List<IndexTuple> options = selectedFigureObject.getValidMoves(chessBoard);
        foreach (IndexTuple o in options) {
            Debug.LogWarning(o.i + "-" + o.j);
            GameObject highlighter = GameObject.Instantiate(original: highlightprefab, chessGO.transform);
            highlighter.transform.localPosition = new Vector3(o.i, 0.501f, o.j); //@TODO refactor
        }
    }

    private void dehighlightOptions()
    {
        GameObject[] highlighters = GameObject.FindGameObjectsWithTag("highlight");
        foreach(GameObject highlight in highlighters)
        {
            GameObject.Destroy(highlight);
        }
    }

    private bool isNotFieldOfSelectedFigure(Vector3Int indexVec)
    {
        if(selectedFigureObject == null)
        {
            return true;
        }
        return selectedFigureObject.positionInFieldX != indexVec.x || selectedFigureObject.positionInFieldY != indexVec.z;
    }

    private bool hasGameObjectColorOfCurrentPlayer(GameObject gameObject) {
		return (hasWhiteTurn() && gameObject.transform.parent.tag == "White") || (!hasWhiteTurn() && gameObject.transform.parent.tag == "Black"); 	
    }

    private bool hasFigureColorOfCurrentPlayer(Figure figure) {
    	return (hasWhiteTurn() && figure.isWhite) || (!hasWhiteTurn() && !figure.isWhite); 		
    }

    private bool hasWhiteTurn() {
    	return (amountOfTurns % 2) == 0 ? true : false; 
    } 

    private void displayMessage(string text) {
    	errorDisplayText.text = text;
    }

    private void initChessboard() {
    	chessBoard = new Figure[8, 8];

    	for (int i=0; i<8; i++) {
    		chessBoard[i, 1] = new Figure(i, 1, "Pawn", true);
    		chessBoard[i, 6] = new Figure(i, 6, "Pawn", false);
    	}

    	string[] sideRoles = new string[] {"Rook", "Knight", "Bishop"};
    	for (int j=0; j<3; j++) {
    		chessBoard[j, 0] = new Figure(j, 0, sideRoles[j], true);
    		chessBoard[7-j, 0] = new Figure(7-j, 0, sideRoles[j], true);
    		chessBoard[j, 7] = new Figure(j, 7, sideRoles[j], false);	
    		chessBoard[7-j, 7] = new Figure(7-j, 7, sideRoles[j], false);	
    	}

        chessBoard[4, 0] = new Figure(4, 0, "Queen", true);
		chessBoard[3, 0] = new Figure(3, 0, "King", true);
		chessBoard[3, 7] = new Figure(3, 7, "Queen", false);
		chessBoard[4, 7] = new Figure(4, 7, "King", false);
    }
}


public class IndexTuple
{
    public int i;
    public int j;

    public IndexTuple(int i, int j)
    {
        this.i = i;
        this.j = j;
    }

}

public class Figure {

	public int positionInFieldX;
	public int positionInFieldY;
	public string role;
	public bool isWhite;

	public Figure(int positionX, int positionY, string pRole, bool pIsWhite) {
      positionInFieldX = positionX;
      positionInFieldY = positionY;
      role = pRole;
      isWhite = pIsWhite;
   	}

	public bool isValidMove(int positionX, int positionY, Figure[,] chessBoard) {
		switch(role) {
			case "Pawn": return isValidPawnMove(positionX, positionY, chessBoard);
			case "Knight": return isValidKnightMove(positionX, positionY, chessBoard);
			case "Bishop": return isValidBishopMove(positionX, positionY, chessBoard);
			case "Rook": return isValidRookMove(positionX, positionY, chessBoard);
			case "King": return isValidKingMove(positionX, positionY, chessBoard);
			case "Queen": return isValidQueenMove(positionX, positionY, chessBoard);
		}
		return false;
	}

    public List<IndexTuple> getValidMoves(Figure[,] chessBoard)
    {
        switch (role)
        {
            case "Pawn": return getValidPawnMoves(chessBoard);
           /** case "Knight": return getValidKnightMoves(chessBoard);
            case "Bishop": return getValidBishopMoves(chessBoard);
            case "Rook": return getValidRookMoves(chessBoard);
            case "King": return getValidKingMoves(chessBoard);
            case "Queen": return getValidQueenMoves(chessBoard); **/
        }
        return new List<IndexTuple>();
    }
    public List<IndexTuple> getValidPawnMoves(Figure[,] chessBoard)
    {
        List<IndexTuple> results = new List<IndexTuple>();

        int yVorzeichen = isWhite ? 1 : -1;
        bool isInStartPosition = isWhite ? positionInFieldY == 1 : positionInFieldY == 6;
        bool isFordwardFieldNotOccupied = chessBoard[positionInFieldX, positionInFieldY + yVorzeichen] == null;
        bool isDiagonalFieldOccupiedRight = positionInFieldX < 7 && chessBoard[positionInFieldX + 1, positionInFieldY + yVorzeichen] != null;
        bool isDiagonalFieldOccupiedLeft = positionInFieldX > 0 && chessBoard[positionInFieldX - 1, positionInFieldY + yVorzeichen] != null;


        if (isInStartPosition)
        {
            results.Add(new IndexTuple(positionInFieldX, positionInFieldY + (2*yVorzeichen)));
        }
        if (isFordwardFieldNotOccupied)
        {
            results.Add(new IndexTuple(positionInFieldX, positionInFieldY + yVorzeichen));
        }
        if (isDiagonalFieldOccupiedRight)
        {
            results.Add(new IndexTuple(positionInFieldX+1, positionInFieldY + yVorzeichen));
        }
        if (isDiagonalFieldOccupiedLeft)
        {
            results.Add(new IndexTuple(positionInFieldX-1, positionInFieldY + yVorzeichen));
        }

        return results;
    }

    private bool isValidPawnMove(int positionX, int positionY, Figure[,] chessBoard) {
		int xDiff = positionX - positionInFieldX;
		int yDiff = positionY - positionInFieldY;

		bool isOneStepForward = isWhite ? yDiff == 1 : yDiff == -1;
		bool isStraightMoveAndNotOccupiedField = chessBoard[positionX, positionY] == null && xDiff == 0;
		bool isDiagonalMoveAndOccupiedField = chessBoard[positionX, positionY] != null && Math.Abs(xDiff) == 1;
        bool isInStartPosition = isWhite ? positionInFieldY == 1 : positionInFieldY == 6;
        bool isTwoStepsForward = Math.Abs(yDiff) == 2;
		return (isOneStepForward && (isStraightMoveAndNotOccupiedField || isDiagonalMoveAndOccupiedField))
            || (isInStartPosition && isTwoStepsForward);
	}

	// TODO change Pawn to Queen if other end reached

	private bool isValidKnightMove(int positionX, int positionY, Figure[,] chessBoard) {
		int xDiff = Math.Abs(positionX - positionInFieldX);		
		int yDiff = Math.Abs(positionY - positionInFieldY);
		return (xDiff == 2 && yDiff == 1) || (xDiff == 1 && yDiff == 2);
	}

	private bool isValidBishopMove(int positionX, int positionY, Figure[,] chessBoard) {
		int xDiff = positionX - positionInFieldX;		
		int yDiff = positionY - positionInFieldY;
		int xVorzeichen = xDiff < 0 ? -1 : 1;
		int yVorzeichen = yDiff < 0 ? -1 : 1;


		if(Math.Abs(yDiff) != Math.Abs(xDiff)) {
			return false;
		}

		for(int i=1; i< Math.Abs(xDiff); i++) {
			if(chessBoard[positionInFieldX+(i*xVorzeichen), positionInFieldY+(i*yVorzeichen)] != null) {
				return false;
			}
		}

		return true;
	}

	private bool isValidRookMove(int positionX, int positionY, Figure[,] chessBoard) {
		int xDiff = positionX - positionInFieldX;		
		int yDiff = positionY - positionInFieldY;
		int xVorzeichen = xDiff < 0 ? -1 : 1;
		int yVorzeichen = yDiff < 0 ? -1 : 1;

		if(!(xDiff == 0 || yDiff == 0)) {
			return false;
		}

		int maxDiff = xDiff > yDiff ? xDiff : yDiff;
		for(int i=1; i< maxDiff; i++) {
			if(yDiff == 0 && chessBoard[positionInFieldX+(i*xVorzeichen), positionInFieldY] != null) {
				return false;
			} else if (xDiff == 0 && chessBoard[positionInFieldX, positionInFieldY+(i*yVorzeichen)] != null ) {
				return false;
			}
		}

		return true;
	}

	private bool isValidQueenMove(int positionX, int positionY, Figure[,] chessBoard) {
		return isValidRookMove(positionX, positionY, chessBoard) || isValidBishopMove(positionX, positionY, chessBoard);
	}

	private bool isValidKingMove(int positionX, int positionY, Figure[,] chessBoard) {
		int xDiff = Math.Abs(positionX - positionInFieldX);		
		int yDiff = Math.Abs(positionY - positionInFieldY);
		return xDiff <= 1 && yDiff <= 1;
	}
	// TODO: TurmKing Tausch
	// TODO: checkIsKingThreaten




}