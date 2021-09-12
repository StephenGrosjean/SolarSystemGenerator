using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public SolarSystemGenerator generator;
    public GameObject pauseMenu;
    public TextMeshProUGUI infoText;
    public bool isPaused;

    private void Start() {
        SolarSystemGenerator.SolarSystemData data = generator.GetData();
        string starClass = "";
        switch (data.starClass) {
            case StellarBodyData.StarClass.O:
                starClass = "O";
                break;
            case StellarBodyData.StarClass.B:
                starClass = "B";
                break;
            case StellarBodyData.StarClass.A:
                starClass = "A";
                break;
            case StellarBodyData.StarClass.F:
                starClass = "F";
                break;
            case StellarBodyData.StarClass.G:
                starClass = "G";
                break;
            case StellarBodyData.StarClass.K:
                starClass = "K";
                break;
            case StellarBodyData.StarClass.M:
                starClass = "M";
                break;
        }

        infoText.text = "Seed : " + data.seed + "\n" +
                        "Telluric planets : " + data.telluricPlanetCount + "\n" +
                        "Gas planets : " + data.gasPlanetCount + "\n" +
                        "Super gas planets : " + data.superGasPlanetCount + "\n" +
                        "Star Type : " + starClass + "\n" +
                        "Only possible planets : " + (data.onlyPossiblePlanets ? "True" : "False");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        }
        isPaused = pauseMenu.activeInHierarchy;
    }

    public void ReturnToMenu() {
        SceneManager.LoadScene("Menu");
    }

    public void Continue() {
        pauseMenu.SetActive(false);
    }
}

