using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


    public enum ColorFeature
    {
        Blue,
        Green,
        Orange,
        Purple,
        Red
    }

public class Feature : MonoBehaviour
{
    public ColorFeature colorFeature;
    public void DestroyFeatureOnClick()
    {
        EventManager.OnClickUser(this);
    }
}
