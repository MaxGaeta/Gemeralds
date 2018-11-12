using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* *
 * This class handles all the logic of the game, such as which blocks to destroy
 * This class should not handle anything user oriented
 * */
public class GameLogic : MonoBehaviour {

    public LevelData level;
    public LevelController levelC;
    public BombController bombC;
    public MouseController mouse;
    public HoverController hover;
    public Events events;
    public Tutorial tutor;
    public InventoryController inventoryC;

    private Vector3 mousePos;
    private Position prevHover;
    private List<Position> prevArea;
    private List<Position> clearingBombSpaces;
    private Position bombOrigin;
    private bool hitGem;
    private bool hitDeathRock;
    private bool validClear;
    private bool defaultState;
    private bool finishedAnimation;
    private int bombIndex;

    private float explosionDelay = .15f;

    private void Start()
    {
        bombIndex = -1;
        events.hideLevelButtons.Invoke();
    }

    public void initializeGameLogic() {
        events.hideLevelButtons.Invoke();
    }

    public void validBombPlaceAnimation()
    {
        StartCoroutine(validBombPlaceEnumerator());
        validClear = false;
    }

    private IEnumerator validBombPlaceEnumerator(){
        List<Position>[] distSpaces = groupByDistance(clearingBombSpaces, bombOrigin);

        for (int i = 0; i < distSpaces.Length; i++)
        {
            List<Position> group = distSpaces[i];
            if (group.Count != 0)
            {

                foreach (Position space in group)
                {
                    int x = space.x;
                    int y = space.y;

                    Vector3 pos = level.getBoardPosition(x, y);

                    if (level.BoardData[x, y] == level.Rock)
                    {
                        Instantiate(level.Explosion, pos, Quaternion.Euler(-90f, 0f, Random.Range(0, 3) * 90f));
                        Destroy(level.Objects[x, y]);
                        level.Objects[x, y] = null;
                    }
                    else if (level.BoardData[x, y] == level.Gem)
                    {
                        level.gemCollected = level.gemCollected + 1;
                        level.Objects[x, y].GetComponent<GemAnimation>().CollectGem(level.gemCollected);
                        level.Objects[x, y] = null;
                    }
                }
                yield return new WaitForSeconds(explosionDelay);
            }
        }
        finishedAnimation = true;
    }

    public void blowUpGemAnimation() 
    {
        StartCoroutine(blowUpGemEnumerator());
        hitGem = false;
    }

    private IEnumerator blowUpGemEnumerator(){

        int size = 0;
        foreach (Position space in clearingBombSpaces)
        {
            size = size + 1;
        }
        int n = 0;

        List<Position>[] distSpaces = groupByDistance(clearingBombSpaces, bombOrigin);

        for (int i = 0; i < distSpaces.Length; i++)
        {
            List<Position> group = distSpaces[i];
            if (group.Count != 0)
            {

                foreach (Position space in group)
                {
                    int x = space.x;
                    int y = space.y;

                    Vector3 pos = level.getBoardPosition(x, y);

                    if (level.BoardData[x, y] == level.Rock)
                    {
                        Instantiate(level.Explosion, pos, Quaternion.Euler(-90f, 0f, Random.Range(0, 3) * 90f));
                        Destroy(level.Objects[x, y]);
                        level.Objects[x, y] = null;
                    }
                    else if (level.BoardData[x, y] == level.Gem)
                    {
                        if (n == size - 1)
                        {
                            level.Objects[x, y].GetComponent<GemAnimation>().DestroyGem();
                            level.Objects[x, y] = null;
                        }
                        else
                        {
                            level.gemCollected = level.gemCollected + 1;
                            level.Objects[x, y].GetComponent<GemAnimation>().CollectGem(level.gemCollected);
                            level.Objects[x, y] = null;
                        }
                    }
                    n = n + 1;
                }
                yield return new WaitForSeconds(explosionDelay);
            }
        }
        finishedAnimation = true;
    }

