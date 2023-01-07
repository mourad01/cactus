using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoapBubble : MonoBehaviour
{
    [Header("GameObject")]
    public GameObject FX_Shower;
    public GameObject FX_SplashBubble;

    [Header("Bool")]
    private bool Status;

    [Header("MeshRenderer")]
    private MeshRenderer Mesh;

    private void Start()
    {
        Mesh = GetComponent<MeshRenderer>();
    }
    public void ManageSoap(bool Value, BathRoom Bath)
    {
        switch (Value)
        {
            case (true):

                if (!Status)
                {
                    Bath.CountSoapBubbles++;
                    gameObject.layer = 6;
                    Instantiate(FX_SplashBubble, transform.position, Quaternion.identity);
                    Status = true;
                }
                break;

            case (false):

                if (Status)
                {
                    Bath.CountSoapBubbles--;
                    gameObject.layer = 12;
                    Instantiate(FX_Shower, transform.position, Quaternion.identity);
                    Status = false;
                }
                break;
        }
        Mesh.enabled = Value;
    }
}
