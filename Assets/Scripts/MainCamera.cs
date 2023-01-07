using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [Header("Transform")]
    private Transform Target;
    private Transform MyTransform;

    [Header("Float")]
    public float Speed;

    [Header("Vector")]
    public Vector3 OffSet;

    [Header("Bool")]
    private bool FollowTarget;

    private void Start()
    {
        MyTransform = transform;
    }
    private void LateUpdate()
    {
        if (FollowTarget)
        {
            MyTransform.position = Vector3.MoveTowards(MyTransform.position, Target.position + OffSet, Speed * Time.deltaTime);
        }
    }
    public void SetTarget(Transform Target)
    {
        this.Target = Target;
        FollowTarget = true;
    }
    public void Unfollow()
    {
        FollowTarget = false;
    }
}
