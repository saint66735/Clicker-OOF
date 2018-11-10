using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{

    public GameLogic gameLogic;
    float speed = 0.2f;
    public GameObject planet;
    float initialDistance;
    // Use this for initialization
    void Start()
    {
        initialDistance = Vector3.Distance(transform.position, planet.transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameLogic.paused)
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            //transform.RotateAround(planet.transform.position, Vector3.up, x * speed);
            //transform.RotateAround(planet.transform.position, Vector3.right, y * speed);
            float convertAnglex = 0;
            if (transform.rotation.eulerAngles.x <= 180) convertAnglex = transform.rotation.eulerAngles.x;
            if (transform.rotation.eulerAngles.x > 180) convertAnglex = -(360 - transform.rotation.eulerAngles.x);
            //print(convertAnglex);
            if (y > 0 && convertAnglex > 70) y = 0;
            if (y < 0 && convertAnglex < -70) y = 0;

            transform.Translate(new Vector3(x * speed, y * speed, 0));
            //transform.rotation = Quaternion.Euler(0, 0, 0);
            transform.LookAt(planet.transform.position);
            if (Vector3.Distance(transform.position, planet.transform.position) > initialDistance)
                transform.Translate(Vector3.forward * 0.04f);
        }
    }
}
