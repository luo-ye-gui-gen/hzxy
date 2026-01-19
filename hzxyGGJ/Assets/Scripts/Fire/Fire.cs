using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public GameObject firePrefab;
    
    public float xYelocity;

    [ContextMenu("·¢Éä»ðÃç")]
    public void FirefirePrefab()
    {
        

        GameObject newFire = Instantiate(firePrefab,transform.position,Quaternion.identity);

        FirePrefab newScript =  newFire.GetComponent<FirePrefab>();

        newScript.SetupFirePrefab(xYelocity + GetComponent<Rigidbody2D>().velocity.x);
    }

    
}
