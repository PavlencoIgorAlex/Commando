using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastShootPlayer : MonoBehaviour
{
    private GameManager gameManager;

    private Transform gunEnd;
    private Camera camera;
    private GunProfile curGunProfile;
    private WaitForSeconds shotDurationWfS;
    private GameObject gunEffects;

    private bool isFire1Pressed = false;

    // Start is called before the first frame update
    void Start()
    {
        gunEnd = this.transform.GetChild(0);
        gunEffects = this.transform.GetChild(1).gameObject;

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        shotDurationWfS = new WaitForSeconds(0);
        gunEffects.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.isGameActive)
        {
            curGunProfile = gameManager.GetGunProfile();

            if (curGunProfile.isAutomatic)
            {
                isFire1Pressed = Input.GetButtonUp("Fire1") ? false : isFire1Pressed;
                isFire1Pressed = Input.GetButtonDown("Fire1") ? true : isFire1Pressed;
            }
            else
            {
                isFire1Pressed = Input.GetButtonDown("Fire1");
            }

            if (isFire1Pressed &&
                Time.time > curGunProfile.nextFire &&
                (gameManager.GetCurAmmoRemain() - curGunProfile.shotConsume >= 0 || curGunProfile.isInfiniteAmmo))
            {
                gameManager.UpdateAmmo(curGunProfile.shotConsume);
                curGunProfile.nextFire = Time.time + curGunProfile.fireRate;
                StartCoroutine(ShotEffect());
                Vector3 rayOrigin = camera.ViewportToWorldPoint(new Vector3(.5f, .5f, 0));
                RaycastHit hit;
                curGunProfile.bulletRay.SetPosition(0, gunEnd.position);
                if (Physics.Raycast(rayOrigin, camera.transform.forward, out hit, curGunProfile.gunRange))
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
                    curGunProfile.bulletRay.SetPosition(1, rayOrigin + camera.transform.forward * curGunProfile.gunRange);
                }
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
