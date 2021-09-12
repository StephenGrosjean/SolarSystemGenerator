using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CameraController : MonoBehaviour
{
    [SerializeField] private float fowardSpeed;
    [SerializeField] private float fastMultiplicator;

    [SerializeField] private float verticalSpeed;

    [SerializeField] private float lookSensitivity;
    Vector2 rotation = Vector2.zero;
    public bool lockX, lockY, lockZ;
    public PauseMenu pauseMenu;
    private Vector3 startRotation;
    private Vector2 inputs;
    private Vector3 followOffset;
    private Transform followed;
    private bool isFollowing;
    private float currentMultiplicator;

    void Start() {
        startRotation = transform.rotation.eulerAngles; 
        Cursor.lockState = CursorLockMode.Locked;
        inputs = Vector2.zero;
    }
    void Update()
    {
        if (!pauseMenu.isPaused) {
            if (Cursor.lockState == CursorLockMode.Locked || Cursor.lockState == CursorLockMode.Confined) {
                Cursor.visible = false;
                if (Input.GetKeyDown(KeyCode.Tab)) {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }

                currentMultiplicator = Input.GetKey(KeyCode.LeftShift) ? fastMultiplicator : 1;
                inputs.x = Input.GetAxis("Vertical");
                inputs.y = Input.GetAxis("Horizontal");
                transform.position += transform.forward * inputs.x * fowardSpeed *currentMultiplicator;
                transform.position += transform.right * inputs.y * fowardSpeed * currentMultiplicator;

                if (Input.GetKey(KeyCode.Space)) {
                    transform.position += Vector3.up * verticalSpeed;
                }
                if (Input.GetKey(KeyCode.LeftControl)) {
                    transform.position += -Vector3.up * verticalSpeed;
                }



                transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * lookSensitivity, Space.Self);
                transform.Rotate(Vector3.right, -Input.GetAxis("Mouse Y") * lookSensitivity, Space.Self);
            }
            else {
                if (Input.GetKeyDown(KeyCode.Tab)) {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }

            if (Cursor.visible) {
                if (Input.GetAxis("Fire1") > 0) {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (!EventSystem.current.IsPointerOverGameObject()) {
                        if (Physics.Raycast(ray, out hit)) {
                            if (hit.transform.tag == "CelestialBody") {
                                hit.transform.GetComponent<CelestialObject>().planetInfoUI.ShowInfo();
                            }
                        }
                        else {
                            PlanetUILocator.instance.HideInfo();
                        }
                    }

                }
            }
            if (Input.GetKeyDown(KeyCode.F)) {
                isFollowing = false;
            }
        }
        else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
    }

    void LateUpdate() {
        if (isFollowing) {
            transform.position = followed.position + followOffset;
        }

        Vector3 newRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(
            lockX ? startRotation.x : newRotation.x,
            lockY ? startRotation.y : newRotation.y,
            lockZ ? startRotation.z : newRotation.z
        );


    }

    public void FollowPlanet(Transform planet) {
        isFollowing = true;
        followOffset = transform.position - planet.position;
        followed = planet;
    }
}
