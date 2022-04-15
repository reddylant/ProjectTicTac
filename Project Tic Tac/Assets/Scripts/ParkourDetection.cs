using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourDetection : MonoBehaviour
{
    public RaycastHit hitVert;
    public RaycastHit hitHor;

    float minDistance = .25f;

    [SerializeField]
    Transform hands;

    private void Start()
    {
        hands = GameObject.FindGameObjectWithTag("Player").transform.Find("Hands");
    }

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
        if (Physics.Raycast(originHorizontal + transform.right * minDistance / 2, directionHorizontal, out hitHor, mDHorizontal) &&
            Physics.Raycast(originShortVertical + transform.right * minDistance / 2, directionVertical, out hitVert, mDVertical))
        {
            //Debug.Log("Ledge Hit");
            result = true;
        }
        else if (Physics.Raycast(originHorizontal - transform.right * minDistance / 2, directionHorizontal, out hitHor, mDHorizontal) &&
            Physics.Raycast(originShortVertical - transform.right * minDistance / 2, directionVertical, out hitVert, mDVertical))
        {
            Debug.DrawRay(originHorizontal - transform.right * minDistance / 2, directionHorizontal * mDHorizontal, Color.red);
            Debug.DrawRay(originShortVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            //Debug.Log("Ledge Hit");
            result = true;
        }
        else if (Physics.Raycast(originHorizontal - transform.right * minDistance / 2, directionHorizontal, out hitHor, mDHorizontal) &&
            Physics.Raycast(originLongVertical - transform.right * minDistance / 2, directionVertical, out hitVert, mDVertical))
        {
            Debug.DrawRay(originHorizontal - transform.right * minDistance / 2, directionHorizontal * mDHorizontal, Color.red);
            Debug.DrawRay(originShortVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.DrawRay(originLongVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            //Debug.Log("Ledge Hit");
            result = true;
        }
        else if (Physics.Raycast(originHorizontal + transform.right * minDistance / 2, directionHorizontal, out hitHor, mDHorizontal) &&
            Physics.Raycast(originLongVertical + transform.right * minDistance / 2, directionVertical, out hitVert, mDVertical))
        {
            Debug.DrawRay(originHorizontal - transform.right * minDistance / 2, directionHorizontal * mDHorizontal, Color.red);
            Debug.DrawRay(originShortVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.DrawRay(originLongVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.DrawRay(originLongVertical + transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            //Debug.Log("Ledge Hit");
            result = true;
        }
        else
        {
            Debug.DrawRay(originHorizontal - transform.right * minDistance / 2, directionHorizontal * mDHorizontal, Color.red);
            Debug.DrawRay(originShortVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.DrawRay(originLongVertical - transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            Debug.DrawRay(originLongVertical + transform.right * minDistance / 2, directionVertical * mDVertical, Color.blue);
            //Debug.Log("No Ledge Hit");
        }

        if (result)
        {
            Debug.Log("X: " + hitHor.point.x + "Y: " + hitVert.point.y + "Z: " + hitHor.point.z);
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

    public bool WallCheckLeft()
    {
        float wallDistance = .35f;

        Vector3 origin = hands.position;
        Vector3 direction = -transform.right;
        Debug.DrawRay(origin - (hands.forward * .2f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
        if (Physics.Raycast(origin - (hands.forward * .2f) - (hands.up * .05f), direction, out hitHor, wallDistance, 3))
        {
            Debug.Log("Wall Detected");

            return true;
        }
        else if (Physics.Raycast(origin - (hands.forward * .3f) - (hands.up * .05f), direction, out hitHor, wallDistance, 3))
        {
            Debug.DrawRay(origin - (hands.forward * .3f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.Log("Wall Detected");

            return true;
        }
        else if (Physics.Raycast(origin - (hands.forward * .2f) + (hands.up * .05f), direction, out hitHor, wallDistance, 3))
        {
            Debug.DrawRay(origin - (hands.forward * .2f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.Log("Wall Detected");

            return true;
        }
        else if (Physics.Raycast(origin - (hands.forward * .3f) + (hands.up * .05f), direction, out hitHor, wallDistance, 3))
        {
            Debug.Log("Wall Detected");

            return true;
        }
        else
        {
            Debug.DrawRay(origin - (hands.forward * .3f) + (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.DrawRay(origin - (hands.forward * .3f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.DrawRay(origin - (hands.forward * .2f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.Log("No Wall");
            return false;
        }
    }

    public bool WallCheckRight()
    {
        float wallDistance = .35f;

        Vector3 origin = hands.position;
        Vector3 direction = transform.right;
        Debug.DrawRay(origin - (hands.forward * .2f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
        if (Physics.Raycast(origin - (hands.forward * .2f) - (hands.up * .05f), direction, out hitHor, wallDistance, 3))
        {
            Debug.Log("Wall Detected");

            return true;
        }
        else if (Physics.Raycast(origin - (hands.forward * .3f) - (hands.up * .05f), direction, out hitHor, wallDistance, 3))
        {
            Debug.DrawRay(origin - (hands.forward * .3f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.Log("Wall Detected");

            return true;
        }
        else if (Physics.Raycast(origin - (hands.forward * .2f) + (hands.up * .05f), direction, out hitHor, wallDistance, 3))
        {
            Debug.DrawRay(origin - (hands.forward * .2f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.Log("Wall Detected");

            return true;
        }
        else if (Physics.Raycast(origin - (hands.forward * .3f) + (hands.up * .05f), direction, out hitHor, wallDistance, 3))
        {
            Debug.Log("Wall Detected");

            return true;
        }
        else
        {
            Debug.DrawRay(origin - (hands.forward * .3f) + (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.DrawRay(origin - (hands.forward * .3f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.DrawRay(origin - (hands.forward * .2f) + (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.Log("No Wall");
            return false;
        }
    }

    //Checks if there's a grabbable ledge above the player
    public bool LedgeUp()
    {
        float ledgeDistance = .5f;

        //Checks for upward facing surface
        Vector3 origin = hands.position + (transform.forward * .05f);
        origin.y += .8f;
        Vector3 direction = Vector3.down;

        Debug.DrawRay(origin + transform.right * minDistance / 2, direction * ledgeDistance, Color.blue);
        if (Physics.Raycast(origin + transform.right * minDistance / 2, direction, out hitVert, ledgeDistance))
        {
            origin = hands.position - (transform.forward * .05f);
            origin.y = hitVert.point.y;
            origin.y -= .01f;
            direction = transform.forward;
            Debug.DrawRay(origin + transform.right * minDistance / 2, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin + transform.right * minDistance / 2, direction, out hitHor, ledgeDistance))
            {

                Debug.Log("Ledge Up Hit");
                return true;
            }
            else if (Physics.Raycast(origin - transform.right * minDistance / 2, direction, out hitVert, ledgeDistance))
            {
                Debug.DrawRay(origin - transform.right * minDistance / 2, direction * ledgeDistance, Color.blue);
                origin = hands.position - (transform.forward * .05f);
                origin.y = hitVert.point.y;
                origin.y -= .01f;
                direction = transform.forward;
                Debug.DrawRay(origin - transform.right * minDistance / 2, direction * ledgeDistance, Color.red);
                if (Physics.Raycast(origin - transform.right * minDistance / 2, direction, out hitHor, ledgeDistance))
                {

                    Debug.Log("Ledge Up Hit");
                    return true;
                }
            }
            else
            {
                return false;
            }

        }
        else if (Physics.Raycast(origin - transform.right * minDistance / 2, direction, out hitVert, ledgeDistance))
        {
            Debug.DrawRay(origin - transform.right * minDistance / 2, direction * ledgeDistance, Color.blue);
            origin = hands.position - (transform.forward * .05f);
            origin.y = hitVert.point.y;
            origin.y -= .01f;
            direction = transform.forward;
            Debug.DrawRay(origin - transform.right * minDistance / 2, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin - transform.right * minDistance / 2, direction, out hitHor, ledgeDistance))
            {

                Debug.Log("Ledge Up Hit");
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.DrawRay(origin - transform.right * minDistance / 2, direction * ledgeDistance, Color.blue);

            Debug.Log("No Above Ledge");
        }

        return false;
    }

    public bool LedgeLeft()
    {
        float ledgeDistance = .5f;

        // Max distance the player can move in one animation
        Vector3 origin = hands.position + (transform.forward * .05f);
        origin.y += .2f;
        Vector3 direction = Vector3.down;

        Debug.DrawRay(origin - (transform.right * .5f), direction * ledgeDistance, Color.blue);
        if (Physics.Raycast(origin - (transform.right * .5f), direction, out hitVert, ledgeDistance))
        {
            // Shoots horizontal Ray to get X and Z coordinates
            origin = hands.position - (transform.forward * .05f);
            origin.y = hitVert.point.y;
            origin.y -= .01f;
            direction = transform.forward;

            Debug.DrawRay(origin - (transform.right * .5f), direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin - (transform.right * .5f), direction, out hitHor, ledgeDistance))
            {

                Debug.Log("Ledge Left Hit");
                return true;
            }
            else if (Physics.Raycast(origin - (transform.right * .2f), direction, out hitVert, ledgeDistance))
            {
                Debug.DrawRay(origin - (transform.right * .2f), direction * ledgeDistance, Color.blue);
                origin = hands.position - (transform.forward * .05f);
                origin.y = hitVert.point.y;
                origin.y -= .01f;
                direction = transform.forward;
                Debug.DrawRay(origin - (transform.right * .2f), direction * ledgeDistance, Color.red);
                if (Physics.Raycast(origin - (transform.right * .2f), direction, out hitHor, ledgeDistance))
                {
                    Debug.Log("Ledge Left Hit");
                    return true;
                }
                else
                {
                    Debug.Log("No Left Hit");
                    return false;
                }

            }
            else
            {
                Debug.Log("No Left Hit");
                return false;
            }

        }
        else if (Physics.Raycast(origin - (transform.right * .2f), direction, out hitVert, ledgeDistance))
        {
            Debug.DrawRay(origin - (transform.right * .2f), direction * ledgeDistance, Color.blue);
            origin = hands.position - (transform.forward * .05f);
            origin.y = hitVert.point.y;
            origin.y -= .01f;
            direction = transform.forward;
            Debug.DrawRay(origin - (transform.right * .2f), direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin - (transform.right * .2f), direction, out hitHor, ledgeDistance))
            {
                Debug.Log("Ledge Left Hit");
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            Debug.DrawRay(origin - (transform.right * .2f), direction * ledgeDistance, Color.blue);
            Debug.Log("No Left Hit");

            return false;
        }
        
    }

    public bool LedgeRight()
    {
        float ledgeDistance = .5f;

        // Max distance the player can move in one animation
        Vector3 origin = hands.position + (transform.forward * .05f);
        origin.y += .2f;
        Vector3 direction = Vector3.down;

        Debug.DrawRay(origin + (transform.right * .5f), direction * ledgeDistance, Color.blue);
        if (Physics.Raycast(origin + (transform.right * .5f), direction, out hitVert, ledgeDistance))
        {
            // Shoots horizontal Ray to get X and Z coordinates
            origin = hands.position - (transform.forward * .05f);
            origin.y = hitVert.point.y;
            origin.y -= .01f;
            
            direction = transform.forward;
            Debug.DrawRay(origin + (transform.right * .5f), direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin + (transform.right * .5f), direction, out hitHor, ledgeDistance))
            {

                Debug.Log("Ledge Left Hit");
                return true;
            }
            else if (Physics.Raycast(origin + (transform.right * .2f), direction, out hitVert, ledgeDistance))
            {
                Debug.DrawRay(origin + (transform.right * .2f), direction * ledgeDistance, Color.blue);
                origin = hands.position - (transform.forward * .05f);
                origin.y = hitVert.point.y;
                origin.y -= .01f;
                direction = transform.forward;
                Debug.DrawRay(origin + (transform.right * .2f), direction * ledgeDistance, Color.red);
                if (Physics.Raycast(origin + (transform.right * .2f), direction, out hitHor, ledgeDistance))
                {
                    Debug.Log("Ledge Left Hit");
                    return true;
                }
                else
                {
                    Debug.Log("No Left Hit");
                    return false;
                }

            }
            else
            {
                Debug.Log("No Left Hit");
                return false;
            }

        }
        else if (Physics.Raycast(origin + (transform.right * .2f), direction, out hitVert, ledgeDistance))
        {
            Debug.DrawRay(origin + (transform.right * .2f), direction * ledgeDistance, Color.blue);
            origin = hands.position - (transform.forward * .05f);
            origin.y = hitVert.point.y;
            origin.y -= .01f;
            direction = transform.forward;
            Debug.DrawRay(origin + (transform.right * .2f), direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin + (transform.right * .2f), direction, out hitHor, ledgeDistance))
            {
                Debug.Log("Ledge Left Hit");
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            Debug.DrawRay(origin + (transform.right * .2f), direction * ledgeDistance, Color.blue);
            Debug.Log("No Left Hit");

            return false;
        }

    }

    public bool LedgeCornerRight()
    {
        float ledgeDistance = .5f;

        Vector3 origin = hands.position;
        origin.y += .2f;
        Vector3 direction = Vector3.down;

        Debug.DrawRay(origin - (transform.forward * .2f) + (transform.right * .4f), direction * ledgeDistance, Color.blue);
        if (Physics.Raycast(origin - (transform.forward * .2f) + (transform.right * .4f), direction, out hitVert, ledgeDistance, 3))
        {
            origin = hands.position - (transform.forward * .2f);
            origin.y = hitVert.point.y - .01f;
            direction = transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance))
            {
                Debug.Log("Corner Found");

                return true;
            }
        }
        else if (Physics.Raycast(origin - (transform.forward * .3f) + (transform.right * .4f), direction, out hitVert, ledgeDistance, 3))
        {
            Debug.DrawRay(origin - (transform.forward * .3f) + (transform.right * .4f), direction * ledgeDistance, Color.blue);
            origin = hands.position - (transform.forward * .3f);
            origin.y = hitVert.point.y - .01f;
            direction = transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance))
            {
                Debug.Log("Corner Found");

                return true;
            }
        }
        else if (Physics.Raycast(origin + (transform.forward * .25f) + (transform.right * .1f), direction, out hitVert, ledgeDistance))
        {
            Debug.DrawRay(origin + (transform.forward * .25f) + (transform.right * .1f), direction * ledgeDistance, Color.blue);
            origin = hands.position + (transform.forward * .25f) + (transform.right * .5f);
            origin.y = hitVert.point.y - .01f;
            direction = -transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance, 3))
            {
                Debug.Log("Forward Corner Found");

                return true;
            }
        }
        else if (Physics.Raycast(origin + (transform.forward * .25f), direction, out hitVert, ledgeDistance))
        {
            Debug.DrawRay(origin + (transform.forward * .25f), direction * ledgeDistance, Color.blue);
            origin = hands.position + (transform.forward * .25f) + (transform.right * .5f);
            origin.y = hitVert.point.y - .01f;
            direction = -transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance, 3))
            {
                Debug.Log("Forward Corner Found");

                return true;
            }
        }
        else
        {
            Debug.DrawRay(origin - (transform.forward * .3f) + (transform.right * .4f), direction * ledgeDistance, Color.blue);
            Debug.DrawRay(origin + (transform.forward * .25f) + (transform.right * .1f), direction * ledgeDistance, Color.blue);
            Debug.DrawRay(origin + (transform.forward * .25f), direction * ledgeDistance, Color.blue);
            
            return false;
        }

        return false;
    }
    public bool LedgeCornerLeft()
    {
        float ledgeDistance = .5f;

        Vector3 origin = hands.position;
        origin.y += .2f;
        Vector3 direction = Vector3.down;

        Debug.DrawRay(origin - (transform.forward * .2f) - (transform.right * .4f), direction * ledgeDistance, Color.blue);
        if (Physics.Raycast(origin - (transform.forward * .2f) - (transform.right * .4f), direction, out hitVert, ledgeDistance, 3))
        {
            origin = hands.position - (transform.forward * .2f);
            origin.y = hitVert.point.y - .01f;
            direction = -transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance))
            {
                Debug.Log("Corner Found");

                return true;
            }
        }
        else if (Physics.Raycast(origin - (transform.forward * .3f) - (transform.right * .4f), direction, out hitVert, ledgeDistance, 3))
        {
            Debug.DrawRay(origin - (transform.forward * .3f) - (transform.right * .4f), direction * ledgeDistance, Color.blue);
            origin = hands.position - (transform.forward * .3f);
            origin.y = hitVert.point.y - .01f;
            direction = -transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance))
            {
                Debug.Log("Corner Found");

                return true;
            }
        }
        else if (Physics.Raycast(origin + (transform.forward * .25f) - (transform.right * .1f), direction, out hitVert, ledgeDistance))
        {
            Debug.DrawRay(origin - (transform.forward * .3f) - (transform.right * .4f), direction * ledgeDistance, Color.blue);
            Debug.DrawRay(origin + (transform.forward * .25f) - (transform.right * .1f), direction * ledgeDistance, Color.blue);
            origin = hands.position + (transform.forward * .25f) - (transform.right * .5f);
            origin.y = hitVert.point.y - .01f;
            direction = transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance, 3))
            {
                Debug.Log("Forward Corner Found");

                return true;
            }
        }
        else if (Physics.Raycast(origin + (transform.forward * .25f), direction, out hitVert, ledgeDistance))
        {
            Debug.DrawRay(origin + (transform.forward * .25f), direction * ledgeDistance, Color.blue);
            Debug.DrawRay(origin - (transform.forward * .3f) - (transform.right * .4f), direction * ledgeDistance, Color.blue);
            Debug.DrawRay(origin + (transform.forward * .25f) - (transform.right * .1f), direction * ledgeDistance, Color.blue);
            origin = hands.position + (transform.forward * .25f) - (transform.right * .5f);
            origin.y = hitVert.point.y - .01f;
            direction = transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance, 3))
            {
                Debug.Log("Forward Corner Found");

                return true;
            }
        }
        else
        {
            Debug.DrawRay(origin - (transform.forward * .3f) - (transform.right * .4f), direction * ledgeDistance, Color.blue);
            Debug.DrawRay(origin + (transform.forward * .25f) - (transform.right * .1f), direction * ledgeDistance, Color.blue);
            Debug.DrawRay(origin + (transform.forward * .25f), direction * ledgeDistance, Color.blue);

            return false;
        }

        return false;
    }
}
