using UnityEngine;
using System.Collections;

public class UILoadingCancelButton : MonoBehaviour
{
    MultiplayerController controller;

    public void CancelLoad()
    {
        if (controller != null)
        {
            controller.CancelLoad();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (controller == null)
        {
            controller = FindObjectOfType<MultiplayerController>();
        }
    }
}
