using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Manipulates the explosion children of this object.
/// Destroys itself upon completion.
/// </summary>
public class ExplosionScript : MonoBehaviour {

	public float animationLength;
    private int numChildren;                 //For efficiency
    public Transform[] childrenTransforms;
    public float[] alphaModifiers;          //Added to the alpha value of the corresponding child
    public Color[] childrenColors;
    private float[] origScaling;            //Original scaling of each child (assuming uniform scale)
    private Quaternion[] origRotation;
    private Quaternion[] targetRotation;    //Random rotation target for each child
    private MeshRenderer[] renderers;       //Mesh renderers of children
    public AnimationCurve sizeCurve;
    public AnimationCurve opacityCurve;
    public AnimationCurve rotationCurve;

    float creationTime; //Time at which this object was created.
                        //Used to evaluate anim curves

	// Use this for initialization
	void Start () {
        creationTime = Time.time;

        numChildren = childrenTransforms.Length;

        targetRotation = new Quaternion[numChildren];
        origRotation = new Quaternion[numChildren];
        origScaling = new float[numChildren];
        renderers = new MeshRenderer[numChildren];
        for (int i = 0; i < numChildren; i++)
        {
            //Obtain a random target rotation for each child
            targetRotation[i] = Random.rotation;
            origRotation[i] = Random.rotation;
            //Set original scaling constants
            origScaling[i] = childrenTransforms[i].localScale.x;
            //Make clones of materials 
            renderers[i] = childrenTransforms[i].GetComponent<MeshRenderer>();
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float localTime = Time.time - creationTime;

        if (localTime > animationLength)
        {
            Destroy(gameObject);
        }

        float sizeMod = sizeCurve.Evaluate(localTime);
        float opacityMod = opacityCurve.Evaluate(localTime);

        for (int i = 0; i < numChildren; i++)
        {
            //Lerp children towards their rotation
            childrenTransforms[i].rotation = Quaternion.Lerp(
                origRotation[i],
                targetRotation[i],
                rotationCurve.Evaluate(localTime)
            );

                //Set new size
            float newScale = origScaling[i] * sizeMod;
            childrenTransforms[i].localScale = new Vector3(newScale,newScale,newScale);

            Color newCol = childrenColors[i];
            newCol.a = Mathf.Clamp01(alphaModifiers[i] + opacityMod);

                //Set new opacity
            renderers[i].material.color = newCol;
        }
    }
}
