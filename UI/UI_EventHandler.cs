using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// PointerEventData 정리 코드
public class UI_EventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Action<PointerEventData> OnEnterHandler = null;
    public Action<PointerEventData> OnExitHandler = null;
    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnBeginDragHandler = null;
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnEndDragHandler = null;
    public Action<PointerEventData> OnDropHandler = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnEnterHandler.IsNull() == false)
            OnEnterHandler.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnExitHandler.IsNull() == false)
            OnExitHandler.Invoke(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler.IsNull() == false)
            OnClickHandler.Invoke(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (OnBeginDragHandler.IsNull() == false)
            OnBeginDragHandler.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragHandler.IsNull() == false)
            OnDragHandler.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (OnEndDragHandler.IsNull() == false)
            OnEndDragHandler.Invoke(eventData);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (OnDropHandler.IsNull() == false)
            OnDropHandler.Invoke(eventData);
    }
}
