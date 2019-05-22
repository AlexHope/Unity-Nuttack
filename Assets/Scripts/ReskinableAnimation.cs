using UnityEngine;
using System;
using System.Collections;

public class ReskinableAnimation : MonoBehaviour {

	public void ChangeBaseSprite(string sprite)
	{
		Sprite[] spriteSheet = Resources.LoadAll<Sprite>("Characters/" + sprite);

		foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
		{
			string spriteName = renderer.sprite.name;

			Sprite newSprite = Array.Find(spriteSheet, item => item.name == spriteName);

			if (newSprite) renderer.sprite = newSprite;
		}
	}
}
