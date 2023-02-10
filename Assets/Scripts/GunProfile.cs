using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunProfile : MonoBehaviour
{
    public int gunDamage;
    public float gunRange;
    public float gunForce;
    public int gunCount;
    public bool isInfiniteAmmo;

    public float fireRate;
    public float nextFire;
    public bool isAutomatic;
    public int shotConsume;

    public float shotDuration;
    public LineRenderer bulletRay;
    public AudioSource gunAudio;

    public int scoreMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        bulletRay = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
