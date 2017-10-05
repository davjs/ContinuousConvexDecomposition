using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Polygon : DestructablePolygon
{
    public override void Init()
    {
        Destroy(this.gameObject);
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
