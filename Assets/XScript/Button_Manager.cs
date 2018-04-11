using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Button_Manager : MonoBehaviour, IPointerDownHandler,IPointerUpHandler{

    public static bool _press = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        _press = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        _press = false;
    }

}
