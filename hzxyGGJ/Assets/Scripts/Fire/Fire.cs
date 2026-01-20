using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public float FireLiveTime;
    public List<GameObject> smallFire = new();
    public Transform smallFireTransform;
    public GameObject fireUIPrefab;
    public GameObject firePrefab;
    private int FireAmount;
    public int MaxFireAmount;
    public float fireTime;
    private float usedFireTime;
    
    public float xYelocity;

    void Awake()
    {
        FireAmount = MaxFireAmount;
    }

    void Start()
    {
        for(int i = 0; i < FireAmount; i++)
        {
            smallFire.Add(Instantiate(fireUIPrefab,smallFireTransform));
        }
    }

    void Update()
    {
        usedFireTime -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            FirefirePrefab();
        }
    }

    [ContextMenu("·¢Éä»ðÃç")]
    public void FirefirePrefab()
    {
        if(FireAmount<=0) return;

        if(usedFireTime<=0) usedFireTime = fireTime;
        else return;

        FireDecrease();

        GameObject newFire = Instantiate(firePrefab,transform.position,Quaternion.identity);

        FirePrefab newScript =  newFire.GetComponent<FirePrefab>();

        newScript.SetupFirePrefab(xYelocity + GetComponent<Rigidbody2D>().velocity.x,FireLiveTime);
    }

    public void FireIncrease()
    {
        if(FireAmount == MaxFireAmount) return;

        FireAmount++;

        smallFire.Add(Instantiate(fireUIPrefab,smallFireTransform));
    }

    public void FireDecrease()
    {
        FireAmount--;

        var currentFire = smallFire[smallFire.Count-1];
        
        smallFire.Remove(smallFire[smallFire.Count-1]);

        Destroy(currentFire);

    }

    
}
