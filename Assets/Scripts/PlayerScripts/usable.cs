using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class usableItem: MonoBehaviour
{
	public virtual void use() {
		Debug.Log("HOW DID WE GET HERE");
	}
}