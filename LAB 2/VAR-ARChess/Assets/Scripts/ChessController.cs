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
	
	private GameObject selectedGameObject;
	private Figure selectedFigureObject;

	public Text turnDisplayText;
	
	Ray ray;
	RaycastHit hit;

	void Start() {
		initChessboard();
	}

    void Update()
    {
    	 ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         if(Physics.Raycast(ray, out hit)) {
         	GameObject hitGameObject = hit.transform.gameObject;
			if(Input.GetMouseButtonDown(0)) {
				if(hitGameObject.tag != "Chess") {
					this.onClickOnFigure(hitGameObject);
            	} else {
            		this.onClickOnBoard(hit);
            	}
            }
         }
    }

    private void onClickOnFigure(GameObject hitGameObject) {
    	if(hasGameObjectColorOfCurrentPlayer(hitGameObject)) {
    		selectFigure(hitGameObject);
    	} else {
    		if(selectedGameObject) {
    			makeTurnBeating(hitGameObject);
    		} else {
    			displayMessage(MESSAGE_NOT_YOUR_TURN);
    		}
    	}
    }


    private void selectFigure(GameObject hitGameObject) {
    	// deselect last figure
		if(selectedGameObject) {
			selectedGameObject.transform.Translate(new Vector3(0, -15, 0));
		}

		// select figure
		selectedGameObject = hitGameObject;
		selectedFigureObject = chessBoard[0, 0]; //TODO: get chessBoard indices By clicked Figure / GameObjectFigure
		hit.transform.gameObject.transform.Translate(new Vector3(0, 15, 0));

		// TODO visualize options
    }


    private void onClickOnBoard(RaycastHit hit) {
    	if(selectedGameObject) {
    		makeTurnMoving(hit);
    	}
    }

    private void makeTurnMoving(RaycastHit hit) {
		// TODO get chessBoard indices By clicked Field
    	int x = 0;
    	int y = 0;

    	if(chessBoard[x,y] != null) {
    		//GameObject gameObjectOnField = null; // TODO get GameObject on clicked Field By chessboard Indices 
    		if(hasFigureColorOfCurrentPlayer(chessBoard[x,y])) {
    			//selectFigure(gameObjectOnField);
    		} else {
    			//makeTurnBeating(gameObjectOnField);
    		}
    	}

		if( !selectedFigureObject.isValidMove(x, y, chessBoard)) {
			displayMessage(MESSAGE_INVALID_MOVE);
			return;
		}    	

		// TODO normalize/centralize position
		selectedGameObject.transform.position = new Vector3((float)Math.Floor(hit.point.x), -80, (float)Math.Floor(hit.point.z));
		moveFigureTo(selectedFigureObject, x, y);
		nextTurn();

		//TOOD: animation
	}

    private void makeTurnBeating(GameObject hitGameObject) {
    	// TODO get ChessBoard Indices By GameObjectFigure
    	int x = 0;
    	int y = 0;
		if(! selectedFigureObject.isValidMove(x, y, chessBoard)) {
			displayMessage(MESSAGE_INVALID_MOVE);
			return;
		}    	

    	checkWhetherKingGotBeaten(hitGameObject);
    	hitGameObject.SetActive(false);
    	selectedGameObject.transform.position = hitGameObject.transform.position;
    	moveFigureTo(selectedFigureObject, x, y);
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
    	turnDisplayText.text = text;
    	// TODO anderes Text Objekt
    }

    private void initChessboard() {
    	chessBoard = new Figure[8, 8];

    	for (int i=0; i<8; i++) {
    		chessBoard[i, 1] = new Figure(i, 1, "Pawn", false);
    		chessBoard[i, 6] = new Figure(i, 6, "Pawn", true);
    	}

    	string[] sideRoles = new string[] {"Rook", "Knight", "Bishop"};
    	for (int j=0; j<3; j++) {
    		chessBoard[j, 1] = new Figure(j, 1, "Pawn", false);
    		chessBoard[7-j, 1] = new Figure(7-j, 1, "Pawn", false);
    		chessBoard[j, 6] = new Figure(j, 6, "Pawn", true);	
    		chessBoard[7-j, 6] = new Figure(7-j, 6, "Pawn", true);	
    	}

		chessBoard[0, 4] = new Figure(0, 4, "Queen", false);
		chessBoard[0, 3] = new Figure(0, 3, "King", false);
		chessBoard[7, 3] = new Figure(7, 3, "Queen", true);
		chessBoard[7, 4] = new Figure(7, 4, "King", true);
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

	private bool isValidPawnMove(int positionX, int positionY, Figure[,] chessBoard) {
		int xDiff = positionX - positionInFieldX;
		int yDiff = Math.Abs(positionY - positionInFieldY);

		bool isForwardMove = isWhite ? yDiff == -1 : yDiff == 1;
		bool isStraightMoveAndNotOccupiedField = chessBoard[positionX, positionY] == null && xDiff == 0;
		bool isDiagonalMoveAndOccupiedField = chessBoard[positionX, positionY] != null && xDiff == 1;
		return isForwardMove && (isStraightMoveAndNotOccupiedField || isDiagonalMoveAndOccupiedField);
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