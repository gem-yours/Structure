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

    private GameObject draggingIcon;
    private RectTransform draggingPlane;
    private Vector2 startPoint;

    private List<IObserver<Vector2>> observers = new List<IObserver<Vector2>>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        if (canvas == null)
            return;

        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.
        draggingIcon = new GameObject("icon");

        draggingIcon.transform.SetParent(canvas.transform, false);
        draggingIcon.transform.SetAsLastSibling();

        var image = draggingIcon.AddComponent<Image>();

        image.sprite = GetComponent<Image>().sprite;
        image.SetNativeSize();

        if (dragOnSurfaces)
            draggingPlane = transform as RectTransform;
        else
            draggingPlane = canvas.transform as RectTransform;

        SetDraggedPosition(eventData);

        startPoint = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingIcon != null)
            SetDraggedPosition(eventData);

        GameManager.instance.onDrag((eventData.position - startPoint).normalized);
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
    }
}
