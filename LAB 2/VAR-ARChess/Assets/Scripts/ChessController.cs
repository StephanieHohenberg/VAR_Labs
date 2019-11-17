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
	private string MESSAGE_SAME_COLOR_BEAT = "You can not beat your own figures.";
	private string MESSAGE_WHITE_WINS = "Checkmated. White wins. Congrats.";
	private string MESSAGE_BLACK_WINS = "Checkmated. Black wins. Congrats.";
	private string MESSAGE_CHECK = "Check !";

	public Text turnDisplayText;
	private int amountOfTurns = 0;

	private GameObject selectedFigure;

	Ray ray;
	RaycastHit hit;

    void Update()
    {
    	 ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         if(Physics.Raycast(ray, out hit)) {
         	GameObject hitGameObject = hit.transform.gameObject;
			if(Input.GetMouseButtonDown(0)) {
			    print(hitGameObject.tag);

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
    		if(selectedFigure) {
    			makeTurnBeating(hitGameObject);
    		} else {
    			displayMessage(MESSAGE_NOT_YOUR_TURN);
    		}
    	}
    }


    private void selectFigure(GameObject hitGameObject) {
    	// deselect last figure
		if(selectedFigure) {
			selectedFigure.transform.Translate(new Vector3(0, -15, 0));
		}

		// select figure
		selectedFigure = hitGameObject;
		hit.transform.gameObject.transform.Translate(new Vector3(0, 15, 0));

		// TODO visualize options
    }


    private void onClickOnBoard(RaycastHit hit) {
    	if(selectedFigure) {
    		makeTurnMoving(hit);
    	}
    }

    private void makeTurnMoving(RaycastHit hit) {
    	    // TODO validate move
			// TODO check whether field is occupied - by own color -> display message, by opposite color - beat

    		// TODO normalize/centralize position
    		selectedFigure.transform.position = new Vector3((float)Math.Floor(hit.point.x), -80, (float)Math.Floor(hit.point.z));
    		nextTurn();
    		
    }

    private void makeTurnBeating(GameObject hitGameObject) {
    	// TODO validate move
    	// TODO detect field

    	checkWhetherKingGotBeaten(hitGameObject);
    	hitGameObject.SetActive(false);
    	selectedFigure.transform.position = hitGameObject.transform.position;
    	//TODO geilere Animation als nur ersetzen ! 
    	nextTurn();
    }

    private void checkWhetherKingGotBeaten(GameObject hitGameObject) {
    	if(hitGameObject.tag == "King") {
    		string message = hasWhiteTurn() ? MESSAGE_WHITE_WINS : MESSAGE_BLACK_WINS;
    		displayMessage(message);
    	}
    }

    private void nextTurn() {
    	// TODO validate whether opposite King is threaten

    	selectedFigure = null;
    	amountOfTurns += 1;
    	turnDisplayText.text = hasWhiteTurn() ? MESSAGE_WHITE_TURN : MESSAGE_BLACK_TURN;
    }


    private bool hasGameObjectColorOfCurrentPlayer(GameObject gameObject) {
		return (hasWhiteTurn() && gameObject.transform.parent.tag == "White") || (!hasWhiteTurn() && gameObject.transform.parent.tag == "Black"); 	
    }

    private bool hasWhiteTurn() {
    	return (amountOfTurns % 2) == 0 ? true : false; 
    } 

    private void displayMessage(string text) {
    	turnDisplayText.text = text;
    }
}
