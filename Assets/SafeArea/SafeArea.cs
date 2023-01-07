using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeArea : MonoBehaviour
{
    [Header("RectTransform")]
    private RectTransform RectTransform;

    [Header("Rect")]
    private Rect Rect;

    [Header("Vector")]
    private Vector2 MinAnchor;
    private Vector2 MaxAnchor;

    // Start is called before the first frame update
    void Awake()
    {
        SetSafeArea();
    }

    private void SetSafeArea()
    {
        RectTransform = GetComponent<RectTransform>();
        Rect = Screen.safeArea;
        MinAnchor = Rect.position;
        MaxAnchor = MinAnchor + Rect.size;

        MinAnchor.x /= Screen.width;
        MinAnchor.y /= Screen.height;
        MaxAnchor.x /= Screen.width;
        MaxAnchor.y /= Screen.height;

        RectTransform.anchorMin = MinAnchor;
        RectTransform.anchorMax = MaxAnchor;
    }
}
