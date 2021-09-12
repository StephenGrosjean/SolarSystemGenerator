using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDisable : MonoBehaviour
{
    public struct Planet
    {
        public MeshRenderer[] meshRenderers;
        public Collider meshCollider;
    }

    public static MeshDisable instance;

    private List<Planet> planets = new List<Planet>();
    Plane[] planes;
    Camera cam;

    private void Awake() {
        instance = this;
    }

    void Start()
    {
        cam = Camera.main;
    }

    private void FixedUpdate() {
        planes = GeometryUtility.CalculateFrustumPlanes(cam);

        foreach (Planet p in planets) {
            if (GeometryUtility.TestPlanesAABB(planes, p.meshCollider.bounds)) {
                ToggleRenderers(p, true);
                PlanetUILocator.instance.SetInView(p.meshCollider.gameObject, true);
            }
            else {
                ToggleRenderers(p, false);
                PlanetUILocator.instance.SetInView(p.meshCollider.gameObject, false);
            }

            if (PlanetUILocator.instance.IsFarFromPlanet(p.meshCollider.gameObject)) {
                ToggleRenderers(p, false);
            }
            else {
                ToggleRenderers(p, true);
            }
        }       
    }

    public void AddCollider(Collider collider) {
        Planet p = new Planet();
        p.meshCollider = collider;
        p.meshRenderers = p.meshCollider.gameObject.GetComponentsInChildren<MeshRenderer>();
        planets.Add(p);
    }

    void ToggleRenderers(Planet p, bool state) {
        foreach (MeshRenderer m in p.meshRenderers) {
            m.enabled = state;
        }
    }
}
