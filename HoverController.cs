using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour {

    public LevelData level;

    // Use this for initialization
    public void InstantiateHoverGrid () 
    {

        level.HoverData = new GameObject[level.squaresX, level.squaresY];
        for (int i = 0; i < level.squaresX; i++)
        {
            for (int j = 0; j < level.squaresY; j++)
            {
                level.HoverData[i, j] = level.HoverSquare;
            }
        }
        
    }

    public void bombShapeHighlight(List<Position> affectedArea)
    {
        foreach (Position space in affectedArea)
        {
            setBlue(space);
        }
    }

    public void bombShapeHighlightRed(List<Position> affectedArea)
    {
        foreach (Position space in affectedArea)
        {
            setRed(space);
        }
    }

    public void resetBombShapeHighlight(List<Position> affectedArea)
    {
        foreach (Position space in affectedArea)
        {
            resetSpace(space);
        }
    }

    public void resetSpace(Position pos)
    {
        level.HoverObjects[pos.x, pos.y].SetActive(false);
    }

    // TODO: Change color to designer's preference
    public void setGreen(Position pos)
    {
        level.HoverObjects[pos.x, pos.y].GetComponent<Renderer>().material.color = new Color(66 / 255f, 244 / 255f, 80 / 255f, 0.4f);
        level.HoverObjects[pos.x, pos.y].SetActive(true);
    }

    // TODO: Change color to designer's preference
    public void setRed(Position pos)
    {
        level.HoverObjects[pos.x, pos.y].GetComponent<Renderer>().material.color = new Color(242 / 255f, 70 / 255f, 78 / 255f, 0.4f);
        level.HoverObjects[pos.x, pos.y].SetActive(true);
    }

    // TODO: Change color to designer's preference
    public void setDarkRed(Position pos)
    {
        level.HoverObjects[pos.x, pos.y].GetComponent<Renderer>().material.color = new Color(242 / 255f, 70 / 255f, 78 / 255f, 0.8f);
        level.HoverObjects[pos.x, pos.y].SetActive(true);
    }

    // TODO: Change color to designer's preference
    public void setBlue(Position pos)
    {
        level.HoverObjects[pos.x, pos.y].GetComponent<Renderer>().material.color = new Color(70 / 255f, 204 / 255f, 242 / 255f, 0.4f);
        level.HoverObjects[pos.x, pos.y].SetActive(true);
    }
}
