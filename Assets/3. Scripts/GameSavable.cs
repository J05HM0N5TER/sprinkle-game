using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSavable : MonoBehaviour
{
	public virtual string Save()
	{
		return "This is not implimented";
	}
	public virtual void Load(string info) { }
}
