using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SelectProvince : MonoBehaviour {


    public GameLogic gameLogic;
	// Use this for initialization
	void Start () {
		
	}
    Color lastColor = new Color(0,0,0);
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0) && !gameLogic.paused)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Province")
                {
                    int id = int.Parse(hit.transform.name) - 1;
                    if(lastColor !=new Color(0,0,0)) gameLogic.provinces[gameLogic.currentProvince].GetComponent<Renderer>().material.color = lastColor;
                    lastColor = gameLogic.provinces[id].GetComponent<Renderer>().material.color;
                    gameLogic.provinces[id].GetComponent<Renderer>().material.color = Color.red;
                    
                    gameLogic.currentProvince = id;
                }
            }
        }
	}
}