    //clears the spaces when given a bomb area (spaces)
    public List<Position> clearingSpaces(List<Position> spaces, Position origin) {

        List<Position> clearedSpaces = new List<Position>();
        List<Position>[] distSpaces = groupByDistance(spaces, origin);

        for (int i = 0; i < distSpaces.Length; i++)
        {
            List<Position> group = distSpaces[i];

            foreach (Position space in group)
            {
                int x = space.x;
                int y = space.y;

                //Clear object 
                GameObject target = level.inBounds(x, y) ? level.Objects[space.x, space.y] : null;
                if (target)
                {
                    //need to check if next bombs to be destroyed will hit gems 
                    //since they explode outwards 
                    if (i < distSpaces.Length - 1)
                    {
                        List<Position> nextGroup = distSpaces[i+1];             
                        Position doesHitG = doesHitGem(nextGroup);
                        Position doesHitDR = doesHitDeathRock(nextGroup);
                        if (doesHitG !=null) {
                            hitGem = true;
                            clearedSpaces.Add(new Position(x,y));
                            clearedSpaces.Add(doesHitG);
                            return clearedSpaces;
                        }
             
                        else if (doesHitDR !=null) {
                            hitDeathRock = true;
                            clearedSpaces.Add(new Position(x,y));
                            clearedSpaces.Add(doesHitDR);
                            return clearedSpaces;
                        }
                        
                        else if (level.BoardData[space.x, space.y] == level.Rock)
                        {     
                            clearedSpaces.Add(new Position(x,y));
                            collectGem(x, y,clearedSpaces);              
                            
                        }
                    }
                    else if (level.BoardData[space.x, space.y] == level.Rock)
                    {      
                        clearedSpaces.Add(new Position(x,y));   
                        collectGem(x, y,clearedSpaces);                                       
                    }
                }
            }
        }

        validClear = true;
        return clearedSpaces;
    }

    // Returns a list of spaces grouped by their distance from the origin
    private List<Position>[] groupByDistance (List<Position> spaces, Position origin) {

        int maxDistance = (level.squaresX + 1) * (level.squaresY + 1);

        List<Position>[] distSpaces = new List<Position>[maxDistance];
        for (int i = 0; i < maxDistance; i++)
        {
            distSpaces[i] = new List<Position>();
        }

        foreach (Position space in spaces)
        {
            int dist = Position.Distance(space, origin);
            distSpaces[dist].Add(space);
        }

        return distSpaces;

    }

    //returns true if the given bomb spaces are collectable gems
    private Position doesHitGem(List<Position> bombs) {
        foreach (Position bomb in bombs) {
            if (level.inBounds(bomb.x, bomb.y) && level.Objects[bomb.x, bomb.y]){
                if (level.BoardData[bomb.x, bomb.y] == level.Gem) {
                    return new Position(bomb.x, bomb.y);
                }
            }
        }
        return null;
    }

    private Position doesHitDeathRock(List<Position> bombs) {
        foreach (Position bomb in bombs) {
            if (level.inBounds(bomb.x, bomb.y) && level.Objects[bomb.x, bomb.y]){
                if (level.BoardData[bomb.x, bomb.y] == level.DeathRock) {
                    return new Position(bomb.x, bomb.y);
                }
            }
        }
        return null;
    }

    public bool containsPos(Position check,List<Position> list)
    {
        foreach (Position space in list)
        {
            if (space.x == check.x && space.y == check.y) 
            {
                return true;
            }
        }
        return false;
    }

    /**
     * Checks if the given Position is in bounds and contains a Gem
     * If it does, it adds the gem to the spaces to be cleared
     * @params x, y the Position to check
     * */
    private void GetGem(int x, int y,List<Position> spaces)
    {
        if (level.inBounds(x, y) && //if in bounds
              level.BoardData[x, y] == level.Gem && //if object at position is a gem
              !containsPos(new Position(x,y),spaces)&&
              level.Objects[x,y]) //if the gem is not already collected
        {

            spaces.Add(new Position(x,y));
            //check the 4 positions around the collected gem for more gems
            collectGem(x, y,spaces);
        }
    }

