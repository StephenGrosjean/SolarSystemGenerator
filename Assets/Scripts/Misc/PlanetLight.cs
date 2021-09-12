using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetLight : MonoBehaviour
{
    private GameObject star;
    private Light light;
    public float lightDistance;
    private void Start() {
        star = GameObject.Find("Star");
        lightDistance = transform.localScale.x + 20;
        GameObject lightObj = new GameObject();
        light = lightObj.AddComponent<Light>();
        light.range = lightDistance;
        light.intensity = 2000;
    }

    private void Update() {
        float d = Vector3.Distance(star.transform.position, transform.position);
        light.transform.position = Vector3.Lerp(star.transform.position, transform.position, (d-lightDistance)/d);
    }
}
