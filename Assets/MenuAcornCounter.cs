using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuAcornCounter : MonoBehaviour
{
    SaveDataController saveData;

    // Use this for initialization
    void Start()
    {
        saveData = GameObject.FindObjectOfType<SaveDataController>();
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Text>().text = saveData.Acorns.ToString();
    }
}
