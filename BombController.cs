using System.Collections.Generic;
using UnityEngine;

/**
 * This class handles the creation of Bomb Objects and handling which bomb is currently selected
 * */
public class BombController : MonoBehaviour
{
    public LevelData level;
    public GameLogic logic;
    public GameObject Bomb;
    
    /**
     * Creates the bomb object
     * @returns the Bomb GameObject
     * */
    public GameObject createBomb(GameObject Parent, float bombStart)
    {
        Vector3 pos = new Vector3(bombStart, 5f, level.getInventoryMidpoint());
        GameObject bomb = Instantiate(Bomb, pos, Quaternion.identity);
        bomb.transform.SetParent(Parent.transform);

        return bomb;
    }

    /** 
     * Returns the list of grid spaces to be destroyed by the bomb
     * @params bomb the bomb type
     * @params x, y the coordinates for placement of bomb
     * @returns the coordinates of the board which are affected by the bomb
     * */
    public List<Position> GetArea(Bomb bomb, int x, int y)
    {
        //The list of grid spaces to be destroyed by the bomb
        List<Position> destroyedArea = new List<Position>();

        //Add on to list depending on bomb type
        switch(bomb.GetShape())
        {
            case "1x5":
                for (int j = -2; j <= 2; j++)
                {
                    if (level.inBounds(x, y + j))
                    {
                        destroyedArea.Add(new Position(x, y + j));
                    }
                }
                break;
            case "5x1":
                for (int i = -2; i <= 2; i++)
                {
                    if (level.inBounds(x + i, y))
                    {
                        destroyedArea.Add(new Position(x + i, y));
                    }
                }
                break;
                //TODO:Remove 2x1
            case "2x1_up":
                for (int i = -1; i <= 0; i++)
                {
                    if (level.inBounds(x + i, y))
                    {
                        destroyedArea.Add(new Position(x + i, y));
                    }
                }
                break;
            case "5x5_cross":
                for (int i = -2; i <= 2; i++)
                {
                    for (int j = -2; j <= 2; j++)
                    {
                        if ((i == 0 || j == 0) && level.inBounds(x + i, y + j))
                        {
                            destroyedArea.Add(new Position(x + i, y + j));
                        }
                    }
                }
                break;
            case "infinite_cross":
                for (int i = 0; i < level.squaresX; i++)
                {
                    for (int j = 0; j < level.squaresY; j++)
                    {
                        if (i == x || j == y)
                        {
                            destroyedArea.Add(new Position(i, j));
                        }
                    }
                }
                break;
            case "1x3":
                for (int j = -1; j <= 1; j++)
                {
                    if (level.inBounds(x, y + j))
                    {
                        destroyedArea.Add(new Position(x, y + j));
                    }
                }
                break;
            case "3x1":
                for (int i = -1; i <= 1; i++)
                {
                    if (level.inBounds(x + i, y))
                    {
                        destroyedArea.Add(new Position(x + i, y));
                    }
                }
                break;
            case "infinite_horizontal":
                for (int j = 0; j < level.squaresY; j++)
                {
                    destroyedArea.Add(new Position(x, j));
                }
                break;
            case "infinite_vertical":
                for (int i = 0; i < level.squaresX; i++)
                {
                    destroyedArea.Add(new Position(i, y));
                }
                break;
        }
        return destroyedArea;
    }


}