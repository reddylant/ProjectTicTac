using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultCheck : MonoBehaviour
{
    public bool Vault()
    {
        bool result = false;
        float distance = 1;

        RaycastHit hipHit;
        Vector3 origin = this.transform.position;
        //origin.y += 1;
        Vector3 direction = this.transform.forward;
        direction.y += 1; 

        Debug.DrawRay(origin, direction * distance);
        if (Physics.Raycast(origin, direction, out hipHit, distance))
        {
            Debug.Log("Vault hit");
            result = true;
        }

        return result;
    }
}
