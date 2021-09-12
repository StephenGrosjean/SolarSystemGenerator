using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    public TextMeshProUGUI telluricText;
    public TextMeshProUGUI gasText;
    public TextMeshProUGUI superGasText;

    public int telluricCount;
    public int gasCount;
    public int superGasCount;
    public StellarBodyData.StarClass starClass;

    [SerializeField] private List<Color> starClassColors = new List<Color>();
    private bool onlyPossiblePlanets;
    private int seed = -1;
    public int planetLimit;
    public enum Type
    {
        Telluric = 0,
        Gas = 1,
        SuperGas = 2
    }

    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode) { 
        if (SceneManager.GetActiveScene().name == "Menu")
            DontDestroyOnLoad(gameObject);
        else {
            GameObject.FindObjectOfType<SolarSystemGenerator>().SetSystemData(telluricCount, gasCount, superGasCount, starClass, onlyPossiblePlanets, seed);
            SpeedTest.instance.PlanetCount = telluricCount + gasCount + superGasCount;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(gameObject);
        }
    }

    public void GotoScene() {
        SceneManager.LoadScene("TestKeplerOrbit");
    }

    public void UpdateCount() {
        telluricText.text = telluricCount.ToString();
        gasText.text = gasCount.ToString();
        superGasText.text = superGasCount.ToString();
        
    }

    public void AddToCounter(int t) {
        Type type = (Type)t;
        switch (type) {
            case Type.Telluric:
                if (telluricCount + 1 > planetLimit) break;
                telluricCount++;
                break;
            case Type.Gas:
                if (gasCount + 1 > planetLimit) break;
                gasCount++;
                break;
            case Type.SuperGas:
                if (superGasCount + 1 > planetLimit) break;
                superGasCount++;
                break;
        }
        UpdateCount();
    }
    public void SubToCounter(int t) {
        Type type = (Type)t;
        switch (type) {
            case Type.Telluric:
                if (telluricCount - 1 < 0) break;
                telluricCount--;
                break;
            case Type.Gas:
                if (gasCount - 1 < 0) break;
                gasCount--;
                break;
            case Type.SuperGas:
                if (superGasCount - 1 < 0) break;
                superGasCount--;
                break;
        }
        UpdateCount();
    }

    public void UpdateStarType(TMP_Dropdown dropdown) {
        starClass = (StellarBodyData.StarClass)dropdown.value;
        dropdown.GetComponent<Image>().color = starClassColors[dropdown.value];
    }

    public void UpdateOnlyPossiblePlanet(Toggle toggle) {
        onlyPossiblePlanets = toggle.enabled;
    }

    public void UpdateSeed(TMP_InputField input) { 
       
        if(input.text.Length > 0) {
            long parsedValue = long.Parse(input.text);
            if(parsedValue > int.MaxValue) {
                parsedValue = int.MaxValue;
                input.text = parsedValue.ToString();
            }
            int value = (int)parsedValue;
            if (value >= 0) {
                seed = value;
            }
            else {
                seed = 0;
                input.text = "0";
            }
        }
        else {
            seed = -1;
        }
        Debug.Log(seed);
    }

    public void Quit() {
        Application.Quit();
    }
}
