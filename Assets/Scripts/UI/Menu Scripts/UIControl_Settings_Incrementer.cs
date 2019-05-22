using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIControl_Settings_Incrementer : MonoBehaviour {
	
	private int index;
	public string[] values;
	
	void Start()
	{
		index = 0;
		transform.FindChild ("Value").GetComponent<Text> ().text = values [index];
	}
	
	public void IncreaseValue()
	{
		if (index != values.Length - 1) {
			index++;
		} else {
			index = 0;
		}
		transform.FindChild ("Value").GetComponent<Text> ().text = values [index];
	}
	
	public void DecreaseValue()
	{
		if (index > 0) {
			index--;
		} else {
			index = values.Length - 1;
		}
		transform.FindChild ("Value").GetComponent<Text> ().text = values [index];
	}

	public int Index()
	{
		return index;
	}

	public void SetIndex(int _index)
	{
		transform.FindChild ("Value").GetComponent<Text> ().text = values [_index];
		index = _index;
	}
}