    /**
     * Checks if there are any gems in the 4 spaces around a Position
     * If there are, collect those gems
     * @params x, y the Position to check
     * */
    public void collectGem(int x, int y,List<Position> spaces)
    {
        //collect gems in the 4 spaces around the position
        GetGem(x - 1, y,spaces);
        GetGem(x + 1, y,spaces);
        GetGem(x, y - 1,spaces);
        GetGem(x, y + 1,spaces);
        
    }
    
    public void setOutOfBombText() 
    {
        int bombCount = 0;
        for (int i = 0;i < level.invSize;i++) {
            bombCount = bombCount+level.InventoryArray[i].GetQuant();
        }
        
        if (level.gemCollected < level.gemTotal && bombCount == 0) {
           level.outOfBombText.text = "You ran out of bombs!";
        }
    }

    public void stateReset()
    {
        defaultState = true;
    }

    public bool getDefaultState()
    {
        return defaultState;
    }

    public void resetDefaultState()
    {
        defaultState = false;
    }

    public bool getFinishedAnimation()
    {
        return finishedAnimation;
    }

    public void resetAnimation()
    {
        finishedAnimation = false;
    }

    public void setCountText()
    {
        if (level.gemCollected >= level.gemTotal) {
            level.winText.text = "You Win!";
            level.gemCollected = 0;
            events.onLevelComplete.Invoke();
        }
    }

    public Vector3 getLogicMousePos() {
        return mousePos;
    }

    // Handles hover
    public void hovering () {
        mousePos = mouse.getMousePos();
        if (bombIndex != -1 && mouse.inGameBoard(mousePos)) 
        {
            Position gridplace = mouse.convertMousePosToGrid(mousePos);
            Bomb selectedBomb = level.InventoryArray[bombIndex];
            List<Position> affectedArea = bombC.GetArea(selectedBomb, gridplace.x, gridplace.y);

            // TODO: Color logic may change after further discussion
            // First hover time
            if (prevHover == null)
            {
                if (prevArea != null) hover.resetBombShapeHighlight(prevArea);
                // Free space
                if (level.Objects[gridplace.x, gridplace.y] == null)
                {
                    hover.bombShapeHighlight(affectedArea);
                    hover.setGreen(gridplace);
                }
                // Non-free space
                else
                {
                    hover.bombShapeHighlightRed(affectedArea);
                    hover.setDarkRed(gridplace);
                }
            }
            // New hover location
            else if (gridplace.x != prevHover.x || gridplace.y != prevHover.y)
            {
                hover.resetBombShapeHighlight(prevArea);
                // Free space
                if (level.Objects[gridplace.x, gridplace.y] == null)
                {
                    hover.resetSpace(prevHover);
                    hover.bombShapeHighlight(affectedArea);
                    hover.setGreen(gridplace);

                }
                // Non-free space
                else 
                {
                    hover.resetSpace(prevHover);
                    hover.bombShapeHighlightRed(affectedArea);
                    hover.setDarkRed(gridplace);
                }

            }
            prevHover = gridplace;
            prevArea = affectedArea;
        }
        else if (prevHover != null)
        {
            hover.resetSpace(prevHover);
            hover.resetBombShapeHighlight(prevArea);
            prevHover = null;
            prevArea = null;
        }
    }


    //if a bomb is place returns true
    public bool hasPlacedBomb() {
        if (Input.GetMouseButtonDown(0)) 
        {
            mousePos = mouse.getMousePos();
            if (mouse.inGameBoard(mousePos))
            {
                Position gridplace = mouse.convertMousePosToGrid(mousePos);
                if (!level.Objects[gridplace.x, gridplace.y])
                {
                    return true;
                }
            }
        }
        return false;
    }

