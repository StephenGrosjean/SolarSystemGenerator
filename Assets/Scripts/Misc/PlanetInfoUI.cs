using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlanetInfoUI : MonoBehaviour
{
    public class PlanetUI
    {
        public Image button;
        public string info;
        public Transform planetObj;
        public SolarSystemGenerator.Planet planet;

        public PlanetUI(SolarSystemGenerator.Planet planet) {
            this.planetObj = planet.obj.transform;
            this.planet = planet;
        }
    }
    public PlanetUI planetUIInfo;

    private bool state = false;
    public void SetInfo(SolarSystemGenerator.Planet planet, StellarBodyData.StarData starData) {
        planetUIInfo = new PlanetUI(planet);
        planetUIInfo.planetObj = planet.obj.transform;
        planetUIInfo.button = GetComponentInChildren<Image>();
        if(planet.data.planetClass != StellarBodyData.PlanetClass.Moon) {
            planetUIInfo.info =
            "Diameter : " + (planet.size * 1000).ToString("F1") + " km" + "\n"
            + "Mean temperature : " + planet.temperature.ToString("F1") + " K" + "\n"
            + "Mean temperature : " + (planet.temperature - 273.15f).ToString("F1") + " C" + "\n"
            + "Escape Velocity : " + planet.escapeVelocity.ToString("F1") + " km/s" + "\n"
            + "Type : " + (planet.isGasPlanet ? "Gas" : "Terrestrial") + "\n"
            + "Earth like score : " + Universe.EarthLikeScore(planet, starData).ToString("F1");
        }
        else {
            planetUIInfo.info =
            "Diameter : " + (planet.size * 1000).ToString("F1") + " km" + "\n"
            + "Mean temperature : " + planet.temperature.ToString("F1") + " K" + "\n"
            + "Mean temperature : " + (planet.temperature - 273.15f).ToString("F1") + " C" + "\n"
            + "Type : " + "Terrestrial Moon";
        }
        
        planet.obj.GetComponent<CelestialObject>().planetInfoUI = this;
    }

    public void ShowInfo() {
        PlanetUILocator.instance.ShowInfo(planetUIInfo);
    }
}
