using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastShootEnemy : MonoBehaviour
{
    private GameManager gameManager;
    private Transform gunEnd;

    private GunProfile curGunProfile;
    private WaitForSeconds shotDurationWfS;
    private GameObject gunEffects;

    // Start is called before the first frame update
    void Start()
    {
        gunEnd = this.transform.GetChild(0);
        gunEffects = this.transform.GetChild(1).gameObject;

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        shotDurationWfS = new WaitForSeconds(0);
        gunEffects.gameObject.SetActive(false);

        curGunProfile = this.transform.GetComponentInChildren<GunProfile>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (gameManager.isGameActive)
        //{
        //}
    }

    public void Fire(Vector3 target)
    {
        if (Time.time > curGunProfile.nextFire)
        {
            curGunProfile.nextFire = Time.time + curGunProfile.fireRate;
            StartCoroutine(ShotEffect());
            RaycastHit hit;
            curGunProfile.bulletRay.SetPosition(0, gunEnd.position);
            if (Physics.Raycast(gunEnd.position, gunEnd.forward, out hit, curGunProfile.gunRange))
            {
                curGunProfile.bulletRay.SetPosition(1, hit.point);
                Shootable health = hit.collider.GetComponent<Shootable>();
                if (health != null)
                {
                    health.Damage(curGunProfile.gunDamage);
                }
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * curGunProfile.gunForce);
                }
            }
            else
            {
                curGunProfile.bulletRay.SetPosition(1, gunEnd.position + gunEnd.forward * curGunProfile.gunRange);
            }
        }
    }

    private IEnumerator ShotEffect()
    {
        shotDurationWfS = new WaitForSeconds(curGunProfile.shotDuration);

        curGunProfile.bulletRay.enabled = true;
        curGunProfile.gunAudio.Play();
        gunEffects.gameObject.SetActive(true);

        yield return shotDurationWfS;

        curGunProfile.bulletRay.enabled = false;
        gunEffects.gameObject.SetActive(false);

    }

}
