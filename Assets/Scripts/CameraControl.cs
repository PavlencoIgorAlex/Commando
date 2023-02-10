using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private GameManager gameManager;

    private float verticalMouseInput;
    private float sensitivityMouse;
    private bool mouseInverted;
    public GameObject player;
    private bool isInitialized = false;

    public float maxRotation;
    public float minRotation;
    private float curMaxRotation;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        minRotation = minRotation < 0 ? -minRotation : minRotation;
        curMaxRotation = 360f - maxRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive != false)
        {
            if (!isInitialized)
            {
                mouseInverted = PlayerController.mouseInvertedGlobal;
                sensitivityMouse = PlayerController.sensitivityMouseGlobal;
                sensitivityMouse = mouseInverted ? -sensitivityMouse : sensitivityMouse;

                isInitialized = true;
            }
            else
            {
                verticalMouseInput = Input.GetAxis("Mouse Y");
                transform.Rotate(Vector3.right * Time.deltaTime * verticalMouseInput * sensitivityMouse);

                Vector3 currentRotation = transform.localRotation.eulerAngles;
                currentRotation.x = (curMaxRotation > currentRotation.x && currentRotation.x > 180) ? curMaxRotation : currentRotation.x;
                currentRotation.x = (180 > currentRotation.x && currentRotation.x > minRotation) ? minRotation : currentRotation.x;
                transform.localRotation = Quaternion.Euler(currentRotation);
            }
        }
    }
}