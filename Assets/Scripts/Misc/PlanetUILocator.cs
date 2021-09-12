using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlanetUILocator : MonoBehaviour
{
    public static PlanetUILocator instance;

    public class Planet
    {
        public GameObject obj;
        public GameObject imageObject;
        public bool follow;
        public bool isInView;
    }

    [SerializeField] private GameObject defaultImage;
    [SerializeField] private List<Planet> planets = new List<Planet>();
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private GameObject followPlanetButton;
    private Transform currentPlanetTransform;
    private Vector2 uiScaleRange = new Vector2( 10, 800);
    private Vector2 uiDistanceRange = new Vector2(800, 30000);
    private Vector2 lineScaleRange = new Vector2(.1f, 30);
    private Vector2 lineDistanceRange = new Vector2(100, 30000);
    private Image currentActiveButton;
    private void Awake() {
        instance = this;
    }

    private void Update() {
        DistanceToPlanet();
        foreach (Planet p in planets) {
            if (p.follow) {
                p.imageObject.transform.LookAt(p.imageObject.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
                p.imageObject.transform.position = p.obj.transform.position;

                p.imageObject.transform.localScale = Vector2.one * ScaleUIFromDistance(p);
            }
            p.obj.GetComponent<LineRenderer>().widthMultiplier = ScaleLineFromDistance(p);
        }
    }
    public void AddImageToPlanet(SolarSystemGenerator.Planet planet, StellarBodyData.StarData starData) {
        Planet p = new Planet();
        p.obj = planet.obj;
        p.imageObject = Instantiate(defaultImage, transform);
        p.follow = true;
        p.imageObject.GetComponent<PlanetInfoUI>().SetInfo(planet, starData);
        p.isInView = true;
        planets.Add(p);
    }

    public void TogglePlanetIndicator(GameObject obj, bool state) {
        Planet p = FindPlanet(obj);
        p.imageObject.SetActive(state);
        p.follow = state;
    }

    Planet FindPlanet(GameObject obj) {
        foreach(Planet p in planets) {
            if(p.obj == obj) {
                return p;
            }
        }

        return new Planet();
    }

    void DistanceToPlanet() {
        foreach (Planet p in planets) {
            if (Vector3.Distance(Camera.main.transform.position, p.obj.transform.position) > uiDistanceRange.x && p.isInView) {
                TogglePlanetIndicator(p.obj, true);
            }
            else {
                TogglePlanetIndicator(p.obj, false);
            }
        }
    }

    float ScaleUIFromDistance(Planet p) {
        float dist = Vector3.Distance(Camera.main.transform.position, p.obj.transform.position);
        if (dist > uiDistanceRange.x) {
            if(dist <= uiDistanceRange.y) {
                float slope = (uiScaleRange.y - uiScaleRange.x) / (uiDistanceRange.y - uiDistanceRange.x);
                float b = uiScaleRange.x - (slope * uiDistanceRange.x);
                return (slope * dist) + b;
            }
            else {
                return uiScaleRange.y;
            }
        }
            
        else {
            return 1;
        }
    }

    float ScaleLineFromDistance(Planet p) {
        float dist = Vector3.Distance(Camera.main.transform.position, p.obj.transform.position);
        if (dist > lineDistanceRange.x) {
            if (dist <= lineDistanceRange.y) {
                float slope = (lineScaleRange.y - lineScaleRange.x) / (lineDistanceRange.y - lineDistanceRange.x);
                float b = lineScaleRange.x - (slope * lineDistanceRange.x);
                return (slope * dist) + b;
            }
            else {
                return lineScaleRange.y;
            }
        }

        else {
            return lineScaleRange.x;
        }
    }

    public bool IsFarFromPlanet(GameObject p) {
        return Vector3.Distance(Camera.main.transform.position, p.transform.position) > uiDistanceRange.x+p.transform.localScale.x;
    }

    public void SetInView(GameObject planet, bool state) {
        Planet p = FindPlanet(planet);
        p.isInView = state;
    }

    public void ShowInfo(PlanetInfoUI.PlanetUI planetUIInfo) {
        if (currentActiveButton) {
            SetButtonColor(currentActiveButton, Color.white);
        }

        infoText.text = planetUIInfo.info;
        SetButtonColor(planetUIInfo.button, Color.red);
        currentActiveButton = planetUIInfo.button;
        currentPlanetTransform = planetUIInfo.planetObj;
        followPlanetButton.SetActive(true);
    }

    public void SetButtonColor(Image button, Color color) {
        button.color = color;
    }

    public void FollowPlanet() {
        Camera.main.GetComponent<CameraController>().FollowPlanet(currentPlanetTransform);
    }

    public void HideInfo() {
        infoText.text = "";
        if(currentActiveButton)
            SetButtonColor(currentActiveButton, Color.white);
        currentActiveButton = null;
        currentPlanetTransform = null;
        followPlanetButton.SetActive(false);
    }
}
