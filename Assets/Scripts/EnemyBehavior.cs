using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    private GameManager gameManager;
    private Animator enemyAnimator;
    private string nameOfTarget = "Player";
    private Transform target;

    private Transform eyes;
    public float sightRange;
    private bool inCombat = false;
    public float fireRange;
    public float closeRange;

    public float speed;

    private bool inMotion = false; private bool tmp_inMotion = false;

    private RaycastShootEnemy shooting;

    //reaction
    public float reaction;
    public float nextReaction;
    private GameObject emptyGO;
    private Transform oldTarget;

    // Start is called before the first frame update
    void Start()
    {
        eyes = this.transform.GetChild(1);
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        enemyAnimator = this.transform.GetComponentInChildren<Animator>();
        shooting = this.transform.GetComponentInChildren<RaycastShootEnemy>();

        target = GameObject.Find(nameOfTarget).transform;
        emptyGO = new GameObject();
        oldTarget = emptyGO.transform;
        oldTarget.position = target.position;
        nextReaction = Time.time + reaction;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive)
        {
            RaycastHit hit;
            target = GameObject.Find(nameOfTarget).transform;
            if (Physics.Raycast(eyes.position, (target.position - eyes.position).normalized, out hit, sightRange))
            {
                string RaycastReturn = hit.collider.gameObject.name;
                if (RaycastReturn == nameOfTarget)
                {

                    if (!inCombat)
                    {
                        enemyAnimator.SetTrigger("inCombat");
                        inCombat = true;
                    }

                    transform.LookAt(oldTarget);
                    if (closeRange < (oldTarget.position - eyes.position).magnitude)
                    {
                        transform.Translate(Vector3.forward * Time.deltaTime * speed);
                        inMotion = true;
                    } else
                    {
                        inMotion = false;
                    }
                    if (inMotion != tmp_inMotion)
                    {
                        tmp_inMotion = inMotion;
                        if (inMotion)
                        {
                            enemyAnimator.SetTrigger("inMotion");
                        }
                        else
                        {
                            enemyAnimator.ResetTrigger("inMotion");
                        }
                    }

                    if (fireRange > (oldTarget.position - eyes.position).magnitude)
                    {
                        shooting.Fire(oldTarget.position);
                    }
                }
            }
            if (Time.time > nextReaction)
            {
                nextReaction = Time.time + reaction;
                oldTarget.position = target.position;
            }
        }
    }
}
