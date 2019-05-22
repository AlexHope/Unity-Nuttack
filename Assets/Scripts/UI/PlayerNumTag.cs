using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerNumTag : MonoBehaviour
{

    public Transform playerTransform;
    static private Color[] tagColors = new Color[5] { Color.red, Color.green, Color.blue, Color.yellow, new Color(0.7f, 0.7f, 0.7f) };
    Canvas ui;

    // Use this for initialization
    void Start()
    {
        ui = transform.parent.GetComponent<Canvas>();
    }

    public void SetupTag(int num)
    {
        Color color = tagColors[4];

        if (num >= 0)
        {
            GetComponent<Text>().text = "P" + (num + 1);
            color = tagColors[num];
        }
        else
        {
            GetComponent<Text>().text = "CPU";
        }

        GetComponent<Text>().color = color;
        transform.GetChild(0).GetComponent<Text>().text = ">";
        transform.GetChild(0).GetComponent<Text>().color = color;
    }

    void UpdatePosition()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(playerTransform.position);
        pos.y += ui.pixelRect.height * 0.11f + 80 - Camera.main.orthographicSize * 8;

        transform.GetComponent<RectTransform>().position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        UpdatePosition();
    }
}
