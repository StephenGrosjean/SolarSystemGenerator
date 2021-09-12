using System.Collections.Generic;
using UnityEngine;
using System;
public class StellarBodyData : MonoBehaviour
{
    public static StellarBodyData instance;
    public enum StarClass
    {
        O = 0,
        B,
        A,
        F,
        G,
        K,
        M
    }

    public enum PlanetClass
    {
        Telluric,
        Moon,
        Gas,
        SuperGas
    }

    [Serializable]
    public struct StarData
    {
        [ColorUsage(true, true)] public Color color;
        public StarClass starClass; 
        public float mass;
        public float size;
        public Luminosity luminosity;
        public float temperature;

        [Serializable]
        public class Luminosity
        {
            public float value;
            public float power;
        }
    }
    

    [Serializable]
    public struct PlanetData
    {
        public PlanetClass planetClass;
        public Vector2 distanceRange;
        public Vector2 eccentricityRange;
        public Vector2 inclinationRange;
        public Vector2 sizeRange;
        public Vector2 rotationSpeed;
        public Vector2 massRange;
        public Vector2 densityRange;
        public Vector2Int moonsCountRange;
        public List<PlanetMat> planetMaterials;
    }

    [Serializable]
    public class PlanetMat
    {
        public Texture2D albedo;
        public Texture2D normal;
    }

    [SerializeField] private List<PlanetData> planets = new List<PlanetData>();
    [SerializeField] private List<StarData> stars = new List<StarData>();

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Return the PlanetData of the PlanetClass
    /// </summary>
    public  PlanetData GetData(PlanetClass planetClass)
    {
        foreach(PlanetData p  in planets)
        {
            if (p.planetClass == planetClass)
                return p;
        }
        return new PlanetData();
    }

    /// <summary>
    /// Return the StarData of the StarClass
    /// </summary>
    public  StarData GetData(StarClass starClass)
    {
        foreach (StarData s in stars)
        {
            if (s.starClass == starClass)
                return s;
        }
        return new StarData();
    }

}
