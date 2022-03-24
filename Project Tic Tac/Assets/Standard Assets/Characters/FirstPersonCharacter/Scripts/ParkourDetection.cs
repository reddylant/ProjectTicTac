using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourDetection : MonoBehaviour
{
    public RaycastHit hitVert;
    public RaycastHit hitHor;

    float minDistance = .25f;

    //Checks for vaultable surface
    public bool Vault()
    {
        bool result = false;
        float vaultDistance = 1;
        
        Vector3 origin = this.transform.position;
        Vector3 direction = this.transform.forward;
        //Checks if object is too tall
        Vector3 originHorizontal = this.transform.position;
        originHorizontal.y += .4f;
        Vector3 directionHorizontal = this.transform.forward;

        Debug.DrawRay(origin, direction * vaultDistance, Color.red);
        Debug.DrawRay(originHorizontal, directionHorizontal * vaultDistance, Color.red);
        if (Physics.Raycast(origin, direction, out hitHor, vaultDistance) &&
            !Physics.Raycast(originHorizontal, directionHorizontal, out hitVert, vaultDistance))
        {
            //Debug.Log("Vault hit");
            result = true;
        }
        else
        {
            //Debug.Log("No Vault Hit");
        }

        return result;
    }

    //Detects if the vaultable obejct is actually a small step needed to be climbed
    public bool StepUp()
    {
        bool result = false;
        float stepDistance = 1;

        Vector3 origin = transform.position + (transform.forward * .5f);
        origin.y += 1;
        Vector3 direction = Vector3.down;

        Debug.DrawRay(origin, direction * stepDistance, Color.blue);
        if (Physics.Raycast(origin, direction, out hitVert, stepDistance))
        {
            Debug.Log("Step Hit");
            result = true;
        }
        else
        {
            Debug.Log("No Step Hit");
        }

        return result;
    }

    //Detects grabbable ledge
    public bool FindLedge()
    {
        bool result = false;
        float mDHorizontal = 1;
        float mDVertical = 1;

        RaycastHit hitVert;
        RaycastHit hitHor;
        //Detects surface facing player
        Vector3 originHorizontal = this.transform.position;
        originHorizontal.y += 1f;
        Vector3 directionHorizontal = this.transform.forward;
        //Detects if the surface has a grabbable spot almost directly above the players head
        Vector3 originShortVertical = transform.position + (transform.forward * .3f);
        originShortVertical.y += 2f;
        Vector3 directionVertical = -this.transform.up;
        //Detects if the surface is a little further away in the horizontal direction
        Vector3 originLongVertical = transform.position + (transform.forward * .5f);
        originLongVertical.y += 2f;

        Debug.DrawRay(originHorizontal + transform.right * minDistance / 2, directionHorizontal * mDHorizontal, Color.red);
        Debug.DrawRay(originShortVertical + transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
        if (Physics.Raycast(originHorizontal + transform.right * minDistance / 2, directionHorizontal, out hitVert, mDHorizontal) &&
            Physics.Raycast(originShortVertical + transform.right * minDistance / 2, directionVertical, out hitHor, mDVertical))
        {
            Debug.Log("Ledge Hit");
            result = true;
        }
        else if (Physics.Raycast(originHorizontal - transform.right * minDistance / 2, directionHorizontal, out hitVert, mDHorizontal) &&
            Physics.Raycast(originShortVertical - transform.right * minDistance / 2, directionVertical, out hitHor, mDVertical))
        {
            Debug.DrawRay(originHorizontal - transform.right * minDistance / 2, directionHorizontal * mDHorizontal, Color.red);
            Debug.DrawRay(originShortVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.Log("Ledge Hit");
            result = true;
        }
        else if (Physics.Raycast(originHorizontal - transform.right * minDistance / 2, directionHorizontal, out hitVert, mDHorizontal) &&
            Physics.Raycast(originLongVertical - transform.right * minDistance / 2, directionVertical, out hitHor, mDVertical))
        {
            Debug.DrawRay(originHorizontal - transform.right * minDistance / 2, directionHorizontal * mDHorizontal, Color.red);
            Debug.DrawRay(originShortVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.DrawRay(originLongVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.Log("Ledge Hit");
            result = true;
        }
        else if (Physics.Raycast(originHorizontal + transform.right * minDistance / 2, directionHorizontal, out hitVert, mDHorizontal) &&
            Physics.Raycast(originLongVertical + transform.right * minDistance / 2, directionVertical, out hitHor, mDVertical))
        {
            Debug.DrawRay(originHorizontal - transform.right * minDistance / 2, directionHorizontal * mDHorizontal, Color.red);
            Debug.DrawRay(originShortVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.DrawRay(originLongVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.DrawRay(originLongVertical + transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.Log("Ledge Hit");
            result = true;
        }
        else
        {
            Debug.DrawRay(originHorizontal - transform.right * minDistance / 2, directionHorizontal * mDHorizontal, Color.red);
            Debug.DrawRay(originShortVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.DrawRay(originLongVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.DrawRay(originLongVertical + transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.Log("No Ledge Hit");
        }

        return result;
    }

    // Checks if can mantle ledge
    public bool Mantle()
    {
        float mantleDistance = .5f;

        Vector3 origin = transform.position + (transform.forward * .8f);
        origin.y += 1.5f;
        Vector3 direction = Vector3.down;

        Debug.DrawRay(origin + transform.right * minDistance / 2, direction * mantleDistance, Color.blue);
        if (Physics.Raycast(origin + transform.right * minDistance / 2, direction, out hitVert, mantleDistance))
        {
            Debug.Log("Mantle Hit");
            return true;
        }
        else if (Physics.Raycast(origin - transform.right * minDistance / 2, direction, out hitVert, mantleDistance))
        {
            Debug.DrawRay(origin - transform.right * minDistance / 2, direction * mantleDistance, Color.blue);
            Debug.Log("Mantle Hit");
            return true;
        }
        else
        {
            Debug.DrawRay(origin - transform.right * minDistance / 2, direction * mantleDistance, Color.blue);
            Debug.Log("No Mantle Hit");
        }

        return false;
    }

    public bool NextLedge()
    {


        return false;
    }
}
