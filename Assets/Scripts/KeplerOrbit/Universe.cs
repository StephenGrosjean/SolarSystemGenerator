using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Universe
{
    public static float gravityConstant = 6.67408f;
    public const float G = 6.67f;
    public const float TAU = 6.28318530718f;
    public const float SBConstant = .0000000567f;
    public const float SunLuminosity = 38000000f*1.7f;
    public const float WaterFreezeTemperature = 273; // Temperature of water freeze in Kelvin
    public const float WaterBoilTemperature = 373; // Temperature of water boil in Kelvin
    public const int PlasmaLimit = 3000; //Limit at which matter turn intoPlasma
    public const int AU = 1496;
    public const float EarthMass = 5.97f; //Earth mass in simulation unit
    public const float EarthSize = 12.7f; //Earth size in simulation unit
    public const float EarthDensity = 5.5f; //Earth density ingm/cm3
    public const float EarthEscapeVelocity = 11.19f; //Earth escape density in km/s
    public const float EarthRadius = 6.35f; //Earth radius in simulation unit
    public const float EarthSurfaceTemperature = 288; //Earth surface temperature in Kelvin
    public const float SunTemperature = 5700; //Sun Temperature in K
    public const float HZDEarth = -.52f;
    public const float HZAEarth = -.5f;
    public const float ESIEarth = 1f;

    private const float radiusWeight = 0.57f;
    private const float densityWeight = 1.07f;
    private const float escapeVelocityWeight = 0.7f;
    private const float temperatureWeight = 5.58f;


    //HZD constants
    private const float ai = 2.7619e-5f;
    private const float bi = 3.8095e-9f;
    private const float ao = 1.3786e-4f;
    private const float bo = 1.4286e-9f;
    private const float ris = 0.72f;
    private const float ros = 1.77f;

    /// <summary>
    /// Find Habitable zone using formula : d = sqrt(L/4*PI*SB*T^4)
    /// </summary>
    /// <param name="luminosity"></param>
    /// <returns></returns>
    public static Vector2 FindHabitableZone(StellarBodyData.StarData.Luminosity luminosity)
    {
        Vector2 returnZone = new Vector2();
        float lum = luminosity.value * Mathf.Pow(10, luminosity.power) * SunLuminosity;
        returnZone.x = Mathf.Sqrt(lum / (4 * Mathf.PI * SBConstant * Mathf.Pow(273,4)));
        returnZone.y = Mathf.Sqrt(lum / (4 * Mathf.PI * SBConstant * Mathf.Pow(373, 4)));
        return returnZone;
    }

    /// <summary>
    /// Find Temperature at distance using formula : T = ((L/4*PI*r^2)/SB)^(1/4)
    /// </summary>
    /// <param name="luminosity"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    public static float FindTemperatureAtDistance(StellarBodyData.StarData.Luminosity luminosity, float distance)
    {
        float lum = luminosity.value * Mathf.Pow(10, luminosity.power) * SunLuminosity;

        return Mathf.Pow((lum / (4 * Mathf.PI * Mathf.Pow(distance/10, 2))) / SBConstant, 0.25f);
    }

    /// <summary>
    /// Return the Unix time since 1st January 1970
    /// </summary>
    public static int UnixTime() {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        return cur_time;
    }

    /// <summary>
    /// Return the escape velocity of a planet
    /// </summary>
    public static float FindEscapeVelocity(SolarSystemGenerator.Planet planet) {
        return Mathf.Sqrt((2*gravityConstant * planet.mass) / (1000*(planet.size/2)))*100;
    }

    /// <summary>
    /// Return the Teqq off a planet with an albedo of 0.3
    /// </summary>
    public static float EquilibriumTemperature(SolarSystemGenerator.Planet planet, StellarBodyData.StarData starData) {
        return (starData.temperature * Mathf.Pow(.7f, .25f)) * Mathf.Pow((starData.size * 1e4f) / (planet.distance * 2e5f), 0.5f);
    }

    /// <summary>
    /// Return the HZA of a planet
    /// </summary>
    public static float HZA(SolarSystemGenerator.Planet planet, StellarBodyData.StarData starData) {
        float teq = EquilibriumTemperature(planet, starData);
        float veh = Mathf.Pow(0.02f * teq, 0.5f);
        float ven = Mathf.Pow((0.02f * teq) / 14, 0.5f);
        return ((2 * Mathf.Pow(planet.mass/ EarthMass * (planet.size/ EarthSize), 0.5f))- veh - ven) / (veh - ven);
    }

    /// <summary>
    /// Return the ESI of a planet
    /// </summary>
    public static float ESI(SolarSystemGenerator.Planet planet) {
        float radius = Mathf.Pow((1 - ((planet.size / 2) - (EarthSize/2)) / ((planet.size / 2) + (EarthSize/2))), radiusWeight / 4);
        float density = Mathf.Pow((1 - (planet.density - EarthDensity) / (planet.density + EarthDensity)), densityWeight / 4);
        float escapeVel = Mathf.Pow((1 - (planet.escapeVelocity - EarthEscapeVelocity) / (planet.escapeVelocity + EarthEscapeVelocity)), escapeVelocityWeight / 4);
        float temperature = Mathf.Pow((1 - (planet.temperature - EarthSurfaceTemperature) / (planet.temperature + EarthSurfaceTemperature)), temperatureWeight / 4);
        return radius * density * escapeVel * temperature;
    }

    /// <summary>
    /// Return the HZD of a planet
    /// </summary>
    public static float HZD(SolarSystemGenerator.Planet planet, StellarBodyData.StarData starData) {
        float L = Mathf.Pow(Mathf.Pow(starData.luminosity.value, starData.luminosity.power), .5f);
        float temp = (starData.temperature - SunTemperature);

        float ri = ((ris - (ai * temp)) - (bi * (temp * temp))) * Mathf.Sqrt(L);
        float ro = ((ros - (ao * temp)) - (bo * (temp * temp))) * Mathf.Sqrt(L);
        float r = AU/planet.distance;
        return ((2 * r) - ro - ri) / (ro - ri);
    }

    /// <summary>
    /// Return the Earth like score of a planet, based on the difference with earth values
    /// </summary>
    public static float EarthLikeScore(SolarSystemGenerator.Planet planet, StellarBodyData.StarData starData) {
        float planetHZA = HZA(planet, starData);
        float planetESI = ESI(planet);
        float planetHZD = HZD(planet, starData);

        float HZADelta = Mathf.Abs(Mathf.Abs(planetHZA) - Mathf.Abs(HZAEarth));
        float ESIDelta = Mathf.Abs(Mathf.Abs(planetESI) - Mathf.Abs(ESIEarth));
        float HZDDelta = Mathf.Abs(Mathf.Abs(planetHZD) - Mathf.Abs(HZDEarth));
        return HZADelta + ESIDelta + HZDDelta;
    }

    public static bool IsEarthLike(float score) {
         return score <= 1f;    
    }
}
