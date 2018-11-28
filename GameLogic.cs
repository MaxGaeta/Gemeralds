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

    public bool ABTestValue;

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
    private bool finishedGemCollection;
    private bool finishedGemDestroy;
    private int bombIndex;

    private float explosionDelay = .15f;

    private void Start()
    {
        bombIndex = -1;
        events.hideLevelButtons.Invoke();

        ABTestValue = false; /*LoggingManager.instance.AssignABTestValue(Random.Range(0, 2)) == 1;*/
        LoggingManager.instance.RecordABTestValue();
    }
    
    public void initializeGameLogic() {
        events.hideLevelButtons.Invoke();
    }

    public void validBombPlaceAnimation()
    {
        screenShake.shakeTime = 0.6f;
        StartCoroutine(validBombPlaceEnumerator());
        validClear = false;
    }

    private IEnumerator validBombPlaceEnumerator(){
        List<Position>[] distSpaces = groupByDistance(clearingBombSpaces, bombOrigin);

        Position sink = null;

        for (int i = 1; i < distSpaces.Length; i++)
        {
            List<Position> group = distSpaces[i];
            if (group.Count != 0)
            {

                foreach (Position space in group)
                {
                    int x = space.x;
                    int y = space.y;

                    Vector3 pos = level.getBoardPosition(x, y);

                    if (level.Objects[x, y] == null) {
                        Instantiate(level.Explosion, pos, Quaternion.Euler(-90f, 0f, Random.Range(0, 3) * 90f));
                        Destroy(level.Objects[x, y]);
                    }

                    else if (level.BoardData[x, y] == level.Rock)
                    {
                        Instantiate(level.Explosion, pos, Quaternion.Euler(-90f, 0f, Random.Range(0, 3) * 90f));
                        Destroy(level.Objects[x, y]);
                        level.Objects[x, y] = null;
                    }

                    else if (level.BoardData[x, y] == level.Sinkhole) {
                        sink = new Position(x,y);
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
        if (sink != null) 
        {
            Suck(sink);
        }

        finishedAnimation = true;
    }

    public void blowUpGemAnimation() 
    {
        screenShake.shakeTime = 0.6f;
        StartCoroutine(blowUpGemEnumerator());
        hitGem = false;
    }

     private IEnumerator blowUpGemEnumerator(){

        int size = 0;
        foreach (Position space in clearingBombSpaces)
        {
            size = size + 1;
        }
        int n = 1;
        Position sink = null;

        List<Position>[] distSpaces = groupByDistance(clearingBombSpaces, bombOrigin);

        for (int i = 1; i < distSpaces.Length; i++)
        {
            List<Position> group = distSpaces[i];
            if (group.Count != 0)
            {

                foreach (Position space in group)
                {
                    int x = space.x;
                    int y = space.y;

                    Vector3 pos = level.getBoardPosition(x, y);

                    if (level.Objects[x, y] == null) {
                        Instantiate(level.Explosion, pos, Quaternion.Euler(-90f, 0f, Random.Range(0, 3) * 90f));
                        Destroy(level.Objects[x, y]);
                    }

                    else if (level.BoardData[x, y] == level.Rock)
                    {
                        Instantiate(level.Explosion, pos, Quaternion.Euler(-90f, 0f, Random.Range(0, 3) * 90f));
                        Destroy(level.Objects[x, y]);
                        level.Objects[x, y] = null;
                    }

                    else if (level.BoardData[x, y] == level.Sinkhole) {
                        sink = new Position(x,y);
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
        if (sink != null) 
        {
            Suck(sink);
        }
        finishedAnimation = true;
    }


    public void blowUpDeathRockAnimation() 
    {
        screenShake.shakeTime = 0.6f;
        StartCoroutine(blowUpDeathRockEnumerator());
        hitDeathRock = false;
    }

    private IEnumerator blowUpDeathRockEnumerator(){

        List<Position>[] distSpaces = groupByDistance(clearingBombSpaces, bombOrigin);
        Position sink = null;

        for (int i = 1; i < distSpaces.Length; i++)
        {
            List<Position> group = distSpaces[i];
            if (group.Count != 0)
            {

                foreach (Position space in group)
                {
                    int x = space.x;
                    int y = space.y;

                    Vector3 pos = level.getBoardPosition(x, y);

                    if (level.Objects[x, y] == null) {
                        Instantiate(level.Explosion, pos, Quaternion.Euler(-90f, 0f, Random.Range(0, 3) * 90f));
                        Destroy(level.Objects[x, y]);
                    }

                    else if (level.BoardData[x, y] == level.Rock)
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

                    else if (level.BoardData[x, y] == level.Sinkhole) {
                        sink = new Position(x,y);
                    }

                    else if (level.BoardData[x,y] == level.DeathRock)
                    {
                        Instantiate(level.ExplosionPoison, pos, Quaternion.Euler(-90f, 0f, Random.Range(0, 3) * 90f));
                        Destroy(level.Objects[x, y]);
                        level.Objects[x, y] = null;
                        // level.Objects[x, y].GetComponent<GemAnimation>().DestroyGem();
                        // level.Objects[x, y] = null;
                    }
                }
                yield return new WaitForSeconds(explosionDelay);
            }
        }
        if (sink != null) 
        {
            Suck(sink);
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

                GameObject target = level.inBounds(x, y) ? level.Objects[space.x, space.y] : null;
                
                if (level.BoardData[space.x, space.y] == level.DeathRock) 
                {
                    hitDeathRock = true;
                    clearedSpaces.Add(new Position(x,y));
                    return clearedSpaces;
                }
                else if (target)
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
                            foreach (Position s in group){
                                clearedSpaces.Add(new Position(s.x,s.y));                                
                            }  
                            // clearedSpaces.Add(new Position(x,y));
                            clearedSpaces.Add(doesHitG);
                            return clearedSpaces;
                        }
             
                        else if (doesHitDR !=null) {
                            hitDeathRock = true;
                            foreach (Position s in group){
                                clearedSpaces.Add(new Position(s.x,s.y));
                            }                            
                            clearedSpaces.Add(doesHitDR);
                            return clearedSpaces;
                        }
                        
                        else if (level.BoardData[space.x, space.y] == level.Rock)
                        {     
                            clearedSpaces.Add(new Position(x,y));
                            collectGem(x, y,clearedSpaces);              
                            
                        }

                        else if (level.BoardData[x, y] == level.Sinkhole) {
                            clearedSpaces.Add(new Position(x,y)); 
                        }
                    }
                    else if (level.BoardData[space.x, space.y] == level.Rock)
                    {      
                        clearedSpaces.Add(new Position(x,y));   
                        collectGem(x, y,clearedSpaces);                                       
                    }

                    else if (level.BoardData[x, y] == level.Sinkhole) {
                        clearedSpaces.Add(new Position(x,y)); 
                    }

                    else if (level.BoardData[space.x, space.y] == level.DeathRock) 
                    {
                        hitDeathRock = true;
                        clearedSpaces.Add(new Position(x,y));
                        return clearedSpaces;
                    }
                }

                else {
                    clearedSpaces.Add(new Position(x,y));   
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

    //if one of the spaces is a gem then it will return the position of that space
    //returns null if none of the sapces in bombs are gems
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

    //if one of the spaces is a death rock then it will return the position of that space
    //returns null if none of the sapces in bombs are deathrocks 
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
        level.deathRockText.text = "";
        defaultState = true;
        setBombIndex(-1);
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

    public bool getFinishedGemCollection()
    {
        return finishedGemCollection;
    }

    public bool getFinishedGemDestroy()
    {
        return finishedGemDestroy;
    }

    public void resetAnimation()
    {
        finishedAnimation = false;
        finishedGemDestroy = false;
        finishedGemCollection = false;
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
                if (prevArea != null) hover.resetAllHover();
                // Free space
                if (level.Objects[gridplace.x, gridplace.y] == null)
                {
                    hover.bombShapeHighlightFree(affectedArea);
                    hover.setMiddleBlockFree(gridplace, selectedBomb);
                }
                // Non-free space
                else
                {
                    hover.bombShapeHighlight(affectedArea);
                    hover.setMiddleBlock(gridplace, selectedBomb);
                }
            }
            // New hover location
            else
            {
                hover.resetAllHover();
                // Free space
                if (level.Objects[gridplace.x, gridplace.y] == null)
                {
                    hover.resetSpace(prevHover);
                    hover.bombShapeHighlightFree(affectedArea);
                    hover.setMiddleBlockFree(gridplace, selectedBomb);

                }
                // Non-free space
                else 
                {
                    hover.resetSpace(prevHover);
                    hover.bombShapeHighlight(affectedArea);
                    hover.setMiddleBlock(gridplace, selectedBomb);
                }

            }
            prevHover = gridplace;
            prevArea = affectedArea;
        }
        else if (prevHover != null)
        {
            hover.resetSpace(prevHover);
            hover.resetAllHover();
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
        
        return bombIndex;
    }


    //used in the placing bomb state. 
    //responsible for clearing spaces and other placing bomb functions  
    public void placingBomb() 
    {
        Position gridplace = mouse.convertMousePosToGrid(mousePos);
        LoggingManager.instance.RecordEvent(1,"placed bomb " + bombIndex + " at " + gridplace);

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
            // inventoryC.deselectBomb();
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
        if (levelC.completedString != "") levelC.completedString += ", ";
        levelC.completedString += levelC.currentLevel;
        LoggingManager.instance.RecordEvent(3, "Completed Levels: [" + levelC.completedString + "]");
        level.gemCollected = 0;

        levelC.clearedLevels[levelC.currentLevel] = true;

        if (levelC.currentLevel == levelC.numLevels){
            level.winText.text = "Congrats! You have completed the final level!";
            events.onAllLevelsComplete.Invoke();
        }
        else if (ABTestValue && levelC.currentLevel > levelC.highestLevelCompleted)
        {
            levelC.highestLevelCompleted = levelC.currentLevel;
            level.winText.text = "You Win!";
            events.onLevelCompleteUnlock.Invoke();
        }
        else
        {
            level.winText.text = "You Win!";
            events.onLevelComplete.Invoke();
        }

    }

    public void loseByBlowUpGem()
    {
        LoggingManager.instance.RecordEvent(5, "broke gem");
        level.brokeGemText.text = "Oh no! You broke a gem!";
        events.onBreakGem.Invoke();
    }

    public void loseByBlowUpDeathRock()
    {
        LoggingManager.instance.RecordEvent(6, "hit death rock");
        level.deathRockText.text = "Oh no! You blew up a toxic crate!";
        events.onHitDeathRock.Invoke();
    }

    public void noBombsLeftLossState() 
    {
        LoggingManager.instance.RecordEvent(4, "ran out of bombs");
        level.outOfBombText.text = "You ran out of bombs!";
        level.gemCollected = 0;
        events.onNoBombsLeft.Invoke();
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

    public void setFinishedGemDestroy() 
    {
        finishedGemDestroy = true;

    }

    public void setFinishedGemCollection() 
    {
        finishedGemCollection = true;
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
            //Disabled for Friends Release & Newgrounds
            //levelC.NextLevel();
        }
    }

    public int selectNextBomb()
    {
        if (bombIndex >= 0) 
        {
            if (level.InventoryArray[bombIndex].GetQuant() > 0) {
                setBombIndex(bombIndex);
                inventoryC.selectBomb(bombIndex);
                return bombIndex;
            }

            else if (level.InventoryArray[((bombIndex+1)%level.invSize)].GetQuant() > 0)
            {
                int i = ((bombIndex+1)%level.invSize);
                setBombIndex(i);
                inventoryC.selectBomb(i);
                return i;
            }

            else {
                int i = ((bombIndex+2)%level.invSize);
                setBombIndex(i);
                inventoryC.selectBomb(i);
                return i;
            }
        }
        return -1;

    }

    public void checkBombSwitch()
    {
        if (level.InventoryArray[0].GetQuant() > 0 && Input.GetKeyDown(KeyCode.Alpha1))
        {
            setBombIndex(0);
            inventoryC.selectBomb(0);
        } 
        else if (level.invSize > 1 && level.InventoryArray[1].GetQuant() > 0 && Input.GetKeyDown(KeyCode.Alpha2))
        {
            setBombIndex(1);
            inventoryC.selectBomb(1);
        }
        else if (level.invSize > 2 && level.InventoryArray[2].GetQuant() > 0 && Input.GetKeyDown(KeyCode.Alpha3))
        {
            setBombIndex(2);
            inventoryC.selectBomb(2);
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

    [SerializeField]
    AnimationCurve movementCurve;
    const float timeToReachTargetDefault = 0.6f;
    private IEnumerator MoveToPosition(Transform transformToUse, Vector3 position,
                                        float timeToReachTarget = timeToReachTargetDefault,
                                        float beginningScaleFactor = 1f,
                                        float endingScaleFactor = 1f,
                                        float randomRotation = 0f)
    {
        Vector3 origPos = transformToUse.position;
        Vector3 origScale = transformToUse.localScale;
        Quaternion origRot = transformToUse.rotation;
        Quaternion targetRot = Random.rotation;
        float t = 0f;
        while (t < timeToReachTarget)
        {
            float lerpAmt = movementCurve.Evaluate(t / timeToReachTarget);
            transformToUse.position = Vector3.Lerp(origPos, position, lerpAmt);
            transformToUse.rotation = Quaternion.SlerpUnclamped(origRot, targetRot, lerpAmt * randomRotation);
            transformToUse.localScale = origScale *
                (lerpAmt * endingScaleFactor + (1 - lerpAmt) * beginningScaleFactor);

            t += Time.deltaTime;
            if (t >= timeToReachTarget)
            {
                transformToUse.position = position;
                transformToUse.localScale = origScale * endingScaleFactor;
            }
            yield return null;
        }
    }

    public void InvokeMoveToPosition(Transform transformToUse, Vector3 position,
                                        float timeToReachTarget = timeToReachTargetDefault,
                                        float beginningScaleFactor = 1f,
                                        float endingScaleFactor = 1f,
                                        float randomRotation = 0f)
    {
        StartCoroutine(MoveToPosition(transformToUse, position, timeToReachTarget,
                                     beginningScaleFactor, endingScaleFactor,
                                     randomRotation));
    }

    //width and height of the board currently
    private float height;
    private float width;
    private Vector3 sinkHoleOffset = new Vector3(0f, -4.5f, 0f);
    private void SpawnRock(int x, int y, bool spawnedBySinkhole = false)
    {

        Vector3 pos = new Vector3(level.topX + (height * x + height / 2f) / level.squaresX, 1f,
                                                level.leftZ + (width * y + width / 2f) / level.squaresY);
        GameObject rock = Instantiate(level.Rock, pos, Quaternion.Euler(-90f, 0f, Random.Range(0, 3) * 90f));
        rock.name = rock.name.Substring(0, rock.name.Length - 7) + "[" + x + ", " + y + "]";
        Vector3 scale = rock.transform.localScale;
        rock.transform.localScale = new Vector3(scale.x * 6.0f / level.squaresX, scale.y * 7.0f / level.squaresY, scale.x * 6.0f / level.squaresX);
        rock.transform.SetParent(level.gb.transform);
        level.Objects[x, y] = rock;
        if (spawnedBySinkhole)
        {
            Vector3 correctPos = rock.transform.position;
            rock.transform.position += sinkHoleOffset;
            StartCoroutine(MoveToPosition(rock.transform, correctPos, 1.1f, 0.4f, 1f));
        }
    }

    Vector3 sinkholeFallDistance = new Vector3(0, -3, 0);
    //Activates the sinkhole/black hole at Position pos
    public void Suck(Position pos)
    {
        level.Objects[pos.x, pos.y].GetComponent<sinkholeAnimation>().BeginAnimation();

        height = level.botX - level.topX;
        width = level.rightZ - level.leftZ;

        //suck from up outside in
        for (int i = 0; i < pos.x; i++)
        {
            if (level.Objects[i, pos.y])
            {
                // move towards next location
                Vector3 nextPos = new Vector3(level.topX + (height * (i + 1) + height / 2f) / level.squaresX, 1f,
                                                    level.leftZ + (width * pos.y + width / 2f) / level.squaresY);
                // blocks closest to center "fall"
                if (i == pos.x - 1)
                {
                    nextPos += sinkholeFallDistance;
                    StartCoroutine(MoveToPosition(level.Objects[i, pos.y].transform, nextPos,
                                                  beginningScaleFactor: 1f,
                                                  endingScaleFactor: 0.1f));
                }
                else
                {
                    StartCoroutine(MoveToPosition(level.Objects[i, pos.y].transform, nextPos));
                }
            }
        }
        //Destroy block closest to center
        if (level.inBounds(pos.x - 1, pos.y)) Destroy(level.Objects[pos.x - 1, pos.y], 0.6f);
        //Reassign objects center to outside
        for (int i = pos.x - 1; i > 0; i--)
        {
            level.Objects[i, pos.y] = level.Objects[i - 1, pos.y];
        }
        //Shift in a rock
        if (pos.x != 0 && level.inBounds(0, pos.y)) SpawnRock(0, pos.y, true);


        // suck from down outside in
        for (int i = level.squaresX - 1; i > pos.x ; i--)
        {
            if (level.Objects[i, pos.y])
            {
                // move towards next location
                Vector3 nextPos = new Vector3(level.topX + (height * (i - 1) + height / 2f) / level.squaresX, 1f,
                                                    level.leftZ + (width * pos.y + width / 2f) / level.squaresY);

                // blocks closest to center "fall"
                if (i == pos.x + 1)
                {
                    nextPos += sinkholeFallDistance;
                    StartCoroutine(MoveToPosition(level.Objects[i, pos.y].transform, nextPos,
                                                  beginningScaleFactor: 1f,
                                                  endingScaleFactor: 0.1f));
                }
                else
                {
                    StartCoroutine(MoveToPosition(level.Objects[i, pos.y].transform, nextPos));
                }
            }
        }
        //Destroy block closest to center
        if (level.inBounds(pos.x + 1, pos.y)) Destroy(level.Objects[pos.x + 1, pos.y], 0.6f);
        //Reassign objects center to outside
        for (int i = pos.x + 1; i < level.squaresX - 1; i++)
        {
            level.Objects[i, pos.y] = level.Objects[i + 1, pos.y];
        }
        //Shift in a rock
        if (pos.x != level.squaresX - 1 && level.inBounds(level.squaresX - 1, pos.y)) SpawnRock(level.squaresX - 1, pos.y, true);


        // suck from left outside in
        for (int j = 0; j < pos.y; j++)
        {
            if (level.Objects[pos.x, j])
            {
                // move towards next location
                Vector3 nextPos = new Vector3(level.topX + (height * pos.x + height / 2f) / level.squaresX, 1f,
                                                        level.leftZ + (width * (j + 1) + width / 2f) / level.squaresY);
                // blocks closest to center "fall"
                if (j == pos.y - 1)
                {
                    nextPos += sinkholeFallDistance;
                    StartCoroutine(MoveToPosition(level.Objects[pos.x, j].transform, nextPos,
                                                  beginningScaleFactor: 1f,
                                                  endingScaleFactor: 0.1f));
                }
                else
                {
                    StartCoroutine(MoveToPosition(level.Objects[pos.x, j].transform, nextPos));

                }
            }
        }
        //Destroy block closest to center
        if (level.inBounds(pos.x, pos.y - 1)) Destroy(level.Objects[pos.x, pos.y - 1], 0.6f);
        //Reassign objects center to outside
        for (int j = pos.y - 1; j > 0; j--)
        {
            level.Objects[pos.x, j] = level.Objects[pos.x, j - 1];
        }
        //Shift in a rock
        if (pos.y != 0 && level.inBounds(pos.x, 0)) SpawnRock(pos.x, 0, true);


        // suck from right outside in
        for (int j = level.squaresY - 1; j > pos.y; j--)
        {
            if (level.Objects[pos.x, j])
            {
                // move towards next location
                Vector3 nextPos = new Vector3(level.topX + (height * pos.x + height / 2f) / level.squaresX, 1f,
                                                    level.leftZ + (width * (j - 1) + width / 2f) / level.squaresY);
                // blocks closest to center "fall"
                if (j == pos.y + 1)
                {
                    nextPos += sinkholeFallDistance;
                    StartCoroutine(MoveToPosition(level.Objects[pos.x, j].transform, nextPos,
                                                  beginningScaleFactor: 1f,
                                                  endingScaleFactor: 0.1f));
                }
                else
                {
                    StartCoroutine(MoveToPosition(level.Objects[pos.x, j].transform, nextPos));

                }

            }
        }
        //Destroy block closest to center
        if (level.inBounds(pos.x, pos.y + 1)) Destroy(level.Objects[pos.x, pos.y + 1], 0.6f);
        //Reassign objects center to outside
        for (int j = pos.y + 1; j < level.squaresY - 1; j++)
        {
            level.Objects[pos.x, j] = level.Objects[pos.x, j + 1];
        }
        //Shift in a rock
        if (pos.y != level.squaresY - 1 && level.inBounds(pos.x, level.squaresY - 1)) SpawnRock(pos.x, level.squaresY - 1, true);

    }
}
