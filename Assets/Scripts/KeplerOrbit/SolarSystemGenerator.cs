using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class SolarSystemGenerator : MonoBehaviour
{

    //PUBLIC//
    [Serializable]
    public class SolarSystemData
    {
        public int seed;
        public int telluricPlanetCount;
        public int gasPlanetCount;
        public int superGasPlanetCount;
        public StellarBodyData.StarClass starClass;
        public bool onlyPossiblePlanets;
    }
    public enum PremadePlanet
    {
        Mercury,
        Venus,
        Earth,
        Mars,
        Jupiter,
        Saturn,
        Uranus,
        Neptune,
        Pluto
    }

    [Serializable]
    public class BodyReferences
    {
        public GameObject star;
        public GameObject planet;
        public GameObject moon;
    }

    [Serializable]
    public class Planet
    {
        public Planet(GameObject obj)
        {
            GameObject instantiatedObj = Instantiate(obj);
            this.obj = instantiatedObj;
        }

        public StellarBodyData.PlanetData data;
        public StellarBodyData.PlanetMat planetMat;
        public float size;
        public float heatGradient;
        public GameObject obj;
        public bool isGasPlanet;
        public float temperature;
        public float distance;
        public float mass;
        public float density;
        public float escapeVelocity;
        public float earthLikeScore;
    }

    [Serializable]
    public class PremadePlanets
    {
        public PremadePlanet name;
        public StellarBodyData.PlanetData data;
    }
    
    [SerializeField] private List<PremadePlanets> premadePlanets = new List<PremadePlanets>();
    [SerializeField] private SolarSystemData solarSystemData = new SolarSystemData();
    [SerializeField] private BodyReferences bodyReferences;
    [SerializeField] private bool isSeedRandom;

    [SerializeField] private PremadePlanet planetToForce;
    [SerializeField] private bool forcePlanet;


    //PRIVATE//

    private List<Planet> planets = new List<Planet>();
    private List<Planet> moons = new List<Planet>();
    private List<Vector2> telluricDistanceRanges = new List<Vector2>();
    private List<Vector2> gasDistanceRanges = new List<Vector2>();
    private List<Vector2> superGasDistanceRanges = new List<Vector2>();
    private List<Vector2> moonDistanceRanges = new List<Vector2>();
    private FastTrig fastTrig;

    /// <summary>
    /// Set the data for the solar system generation
    /// </summary>

    public void SetSystemData(int telluricPlanetCount, int gasPlanetCount, int superGasPlanetCount, StellarBodyData.StarClass starClass, bool onlyPossiblePlanets, int seed) {
        this.solarSystemData.telluricPlanetCount = telluricPlanetCount;
        this.solarSystemData.gasPlanetCount = gasPlanetCount;
        this.solarSystemData.superGasPlanetCount = superGasPlanetCount;
        this.solarSystemData.starClass = starClass;
        this.solarSystemData.onlyPossiblePlanets = onlyPossiblePlanets;
        this.solarSystemData.seed = seed;
        SeedInit();
    }

    /// <summary>
    /// Return the data of the current solar system configuration
    /// </summary>
    public SolarSystemData GetData() {
        return solarSystemData;
    }

    /// <summary>
    /// Init the seed for the random generation
    /// </summary>
    private void SeedInit() {
        isSeedRandom = solarSystemData.seed == -1;

        if (isSeedRandom) {
            solarSystemData.seed = Random.Range(0, int.MaxValue);
        }

        Random.InitState(solarSystemData.seed);
    }
    /// <summary>
    /// Generate each planet and moons, randomize them
    /// </summary>

    private void Start()
    {    
        
        GenerateStar();

        if (!forcePlanet) {
            SetDistanceRanges(StellarBodyData.instance.GetData(StellarBodyData.PlanetClass.Telluric), solarSystemData.telluricPlanetCount, telluricDistanceRanges);
            SetDistanceRanges(StellarBodyData.instance.GetData(StellarBodyData.PlanetClass.Gas), solarSystemData.gasPlanetCount, gasDistanceRanges);
            SetDistanceRanges(StellarBodyData.instance.GetData(StellarBodyData.PlanetClass.SuperGas), solarSystemData.superGasPlanetCount, superGasDistanceRanges);

            for (int i = 0; i < solarSystemData.telluricPlanetCount; i++) {
                GeneratePlanet(StellarBodyData.PlanetClass.Telluric, i);
            }

            for (int i = 0; i < solarSystemData.gasPlanetCount; i++) {
                GeneratePlanet(StellarBodyData.PlanetClass.Gas, i);
            }

            for (int i = 0; i < solarSystemData.superGasPlanetCount; i++) {
                GeneratePlanet(StellarBodyData.PlanetClass.SuperGas, i);
            }

            foreach(Planet p in planets) {
                int moonCount = Random.Range(p.data.moonsCountRange.x, p.data.moonsCountRange.y + 1);
                SetDistanceRanges(StellarBodyData.instance.GetData(StellarBodyData.PlanetClass.Moon), moonCount, moonDistanceRanges);
                for (int i = 0; i < moonCount; i++) {
                    GenerateMoons(p, i);
                }
            }
            RandomizePlanets();
        }
        else {
            GenerateCustomPlanet();
        }
    }


    /// <summary>
    /// Code for generating a custom planet for testing purposes
    /// </summary>
    private void GenerateCustomPlanet() {     
        StellarBodyData.PlanetData planetData = FindCustomPlanet(planetToForce);
        bool isGas = false;
        if (planetData.planetClass == StellarBodyData.PlanetClass.Gas || planetData.planetClass == StellarBodyData.PlanetClass.SuperGas) {
            isGas = true;
        }

        List<Vector2> distanceRanges = new List<Vector2>();
        switch (planetData.planetClass) {
            case StellarBodyData.PlanetClass.Telluric:
                distanceRanges = telluricDistanceRanges;
                break;
            case StellarBodyData.PlanetClass.Gas:
                distanceRanges = gasDistanceRanges;
                break;
            case StellarBodyData.PlanetClass.SuperGas:
                distanceRanges = superGasDistanceRanges;
                break;
        }


        Planet planet = new Planet(bodyReferences.planet);
            
        float mass = planetData.massRange.x;
        float distance = planetData.distanceRange.x;
        float eccentricity = planetData.eccentricityRange.x;
        float temperatureAtDistance = Universe.FindTemperatureAtDistance(StellarBodyData.instance.GetData(solarSystemData.starClass).luminosity, distance);
        float inclination = planetData.inclinationRange.x;
        float rotationSpeed = planetData.rotationSpeed.x;
        planet.obj.GetComponent<CelestialObject>().SetValues(
                                                        distance,
                                                        eccentricity,
                                                        bodyReferences.star.GetComponent<CelestialObjectReference>(),
                                                        temperatureAtDistance,
                                                        inclination,
                                                        rotationSpeed);

        planet.obj.GetComponent<CelestialObjectReference>().mass = mass;
        planet.size = planetData.sizeRange.x;
        planet.isGasPlanet = isGas;
        planet.heatGradient = GetPlanetHeatGradient(planet);
        planet.temperature = temperatureAtDistance;
        planet.planetMat = planetData.planetMaterials[Random.Range(0, planetData.planetMaterials.Count)];
        planet.data = planetData;
        planet.mass = mass;
        planet.distance = distance;
        planet.density = planetData.densityRange.x;
        planet.escapeVelocity = Universe.FindEscapeVelocity(planet);
        planet.earthLikeScore = Universe.EarthLikeScore(planet, StellarBodyData.instance.GetData(solarSystemData.starClass));
        planets.Add(planet);
            

        RandomizePlanets();
    }


    /// <summary>
    /// Generate planet
    /// </summary>
    private void GeneratePlanet(StellarBodyData.PlanetClass planetClass, int index) {
        bool isGas = false;
        if(planetClass == StellarBodyData.PlanetClass.Gas || planetClass == StellarBodyData.PlanetClass.SuperGas) {
            isGas = true;
        }

        List<Vector2> distanceRanges = new List<Vector2>();
        switch (planetClass) {
            case StellarBodyData.PlanetClass.Telluric:
                distanceRanges = telluricDistanceRanges;
                break;
            case StellarBodyData.PlanetClass.Gas:
                distanceRanges = gasDistanceRanges;
                break;
            case StellarBodyData.PlanetClass.SuperGas:
                distanceRanges = superGasDistanceRanges;
                break;
        }

        
        StellarBodyData.PlanetData planetData = StellarBodyData.instance.GetData(planetClass);
        Planet planet = new Planet(bodyReferences.planet);
        { 
            float mass = Random.Range(planetData.massRange.x, planetData.massRange.y);
            float distance = Random.Range(distanceRanges[index].x, distanceRanges[index].y);
            float eccentricity = Random.Range(planetData.eccentricityRange.x, planetData.eccentricityRange.y);
            float temperatureAtDistance = Universe.FindTemperatureAtDistance(StellarBodyData.instance.GetData(solarSystemData.starClass).luminosity, distance);
            float inclination = Random.Range(planetData.inclinationRange.x, planetData.inclinationRange.y);
            float rotationSpeed = Random.Range(planetData.rotationSpeed.x, planetData.rotationSpeed.y);

            if(temperatureAtDistance > 1500 && !isGas && solarSystemData.onlyPossiblePlanets) {
                Destroy(planet.obj);
                return;
            }

            planet.obj.GetComponent<CelestialObject>().SetValues(
                                                            distance,
                                                            eccentricity,
                                                            bodyReferences.star.GetComponent<CelestialObjectReference>(),
                                                            temperatureAtDistance,
                                                            inclination,
                                                            rotationSpeed);

            planet.obj.GetComponent<CelestialObjectReference>().mass = mass;
            planet.size = Random.Range(planetData.sizeRange.x, planetData.sizeRange.y);
            planet.isGasPlanet = isGas;
            planet.heatGradient = GetPlanetHeatGradient(planet);
            planet.temperature = temperatureAtDistance;
            planet.planetMat = planetData.planetMaterials[Random.Range(0, planetData.planetMaterials.Count)];
            planet.data = planetData;
            planet.distance = distance;
            planet.mass = mass;
            planet.earthLikeScore = Universe.EarthLikeScore(planet, StellarBodyData.instance.GetData(solarSystemData.starClass));
            planet.density = Random.Range(planetData.densityRange.x, planetData.densityRange.y);
            planet.escapeVelocity = Universe.FindEscapeVelocity(planet);
            planets.Add(planet);
        }

    }


    /// <summary>
    /// Generate Moon
    /// </summary>
    private void GenerateMoons(Planet reference, int index) {
        StellarBodyData.PlanetData planetData = StellarBodyData.instance.GetData(StellarBodyData.PlanetClass.Moon);
        Planet moon = new Planet(bodyReferences.moon);

        float distance = Random.Range(moonDistanceRanges[index].x, moonDistanceRanges[index].y) + reference.obj.transform.localScale.x/2;
        float eccentricity = Random.Range(planetData.eccentricityRange.x, planetData.eccentricityRange.y);
        float temperatureAtDistance = Universe.FindTemperatureAtDistance(StellarBodyData.instance.GetData(solarSystemData.starClass).luminosity, distance + reference.distance);

        if (temperatureAtDistance > 1500 && solarSystemData.onlyPossiblePlanets) {
            Destroy(moon.obj);
            return;
        }

        float inclination = Random.Range(planetData.inclinationRange.x, planetData.inclinationRange.y);
        float rotationSpeed = Random.Range(planetData.rotationSpeed.x, planetData.rotationSpeed.y);

        moon.obj.GetComponent<CelestialObject>().SetValues(
                                                        distance,
                                                        eccentricity,
                                                        reference.obj.GetComponent<CelestialObjectReference>(),
                                                        temperatureAtDistance,
                                                        inclination,
                                                        rotationSpeed,
                                                        true);

        moon.size = Random.Range(planetData.sizeRange.x, planetData.sizeRange.y);
        moon.heatGradient = GetPlanetHeatGradient(moon);
        moon.temperature = temperatureAtDistance;
        moon.planetMat = planetData.planetMaterials[Random.Range(0, planetData.planetMaterials.Count)];
        moon.earthLikeScore = Universe.EarthLikeScore(moon, StellarBodyData.instance.GetData(solarSystemData.starClass));
        moon.density = Random.Range(planetData.densityRange.x, planetData.densityRange.y);
        moon.escapeVelocity = Universe.FindEscapeVelocity(moon); moon.data = planetData;
        moons.Add(moon);
    }

    /// <summary>
    /// Randomize the planets
    /// </summary>
    private void RandomizePlanets()
    {
        foreach (Planet p in planets)
        {
            SetBodyShaderParams(p, false);
        }

        foreach (Planet m in moons) {
            SetBodyShaderParams(m, true);
        }
    }

    /// <summary>
    /// Set the shader parameters for a body
    /// </summary>
    private void SetBodyShaderParams(Planet body, bool isMoon) {
        body.obj.name = isMoon ? "Moon_" + Random.Range(0, 5000) : "Planet_" + Random.Range(0, 5000);
        MeshRenderer planetRenderer = body.obj.GetComponent<CelestialObject>().PlanetRenderer;
        planetRenderer.material = new Material(planetRenderer.material);
        if (!body.isGasPlanet) {
            planetRenderer.material.SetFloat("Roughness_1", Random.Range(1f, 1.5f));
            planetRenderer.material.SetFloat("Roughness_2", Random.Range(2f, 3f));
            planetRenderer.material.SetFloat("Roughness_3", Random.Range(4f, 5f));
            planetRenderer.material.SetFloat("Persistence", Random.Range(0.1f, 0.3f));

        }
        else {
            planetRenderer.material.SetFloat("Roughness_1", Random.Range(1f, 2f));
            planetRenderer.material.SetFloat("Roughness_2", 0);
            planetRenderer.material.SetFloat("Roughness_3", 0);
            planetRenderer.material.SetFloat("Persistence", Random.Range(0f, 0.1f));

        }

        planetRenderer.material.SetFloat("Radius", Random.Range(0.8f, 0.9f));
        planetRenderer.material.SetVector("Offset", new Vector4(Random.Range(0f, 100f), Random.Range(0f, 100f), Random.Range(0f, 100f), Random.Range(0f, 100f)));
        planetRenderer.material.SetFloat("Temperature", body.temperature);
        planetRenderer.material.SetTexture("PlanetTexture", body.planetMat.albedo);
        planetRenderer.material.SetTexture("PlanetNormal", body.planetMat.normal);
        body.obj.transform.localScale = Vector3.one * body.size;

        if (Universe.IsEarthLike(body.earthLikeScore)) { 
            body.obj.GetComponentInChildren<PlanetWaterLayer>().SetWaterLayerValue(1.4f, body.temperature-273.15f);
        }

        MeshDisable.instance.AddCollider(body.obj.GetComponent<Collider>());
        PlanetUILocator.instance.AddImageToPlanet(body, StellarBodyData.instance.GetData(solarSystemData.starClass));
    }

    /// <summary>
    /// Generate the star
    /// </summary>
    private void GenerateStar()
    {
        StellarBodyData.StarData starData = StellarBodyData.instance.GetData(solarSystemData.starClass);
        bodyReferences.star.transform.localScale = Vector3.one * starData.size;
        bodyReferences.star.GetComponent<Renderer>().material.SetColor("_StarColor", starData.color);
    }


    /// <summary>
    /// Generate the ranges for each planet type
    /// </summary>
    void SetDistanceRanges(StellarBodyData.PlanetData planetData, int planetCount, List<Vector2> ranges)
    {
        ranges.Clear();
        float rangeScale = (planetData.distanceRange.y - planetData.distanceRange.x) / planetCount;
        for (int i = 0; i < planetCount; i++)
        {
            ranges.Add(new Vector2(planetData.distanceRange.x + (rangeScale * i), planetData.distanceRange.x + (rangeScale * (i + 1))));
        }
    }

    /// <summary>
    /// Return the heat gradient of a planet
    /// </summary>
    private float GetPlanetHeatGradient(Planet planet)
    {
        float temperature = planet.obj.GetComponent<CelestialObject>().temperatureAtDistance;
        float returnValue = 0;
        if(temperature <= Universe.WaterFreezeTemperature)
        {
            //planet.colorSettings = coldColorSettings;

            returnValue = temperature * (1f / Universe.WaterFreezeTemperature);
        }
        else if(temperature >= Universe.WaterBoilTemperature)
        {
            //planet.colorSettings = hotColorSettings;
            returnValue = ((1f / 627f) * temperature) - (373f / 627f);
        }
        else
        {
            //planet.colorSettings = habitableColorSettings;
            returnValue = 0.01f * temperature + (Universe.WaterFreezeTemperature / 100f);
        }

        return Mathf.Clamp01(returnValue);
    }

    /// <summary>
    /// Find the custom planet data
    /// </summary>
    private StellarBodyData.PlanetData FindCustomPlanet(PremadePlanet name) {
        foreach(PremadePlanets p in premadePlanets) {
            if(p.name == name) {
                return p.data;
            }
        }
        return new StellarBodyData.PlanetData();
    }
    private void Awake() {

        //fastTrig = new FastTrig();
    }
}
