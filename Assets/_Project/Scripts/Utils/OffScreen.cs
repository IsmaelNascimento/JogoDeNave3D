using System;
using UnityEngine;

public class OffScreen : MonoBehaviour
{
    private static OffScreen instance;
    public static OffScreen Instance
    {
        get
        {
            if(instance == null)
            {
                var offScreenManager = new GameObject("OffScreenManager", typeof(OffScreen));
                instance = FindObjectOfType(typeof(OffScreen)) as OffScreen;
            }

            return instance;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="offsetWidth">Distancia depois que saiu da tela</param>
    public void DestroyGameObjetWidth(GameObject thisObject, float offsetWidth = 10f, Action OnDestroy = null)
    {
        var newPosition = Camera.main.WorldToScreenPoint(thisObject.transform.position);
        var limitMinWidthOffScreen = (Screen.width / Screen.width) - offsetWidth;
        var limitMaxWidthOffScreen = (Screen.width) + offsetWidth;

        if (newPosition.x < limitMinWidthOffScreen || newPosition.x > limitMaxWidthOffScreen)
        {
            Destroy(thisObject);

            if (OnDestroy != null)
                OnDestroy();
        }
    }

    public void DestroyGameObjetWidthAndHeigth(GameObject thisObject, float offsetWidth = 10f, float offsetHeigthUp = 10f, Action OnDestroy = null)
    {
        var newPosition = Camera.main.WorldToScreenPoint(thisObject.transform.position);
        var limitMinWidthOffScreen = (Screen.width / Screen.width) - offsetWidth;
        var limitMaxHeigthOffScreen = (Screen.height + offsetHeigthUp);

        if (newPosition.x < limitMinWidthOffScreen || newPosition.y > limitMaxHeigthOffScreen)
        {
            Destroy(thisObject);

            if (OnDestroy != null)
                OnDestroy();
        }
    }
}