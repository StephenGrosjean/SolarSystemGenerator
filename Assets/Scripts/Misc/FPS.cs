using UnityEngine;
using System.Collections;
using TMPro;
public class FPS : MonoBehaviour
{

	private string label = "";
	private float count;
	public TextMeshProUGUI fpsText;
	IEnumerator Start() {
		while (true) {
			if (Time.timeScale == 1) {
				yield return new WaitForSeconds(0.1f);
				count = (1 / Time.deltaTime);
				label = (Mathf.Round(count)) + " FPS";
			}
			else {
				label = "Pause";
			}
			UpdateUI();
			yield return new WaitForSeconds(0.5f);
		}
	}

	void UpdateUI() {
		fpsText.text = label;
	}
}