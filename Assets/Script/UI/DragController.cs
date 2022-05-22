using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#nullable enable
[RequireComponent(typeof(Image))]
public class DragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerUpHandler, IPointerDownHandler
{
    public bool dragOnSurfaces = true;

    public Sprite? draggingImage;
    public delegate void OnDragging(Vector2 displacement);
    public OnDragging? onDragging { get; set; }
    public delegate void OnEndDraggging();
    public OnEndDraggging? onEndDragging { get; set; }
    public delegate void OnPushed();
    public OnPushed? onPushed = null;
    public delegate void OnClick();
    public OnClick? onClick { get; set; }

    private GameObject? draggingIcon;
    private RectTransform? draggingPlane;
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
        if (draggingPlane == null || draggingIcon == null) return;
        var rt = draggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = draggingPlane.rotation;
        }
    }

    public void OnEndDrag(PointerEventData? eventData)
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
            onClick();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (onPushed is not null) onPushed();
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        if (onEndDragging is not null) onEndDragging();
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
        if (image != null)
        {
            image.sprite = draggingImage;
            image.enabled = draggingImage != null;
            image.SetNativeSize();
        }

        if (dragOnSurfaces)
            draggingPlane = transform as RectTransform;
        else
            draggingPlane = canvas.transform as RectTransform;
    }
}
