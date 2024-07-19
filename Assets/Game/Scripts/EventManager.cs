using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static event Action<Feature> ClickUserOnFeatureEvent;

    public static void OnClickUser(Feature feature)
    {
        ClickUserOnFeatureEvent?.Invoke(feature);
    }
}
