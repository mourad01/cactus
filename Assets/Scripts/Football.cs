using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Football : MonoBehaviour
{
    [Header("Vector")]
    private Vector3 OriginalPosition;

    [Header("Bool")]
    private bool Drag;

    [Header("LayerMask")]
    public LayerMask Football_Mask;

    [Header("Raycast")]
    private Ray Ray;

    private void Awake()
    {
        OriginalPosition = transform.position;
    }
    private void OnEnable()
    {
        transform.position = OriginalPosition;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!Drag)
            {
                Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(Ray, 50, Football_Mask))
                {
                    transform.DOJump(transform.position, 2, 5, 4).OnComplete(() => { Drag = false; });
                    Drag = true;
                }
            }
        }
    }
}
