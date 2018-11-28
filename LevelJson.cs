/**
 * The JSON file format
 * */
[System.Serializable]
public class LevelJson {
    public int id;
    public int height;
    public int width;
    public int B1x3;
    public int B1x5;
    public int B1xINF;
    public int B3x1;
    public int B5x1;
    public int BINFx1;
    public int B3x3Square;
    public int B5x5Square;
    public int B3x3Cross;
    public int B5x5Cross;
    public int BINFxINFCross;
    public int BLeftFirework;
    public int BRightFirework;
    public int BUpFirework;
    public int BDownFirework;
    public int [] gameBoard;
}
