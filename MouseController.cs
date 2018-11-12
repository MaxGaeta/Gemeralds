using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class handles translating mouse data into game data
 * This class should not handle any Game Logic
 * 
 * GameLogic calls this class's MouseDown and polls this class's BombSelected (which bomb selectedby mouse), BoardSquareSelected(which board square the mouse is on), etc.
 * */
public class MouseController : MonoBehaviour {

    public LevelData level;
    public BombController bombC;
    public GameLogic logic;

    // Use this for initialization
    private void Start () {
    }
   
    public Vector3 getMousePos() {
        Vector3 screenPos = Input.mousePosition;
        Camera cam = Camera.main;

        return cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, cam.nearClipPlane));

    }

    public Position convertMousePosToGrid(Vector3 mousePos) {
    
        float height = level.botX - level.topX;
        float width = level.rightZ - level.leftZ;
        return inGameBoard(mousePos)
          ? new Position(
                Mathf.RoundToInt((level.squaresX*(mousePos.x - level.topX) - (height/2))/height),
              Mathf.RoundToInt((level.squaresY * (mousePos.z - level.leftZ) - (width / 2)) / width)
          )
          : new Position(-99, -99);

    }

    public bool inGameBoard(Vector3 pos)
    {

        return (pos.x >= level.topX &&
                pos.x <= level.botX &&
                pos.z >= level.leftZ &&
                pos.z <= level.rightZ
               );

    }

    public bool inInventory(Vector3 pos) {

        return (pos.x >= level.inventoryTopX &&
                pos.x <= level.inventoryBotX &&
                pos.z >= level.inventoryRightZ &&
                pos.z <= level.inventoryLeftZ
               );

    }
}
