using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

public class HealthBarController : MonoBehaviour
{
    public PlayerController player;
	public Texture2D userImage;

    //private Text scoreText;
    //private Text acornsText;
    private RectTransform healthTransform;
    private float healthY;
    private float hBarWidth;
    private float hBarHeight;
    private float hMaxX;
    private float hMinX;
    private RectTransform shieldTransform;
    private float shieldY;
    private float sBarWidth;
    private float sBarHeight;
    private float sMaxX;
    private float sMinX;

    // Use this for initialization
    void Start()
    {

    }

    public void InitHealth(GameObject h, int pNum)
    {
        h.transform.Find("Text/PlayerNum").GetComponent<Text>().text = "Player " + (pNum + 1);
        //scoreText = h.transform.Find("Text/Score").GetComponent<Text>();
        //acornsText = h.transform.Find("Text/Acorns").GetComponent<Text>();

        Color[] playerColors = new Color[4] { Color.red, Color.green, Color.blue, Color.yellow };
        //scoreText.color = playerColors[pNum];
        //acornsText.color = playerColors[pNum];
        h.transform.Find("Text/PlayerNum").GetComponent<Text>().color = playerColors[pNum];

        healthTransform = h.transform.Find("HealthBack/HealthFill").GetComponent<RectTransform>();
        hBarWidth = healthTransform.rect.width;
        hBarHeight = healthTransform.rect.height;
        healthY = healthTransform.rect.y;
        hMaxX = healthTransform.rect.position.x - (hBarWidth / 2); //bars were somehow offset by half the width
        hMinX = hMaxX - hBarWidth;

        shieldTransform = h.transform.Find("ShieldBack/ShieldFill").GetComponent<RectTransform>();
        sBarWidth = shieldTransform.rect.width;
        sBarHeight = shieldTransform.rect.height;
        shieldY = shieldTransform.rect.y;
		sMaxX = shieldTransform.rect.position.x - (sBarWidth / 2); //bars were somehow offset by half the width
        sMinX = sMaxX - sBarWidth;

		//add profile pic
		if (Social.localUser.authenticated && userImage != null)
		{
			//convert Texture2D to Sprite
			Rect spriteRect = new Rect(0,0,userImage.width,userImage.height);
			Vector2 spritePivot = new Vector2(0.5f,0.5f);
			h.transform.Find("ProPic").GetComponent<Image>().overrideSprite = Sprite.Create(userImage,spriteRect,spritePivot);
		}
    }

    private void HandleHealth()
    {
        float newX = MapValues(player.Health, 0, player.MaxHealth, hMinX, hMaxX);
        healthTransform.offsetMin = new Vector2(newX, healthY);
        float posX = healthTransform.offsetMin.x + hBarWidth;
        float posY = healthTransform.offsetMin.y + hBarHeight;
        healthTransform.offsetMax = new Vector2(posX, posY);
    }

    private void HandleShield()
    {
        float newX = MapValues(player.BlockingCounter, 0, player.BlockingAmount, sMinX, sMaxX);
        shieldTransform.offsetMin = new Vector2(newX, shieldY);
        float posX = shieldTransform.offsetMin.x + sBarWidth;
        float posY = shieldTransform.offsetMin.y + sBarHeight;
        shieldTransform.offsetMax = new Vector2(posX, posY);
    }
	/*
    private void HandleScores()
    {
        acornsText.text = "" + player.Acorns;
        scoreText.text = "" + player.Score;
     }
	*/
    private float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
    {
        float outMaxMin = outMax - outMin;
        float inMaxMin = inMax - inMin;
        float result = ((x - inMin) * (outMaxMin / (inMaxMin))) + outMin;
        return result;
    }

    // Update is called once per frame
    void Update()
    {
        HandleHealth();
        HandleShield();
        //HandleScores();
    }

}
