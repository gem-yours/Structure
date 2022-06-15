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
    public GameObject? draggingIcon;
    public delegate void OnDragging(Vector2 displacement);
    public OnDragging? onDragging { get; set; }
    public delegate void OnEndDraggging();
    public OnEndDraggging? onEndDragging { get; set; }
    public delegate void OnPushed();
    public OnPushed? onPushed = null;
    public delegate void OnClick();
    public OnClick? onClick { get; set; }

    private RectTransform? draggingPlane;
    private Vector2 startPoint;

    public void OnBeginDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);

        startPoint = eventData.position;
        if (onDragging != null)
        {
            onDragging((eventData.position - startPoint));
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingIcon is not null)
            SetDraggedPosition(eventData);

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
            var localPosition = draggingIcon.transform.localPosition;
            var threshold = 50;
            if (localPosition.magnitude > threshold)
            {
                draggingIcon.transform.localPosition = localPosition.normalized * threshold;
            }
        }
    }

    public void OnEndDrag(PointerEventData? eventData)
    {
        if (draggingIcon != null)
        {
            StartCoroutine(ReturnDraggingIconToOrigin());
        }
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

    private void Start()
    {
        draggingPlane = transform as RectTransform;
    }

    private IEnumerator ReturnDraggingIconToOrigin()
    {
        if (draggingIcon == null) yield break;
        var positionOfStart = draggingIcon.transform.localPosition;
        var animationDuration = 0.1f;
        yield return AnimationUtil.EaseInOut(animationDuration, (float current) =>
        {
            draggingIcon.transform.localPosition = positionOfStart * current;
        });
        draggingIcon.transform.localPosition = Vector3.zero;
    }
}
