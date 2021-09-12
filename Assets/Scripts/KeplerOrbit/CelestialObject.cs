using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialObject : MonoBehaviour
{
    private class OrbitalPoints
    {
        public Vector3 pos;
        public OrbitalPoints(Vector3 position) {
            pos = position;
        }
    }

    //Public//

    public MeshRenderer PlanetRenderer;
    public float temperatureAtDistance;
    public float rotationSpeed;
    public float meanVelocity;
    public PlanetInfoUI planetInfoUI;


    //Private//

    //Keplerian Parameters
    [SerializeField] private int orbitResolution = 50;
    [SerializeField] private float semiMajorAxis = 20f;                                 // a - size
    [SerializeField] [Range(0f, 0.99f)] private float orbitalEccentricity;              // e - shape
    [SerializeField] [Range(0f, Universe.TAU)] private float orbitalInclination = 0f;   // i - tilt
    [SerializeField] [Range(0f, Universe.TAU)] private float longitudeOfAcendingNode;   // n - swivel
    [SerializeField] [Range(0f, Universe.TAU)] private float argumentOfPeriapsis;       // w - position
    [SerializeField] private float meanLongitude;                                       // L - offset
    [SerializeField] private CelestialObjectReference referenceBody;
    [SerializeField] private float meanAnomaly;

    private float baseOrbitOffset;
    private Rigidbody rigid;   
    private float velocity = 0;
    public bool stop;
    private int velCount = 0;
    private List<OrbitalPoints> orbitalPoints = new List<OrbitalPoints>();
    private bool isVelRunning = true;
  
    //Settings
    private float accuracyTolerance = 1e-6f;
    private int maxIterations = 5;


    //Constants per orbit
    private float mu;
    private float n;
    private float cosLOAN;
    private float sinLOAN;
    private float sinI;
    private float cosI;
    private float trueAnomalyConstant;

    //Other settings
    private FastTrig fastTrig;
    private bool isMoon;

    private void Awake()
    {
        Invoke("EnableVelocityCalculation", 2);
    }

    private void Start() {
        baseOrbitOffset = Random.Range(0, 100.0f);
        rigid = GetComponent<Rigidbody>();
        CalculateSemiConstants();
        CalculatePoints();
        SetPoints();
    }

    /// <summary>
    /// Calculate each frame the different orbit values and next position
    /// </summary>
    private void Update() {
        if (!stop)
            meanAnomaly = CalculateMeanAnomaly() + baseOrbitOffset;

        float EccentricAnomaly = CalculateEccentricAnomaly(meanAnomaly);

        float trueAnomaly = CalculateTrueAnomaly(EccentricAnomaly);

        transform.position = CalculatePosition(EccentricAnomaly, trueAnomaly) + referenceBody.transform.position;

        rigid.angularVelocity = new Vector3(0, 1, 0) * rotationSpeed * 1.67f * TimeScaler.instance.multiplicator;

        if (isMoon) {
            CalculatePoints();
            SetPoints();
        }
    }

    private void FixedUpdate() {
        if (!isVelRunning) {
            StartCoroutine("CalculateVelocity");
        }
    }

    /// <summary>
    /// Set the values for each Keplerian parameters
    /// </summary>
    public void SetValues(float semiMajorAxis, float eccentricity, CelestialObjectReference orbitReference, float temp, float inclination, float rotationSpeed, bool isMoon = false) {
        this.semiMajorAxis = semiMajorAxis;
        this.referenceBody = orbitReference;
        this.orbitalEccentricity = Mathf.Clamp(eccentricity, 0, .99f);
        this.temperatureAtDistance = temp;
        this.orbitalInclination = inclination * Mathf.PI / 180;
        this.rotationSpeed = rotationSpeed;
        this.isMoon = isMoon;
    }

    /// <summary>
    /// Calculate the "Semi Constants" for the current orbit
    /// </summary>
    void CalculateSemiConstants()
    {
         mu = Universe.G * referenceBody.mass;
         n = Mathf.Sqrt(mu / Mathf.Pow(semiMajorAxis, 3));
         trueAnomalyConstant = Mathf.Sqrt((1 + orbitalEccentricity) / (1 - orbitalEccentricity));
         cosLOAN = Mathf.Cos(longitudeOfAcendingNode);
         sinLOAN = Mathf.Sin(longitudeOfAcendingNode);
         cosI = Mathf.Cos(orbitalInclination);
         sinI = Mathf.Sin(orbitalInclination);
    }

    /// <summary>
    /// Function f(x) = 0
    /// </summary>
    private float F(float E, float e, float M)
    {
        return (M - E + e * Mathf.Sin(E));
        //return (M - E + e * fastTrig.Sin(E));

    }

    /// <summary>
    /// Derivative of the function f(x) = 0
    /// </summary>
    private float DF(float E, float e)
    {
        return (-1f) + e * Mathf.Cos(E);
        //return (-1f) + e * fastTrig.Cos(E);
    }

    /// <summary>
    /// Calculate the mean anomaly
    /// </summary>
    private float CalculateMeanAnomaly()
    {
        return (n * ((TimeScaler.instance.SimulationTime) - meanLongitude));
    }

    /// <summary>
    /// Calculate eccentric anomaly
    /// </summary>
    private float CalculateEccentricAnomaly(float meanAnomaly)
    {
        float difference = 1f;
        for(int i = 0; difference > accuracyTolerance && i < maxIterations; i++)
        {
            float E0 = meanAnomaly;
            meanAnomaly = E0 - F(E0, orbitalEccentricity, meanAnomaly) / DF(E0, orbitalEccentricity);
            difference = Mathf.Abs(meanAnomaly - E0);
        }

        return meanAnomaly;
    }

    /// <summary>
    /// Calculate true anomaly
    /// </summary>
    private float CalculateTrueAnomaly(float eccentricAnomaly)
    {
        return 2 * Mathf.Atan(trueAnomalyConstant * Mathf.Tan(eccentricAnomaly / 2));
    }

    /// <summary>
    /// Calculate the position in the orbit at the true anomaly 
    /// </summary>
    private Vector3 CalculatePosition(float eccentricAnomaly, float trueAnomaly)
    {
        float distance = semiMajorAxis * (1 - orbitalEccentricity * Mathf.Cos(eccentricAnomaly));

        float cosAOPPlusTA = Mathf.Cos(argumentOfPeriapsis + trueAnomaly);
        float sinAOPPlusTA = Mathf.Sin(argumentOfPeriapsis + trueAnomaly);

        float x = distance * ((cosLOAN * cosAOPPlusTA) - (sinLOAN * sinAOPPlusTA * cosI));
        float z = distance * ((sinLOAN * cosAOPPlusTA) + (cosLOAN * sinAOPPlusTA * cosI));      //Switching z and y to be aligned with xz not xy
        float y = distance * (sinI * sinAOPPlusTA);

        return new Vector3(x, y, z);
    }


    /// <summary>
    /// Calculate the points for the orbit line renderer
    /// </summary>
    private void CalculatePoints()
    {
        orbitalPoints.Clear();
        float orbitFraction = 1f / orbitResolution;

        for(int i= 0; i < orbitResolution; i++)
        {
            float EccentricAnomaly = i * orbitFraction * Universe.TAU;

            float trueAnomaly = CalculateTrueAnomaly(EccentricAnomaly);
            float meanAnomaly = EccentricAnomaly - orbitalEccentricity * Mathf.Sin(EccentricAnomaly);

            orbitalPoints.Add(new OrbitalPoints(CalculatePosition(EccentricAnomaly, trueAnomaly) + referenceBody.transform.position));
        }
    }

    /// <summary>
    /// Set the points for the line renderer
    /// </summary>
    private void SetPoints() {
        LineRenderer lr = GetComponent<LineRenderer>();

        Vector3[] positions = new Vector3[orbitalPoints.Count];
        for (int i = 0; i < orbitalPoints.Count; i++) {
            positions[i] = orbitalPoints[i].pos;
        }
        lr.positionCount = orbitalPoints.Count;
        lr.SetPositions(positions);
    }

    /// <summary>
    /// Enable the velocity calculation
    /// </summary>
    private void EnableVelocityCalculation()
    {
        isVelRunning = false;
    }

    /// <summary>
    /// Calculate the planet velocity (Only work when time is = 1)
    /// </summary>
    private IEnumerator CalculateVelocity() {
        isVelRunning = true;
        Vector3 pos1 = transform.position;
        yield return new WaitForSecondsRealtime(1);
        Vector3 pos2 = transform.position;
        velocity += Vector3.Distance(pos1, pos2);
        velCount++;
        meanVelocity = velocity / velCount;
        isVelRunning = false;
    }
}
