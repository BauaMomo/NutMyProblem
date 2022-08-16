using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjectController : MonoBehaviour
{
    GameObject BackgroundObject;
    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.childCount > 1) Debug.LogWarning("More than one child object in background empty " + gameObject.name + ". Only one allowed");
        if (transform.childCount == 0) Debug.LogWarning("No child background object detected. Please add an object to " + gameObject.name);

        BackgroundObject = transform.GetChild(0).gameObject;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > 1 || transform.childCount == 0) return;

        Vector2 relCamPos = (Vector2)cam.transform.position - (Vector2)transform.position;
        Vector2 newPos =  new Vector2(0.5f * relCamPos.x,  0.1f * relCamPos.y);

        BackgroundObject.transform.localPosition = newPos;
    }
}
