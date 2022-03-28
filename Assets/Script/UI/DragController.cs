using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool dragOnSurfaces = true;

    public delegate void OnDragging(Vector2 displacement);
    public OnDragging onDragging { get; set; }
    public delegate void OnEndDraggging();
    public OnEndDraggging onEndDragging { get; set; }

    private GameObject draggingIcon;
    private RectTransform draggingPlane;
    private Vector2 startPoint;

    public void OnBeginDrag(PointerEventData eventData)
    {
        CreateDraggingImage(eventData.position);

        SetDraggedPosition(eventData);

        startPoint = eventData.position;
        if (onDragging != null)
        {
            onDragging((eventData.position - startPoint));
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingIcon != null)
            SetDraggedPosition(eventData);
        else
            CreateDraggingImage(eventData.position);

        if (onDragging != null)
        {
            onDragging((eventData.position - startPoint));
        }
    }

    private void SetDraggedPosition(PointerEventData data)
    {
        var rt = draggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = draggingPlane.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingIcon != null)
            Destroy(draggingIcon);
        if (onEndDragging != null)
        {
            onEndDragging();
        }
    }

    public void ForceEndDrag()
    {
        OnEndDrag(null);
    }

    private void CreateDraggingImage(Vector3 position)
    {
        var canvas = GameObject.Find("UI").GetComponent<Canvas>();
        if (canvas == null)
            return;
        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.
        draggingIcon = new GameObject("icon");

        draggingIcon.transform.SetParent(canvas.transform, false);
        draggingIcon.transform.SetAsLastSibling();
        draggingIcon.transform.position = position;

        var image = draggingIcon.AddComponent<Image>();

        image.sprite = GetComponent<Image>().sprite;
        image.SetNativeSize();

        if (dragOnSurfaces)
            draggingPlane = transform as RectTransform;
        else
            draggingPlane = canvas.transform as RectTransform;
    }
}
