using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetWaterLayer : MonoBehaviour
{
    [SerializeField] private Transform waterSphere;
    public void SetWaterLayerValue(float height, float temperature) {
        waterSphere.transform.localScale = Vector3.one * height;
        waterSphere.GetComponent<Renderer>().sharedMaterial = new Material(waterSphere.GetComponent<Renderer>().material);
        waterSphere.GetComponent<Renderer>().sharedMaterial.SetFloat("PlanetTemperature", temperature);
    }
}
