using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject player;
    private Vector3 offset;
    private Vector3 playerLastPosition;

    public float offsetX;
    public float offsetY;
    public float offsetZ;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        offset = new Vector3(offsetX, offsetY, offsetZ);
        playerLastPosition = player.transform.position;
        transform.position = playerLastPosition + offset;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive != false)
        {
            transform.RotateAround(player.transform.position, Vector3.up, player.transform.localRotation.eulerAngles.y - transform.localRotation.eulerAngles.y);
            transform.position += player.transform.position - playerLastPosition;
            playerLastPosition = player.transform.position;
        }
    }
}
