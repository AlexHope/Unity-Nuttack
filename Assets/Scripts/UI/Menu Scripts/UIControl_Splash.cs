using UnityEngine;
using System.Collections;

public class UIControl_Splash : MonoBehaviour
{

    float timer;

    // Use this for initialization
    void Start()
    {
        timer = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            Application.LoadLevel("launchscreen");
        }
    }
}
