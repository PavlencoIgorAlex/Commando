using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootable : MonoBehaviour
{
    private GameManager gameManager;

    public int currentHealth;
    public int score;
    public int frag;
    public void Damage(int gunDamage)
    {
        currentHealth -= gunDamage;
        if (currentHealth <= 0)
        {
            if (gameObject.name != "Player")
            {
                gameObject.SetActive(false);
                gameManager.UpdateScore(score);
                gameManager.UpdateFrags(frag);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