    //returns the index of the new bomb if selected.
    //returns -1 if you deselect the bomb (click outside inventory and gameboard)
    //else returns the index of the previously clicked bomb
    public int checkPlaceOrDeselect() 
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            mousePos = mouse.getMousePos();
            if (mouse.inGameBoard(mousePos))
            {
                Position gridplace = mouse.convertMousePosToGrid(mousePos);

                if (!level.Objects[gridplace.x, gridplace.y])
                {
                    return bombIndex;
                }

            }
            else if (!mouse.inInventory(mousePos))
            {                           
                inventoryC.deselectBomb();
                return -1;
            }
        }
        return bombIndex;
    }

    //used in the start state to check whether a bomb has been selected
    public int selectingBomb() 
    {
        // If a bomb is selected and bombs > 0 
        if (bombIndex != -1 && level.InventoryArray[bombIndex].GetQuant() > 0)
        {
            if (tutor.active && tutor.waitingOnBomb == bombIndex)
            {
                tutor.lvlprog += 1;
                tutor.waitTime = 1;
            }
            return bombIndex;
		}
        
        return -1;
    }


    //used in the placing bomb state. 
    //responsible for clearing spaces and other placing bomb functions  
    public void placingBomb() 
    {
        Position gridplace = mouse.convertMousePosToGrid(mousePos);

        if (!level.Objects[gridplace.x, gridplace.y])
        {
            Bomb selectedBomb = level.InventoryArray[bombIndex];
            List<Position> affectedArea = bombC.GetArea(selectedBomb, gridplace.x, gridplace.y);

            // Remove hover highlight
            hover.resetSpace(gridplace);
            hover.resetBombShapeHighlight(affectedArea);

            // Decrement inventory
            int prevQuant = selectedBomb.GetQuant();
            selectedBomb.SetQuant(prevQuant - 1);

            // Clear out affected area
            bombOrigin = gridplace;
            clearingBombSpaces = clearingSpaces(affectedArea, gridplace);

            // Remove visual highlights
            inventoryC.deselectBomb();
            inventoryC.updateBombTextCount();
            inventoryC.updateBombColor();

            //Update Tutorial if applicable
            if (tutor.active)
            {
                tutor.lvlprog += 1;
                if (tutor.lvlprog == 2) tutor.waitTime = 1;
            }
        }
    }

    public bool checkWin() 
    {
        return level.gemCollected >= level.gemTotal;
    }

    public bool checkNoBombsLeftLoss()
    {
        int bombCount = 0;
        for (int i = 0;i < level.invSize;i++) {
            bombCount = bombCount+level.InventoryArray[i].GetQuant();
        }
        return level.gemCollected < level.gemTotal && bombCount == 0;
    }

    public void winState() 
    {
        level.winText.text = "You Win!";
        level.gemCollected = 0;
        events.onLevelComplete.Invoke();
    }

    public void loseByBlowUpGem()
    {
        level.brokeGemText.text = "Oh no! You broke a gem!";
    }

    public void loseByBlowUpDeathRock()
    {
        level.brokeGemText.text = "Oh no! You blew up a death rock!";
    }

    public void noBombsLeftLossState() 
    {
        level.outOfBombText.text = "You ran out of bombs!";
        level.gemCollected = 0;
    }

    public List<Position> getClearingBombSpaces()
    {
        return clearingBombSpaces;
    }

    public bool getHitGem() 
    {
        return hitGem;
    }

    public bool getHitDeathRock()
    {
        return hitDeathRock;
    }

    public bool getValidClear() 
    {
        return validClear;
    }

    public void checkReset() {
        if (Input.GetKeyDown(KeyCode.R))
        {
            levelC.ResetLevel();
        }
    }

    public void checkNextLevel() {
        if (Input.GetKeyDown(KeyCode.N))
        {
            levelC.NextLevel();
        }
    }

    public void updateBombTextCount() {
        inventoryC.updateBombTextCount();
    }

    public void updateBombColor() {
        inventoryC.updateBombColor();
    }

    public void setBombIndex(int newIndex)
    {
        bombIndex = newIndex;
    }
}
