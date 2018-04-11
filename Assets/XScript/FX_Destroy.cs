using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_Destroy : MonoBehaviour {

    public float lifetime = 1;
	void Start () {
        Destroy(gameObject, lifetime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
