using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomComponent : MonoBehaviour {

	public string customString = "default";
	public int customInt = 0;

	[SerializeField]
	private int privateInt = 5;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
