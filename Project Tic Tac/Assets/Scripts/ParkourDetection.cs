using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourDetection : MonoBehaviour
{
    public RaycastHit hitVert;
    public RaycastHit hitHor;

    float minDistance = .25f;

    [SerializeField]
    public Transform hands;
    [SerializeField]
    Transform feet;
    [SerializeField]
    LayerMask layers;

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
        float distance = 1;

        //Detects surface facing player
        Vector3 originHorizontal = this.transform.position;
        originHorizontal.y += 1f;
        Vector3 directionHorizontal = this.transform.forward;
        //Detects if the surface has a grabbable spot almost directly above the players head
        Vector3 originShort = hands.position + (transform.forward * .3f);
        originShort.y += 1f;
        Vector3 directionVertical = -this.transform.up;
        //Detects if the surface is a little further away in the horizontal direction
        Vector3 originLong = hands.position + (transform.forward * .5f);
        originLong.y += 1f;

        Debug.DrawRay(originShort + transform.right * minDistance / 2, directionVertical * distance, Color.blue);
        if (Physics.Raycast(originShort + transform.right * minDistance / 2, directionVertical, out hitVert, distance, layers))
        {
            originShort = hands.position - (transform.forward * .05f);
            originShort.y = hitVert.point.y;
            originShort.y -= .01f;
            
            Debug.DrawRay(originShort + transform.right * minDistance / 2, directionHorizontal * distance, Color.red);
            if (Physics.Raycast(originShort + transform.right * minDistance / 2, directionHorizontal, out hitHor, distance, layers))
            {
                //Debug.Log("Ledge Hit");
                return true;
            }
            else
            {
                originShort = hands.position + (transform.forward * .3f);
                originShort.y += 1f;
            }
        }

        if(Physics.Raycast(originShort - transform.right * minDistance / 2, directionVertical, out hitVert, distance, layers))
        {
            originShort = hands.position - (transform.forward * .05f);
            originShort.y = hitVert.point.y;
            originShort.y -= .01f;

            Debug.DrawRay(originShort - transform.right * minDistance / 2, directionHorizontal * distance, Color.red);
            Debug.DrawRay(originShort - transform.right * minDistance / 2, directionVertical * distance, Color.blue);
            Debug.DrawRay(originShort + transform.right * minDistance / 2, directionHorizontal * distance, Color.red);
            if (Physics.Raycast(originShort - transform.right * minDistance / 2, directionHorizontal, out hitHor, distance, layers))
            {
                //Debug.Log("Ledge Hit");
                return true;
            }
            else
            {
                originShort = hands.position + (transform.forward * .3f);
                originShort.y += 1f;
            }
        }

        if(Physics.Raycast(originLong - transform.right * minDistance / 2, directionVertical, out hitVert, distance, layers))
        {
            originLong = hands.position - (transform.forward * .05f);
            originLong.y = hitVert.point.y;
            originLong.y -= .01f;

            Debug.DrawRay(originShort - transform.right * minDistance / 2, directionHorizontal * distance, Color.red);
            Debug.DrawRay(originShort - transform.right * minDistance / 2, directionVertical * distance, Color.blue);
            Debug.DrawRay(originShort + transform.right * minDistance / 2, directionHorizontal * distance, Color.red);
            Debug.DrawRay(originLong - transform.right * minDistance / 2, directionVertical * distance, Color.blue);
            if (Physics.Raycast(originLong - transform.right * minDistance / 2, directionHorizontal, out hitHor, distance, layers))
            {
                //Debug.Log("Ledge Hit");
                return true;
            }
            else
            {
                originLong = hands.position + (transform.forward * .5f);
                originLong.y += 1f;
            }
        }

        if(Physics.Raycast(originLong + transform.right * minDistance / 2, directionVertical, out hitVert, distance, layers))
        {
            originLong = hands.position - (transform.forward * .05f);
            originLong.y = hitVert.point.y;
            originLong.y -= .01f;

            Debug.DrawRay(originShort - transform.right * minDistance / 2, directionHorizontal * distance, Color.red);
            Debug.DrawRay(originShort - transform.right * minDistance / 2, directionVertical * distance, Color.blue);
            Debug.DrawRay(originShort + transform.right * minDistance / 2, directionHorizontal * distance, Color.red);
            Debug.DrawRay(originLong - transform.right * minDistance / 2, directionVertical * distance, Color.blue);
            Debug.DrawRay(originLong + transform.right * minDistance / 2, directionVertical * distance, Color.blue);
            if (Physics.Raycast(originLong - transform.right * minDistance / 2, directionHorizontal, out hitHor, distance, layers))
            {
                //Debug.Log("Ledge Hit");
                return true;
            }
        }
        else
        {
            Debug.DrawRay(originShort - transform.right * minDistance / 2, directionVertical * distance, Color.blue);
            Debug.DrawRay(originLong - transform.right * minDistance / 2, directionVertical * distance, Color.blue);
            Debug.DrawRay(originLong + transform.right * minDistance / 2, directionVertical * distance, Color.blue);
            //Debug.Log("No Ledge Hit");
        }
        return false;
    }

    // Checks if can mantle ledge
    public bool Mantle()
    {
        float mantleDistance = .5f;

        Vector3 origin = transform.position + (transform.forward * .8f);
        origin.y += 1.5f;
        Vector3 direction = Vector3.down;

        Debug.DrawRay(origin + transform.right * minDistance / 2, direction * mantleDistance, Color.blue);
        if (Physics.Raycast(origin + transform.right * minDistance / 2, direction, out hitVert, mantleDistance, layers))
        {
            Debug.Log("Mantle Hit");
            return true;
        }
        else if (Physics.Raycast(origin - transform.right * minDistance / 2, direction, out hitVert, mantleDistance, layers))
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
        if (Physics.Raycast(origin - (hands.forward * .2f) - (hands.up * .05f), direction, out hitHor, wallDistance, layers))
        {
            Debug.Log("Wall Detected");

            return true;
        }
        else if (Physics.Raycast(origin - (hands.forward * .3f) - (hands.up * .05f), direction, out hitHor, wallDistance, layers))
        {
            Debug.DrawRay(origin - (hands.forward * .3f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.Log("Wall Detected");

            return true;
        }
        else if (Physics.Raycast(origin - (hands.forward * .2f) + (hands.up * .05f), direction, out hitHor, wallDistance, layers))
        {
            Debug.DrawRay(origin - (hands.forward * .2f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.Log("Wall Detected");

            return true;
        }
        else if (Physics.Raycast(origin - (hands.forward * .3f) + (hands.up * .05f), direction, out hitHor, wallDistance, layers))
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
        if (Physics.Raycast(origin - (hands.forward * .2f) - (hands.up * .05f), direction, out hitHor, wallDistance, layers))
        {
            Debug.Log("Wall Detected");

            return true;
        }
        else if (Physics.Raycast(origin - (hands.forward * .3f) - (hands.up * .05f), direction, out hitHor, wallDistance, layers))
        {
            Debug.DrawRay(origin - (hands.forward * .3f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.Log("Wall Detected");

            return true;
        }
        else if (Physics.Raycast(origin - (hands.forward * .2f) + (hands.up * .05f), direction, out hitHor, wallDistance, layers))
        {
            Debug.DrawRay(origin - (hands.forward * .2f) - (hands.up * .05f), direction * wallDistance, Color.cyan);
            Debug.Log("Wall Detected");

            return true;
        }
        else if (Physics.Raycast(origin - (hands.forward * .3f) + (hands.up * .05f), direction, out hitHor, wallDistance, layers))
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

    public bool WallCheckFront()
    {
        float distance = .5f;

        Vector3 origin = hands.position + (transform.forward * .05f);
        origin.y += .5f;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin + (transform.right * minDistance), direction * distance, Color.red);
        if(Physics.Raycast(origin + (transform.right * minDistance), direction, distance, layers))
        {
            return true;
        }
        else if(Physics.Raycast(origin - (transform.right * minDistance), direction, distance, layers))
        {
            Debug.DrawRay(origin - (transform.right * minDistance), direction * distance, Color.red);
            return true;
        }
        else
        {
            Debug.DrawRay(origin + (transform.right * minDistance), direction * distance, Color.red);
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
        if (Physics.Raycast(origin + transform.right * minDistance / 2, direction, out hitVert, ledgeDistance, layers))
        {
            origin = hands.position - (transform.forward * .05f);
            origin.y = hitVert.point.y;
            origin.y -= .01f;
            direction = transform.forward;
            Debug.DrawRay(origin + transform.right * minDistance / 2, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin + transform.right * minDistance / 2, direction, out hitHor, ledgeDistance, layers))
            {

                Debug.Log("Ledge Up Hit");
                return true;
            }
            else if (Physics.Raycast(origin - transform.right * minDistance / 2, direction, out hitVert, ledgeDistance, layers))
            {
                Debug.DrawRay(origin - transform.right * minDistance / 2, direction * ledgeDistance, Color.blue);
                origin = hands.position - (transform.forward * .05f);
                origin.y = hitVert.point.y;
                origin.y -= .01f;
                direction = transform.forward;
                Debug.DrawRay(origin - transform.right * minDistance / 2, direction * ledgeDistance, Color.red);
                if (Physics.Raycast(origin - transform.right * minDistance / 2, direction, out hitHor, ledgeDistance, layers))
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
        else if (Physics.Raycast(origin - transform.right * minDistance / 2, direction, out hitVert, ledgeDistance, layers))
        {
            Debug.DrawRay(origin - transform.right * minDistance / 2, direction * ledgeDistance, Color.blue);
            origin = hands.position - (transform.forward * .05f);
            origin.y = hitVert.point.y;
            origin.y -= .01f;
            direction = transform.forward;
            Debug.DrawRay(origin - transform.right * minDistance / 2, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin - transform.right * minDistance / 2, direction, out hitHor, ledgeDistance, layers))
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
        if (Physics.Raycast(origin - (transform.right * .5f), direction, out hitVert, ledgeDistance, layers))
        {
            // Shoots horizontal Ray to get X and Z coordinates
            origin = hands.position - (transform.forward * .05f);
            origin.y = hitVert.point.y;
            origin.y -= .01f;
            direction = transform.forward;

            Debug.DrawRay(origin - (transform.right * .5f), direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin - (transform.right * .5f), direction, out hitHor, ledgeDistance, layers))
            {

                Debug.Log("Ledge Left Hit");
                return true;
            }
            else if (Physics.Raycast(origin - (transform.right * .2f), direction, out hitVert, ledgeDistance, layers))
            {
                Debug.DrawRay(origin - (transform.right * .2f), direction * ledgeDistance, Color.blue);
                origin = hands.position - (transform.forward * .05f);
                origin.y = hitVert.point.y;
                origin.y -= .01f;
                direction = transform.forward;
                Debug.DrawRay(origin - (transform.right * .2f), direction * ledgeDistance, Color.red);
                if (Physics.Raycast(origin - (transform.right * .2f), direction, out hitHor, ledgeDistance, layers))
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
        else if (Physics.Raycast(origin - (transform.right * .2f), direction, out hitVert, ledgeDistance, layers))
        {
            Debug.DrawRay(origin - (transform.right * .2f), direction * ledgeDistance, Color.blue);
            origin = hands.position - (transform.forward * .05f);
            origin.y = hitVert.point.y;
            origin.y -= .01f;
            direction = transform.forward;
            Debug.DrawRay(origin - (transform.right * .2f), direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin - (transform.right * .2f), direction, out hitHor, ledgeDistance, layers))
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
        if (Physics.Raycast(origin + (transform.right * .5f), direction, out hitVert, ledgeDistance, layers))
        {
            // Shoots horizontal Ray to get X and Z coordinates
            origin = hands.position - (transform.forward * .05f);
            origin.y = hitVert.point.y;
            origin.y -= .01f;
            
            direction = transform.forward;
            Debug.DrawRay(origin + (transform.right * .5f), direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin + (transform.right * .5f), direction, out hitHor, ledgeDistance, layers))
            {

                Debug.Log("Ledge Left Hit");
                return true;
            }
            else if (Physics.Raycast(origin + (transform.right * .2f), direction, out hitVert, ledgeDistance, layers))
            {
                Debug.DrawRay(origin + (transform.right * .2f), direction * ledgeDistance, Color.blue);
                origin = hands.position - (transform.forward * .05f);
                origin.y = hitVert.point.y;
                origin.y -= .01f;
                direction = transform.forward;
                Debug.DrawRay(origin + (transform.right * .2f), direction * ledgeDistance, Color.red);
                if (Physics.Raycast(origin + (transform.right * .2f), direction, out hitHor, ledgeDistance, layers))
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
        else if (Physics.Raycast(origin + (transform.right * .2f), direction, out hitVert, ledgeDistance, layers))
        {
            Debug.DrawRay(origin + (transform.right * .2f), direction * ledgeDistance, Color.blue);
            origin = hands.position - (transform.forward * .05f);
            origin.y = hitVert.point.y;
            origin.y -= .01f;
            direction = transform.forward;
            Debug.DrawRay(origin + (transform.right * .2f), direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin + (transform.right * .2f), direction, out hitHor, ledgeDistance, layers))
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
        if (Physics.Raycast(origin - (transform.forward * .2f) + (transform.right * .4f), direction, out hitVert, ledgeDistance, layers))
        {
            origin = hands.position - (transform.forward * .2f);
            origin.y = hitVert.point.y - .01f;
            direction = transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance, layers))
            {
                Debug.Log("Corner Found");

                return true;
            }
        }
        else if (Physics.Raycast(origin - (transform.forward * .3f) + (transform.right * .4f), direction, out hitVert, ledgeDistance, layers))
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
        else if (Physics.Raycast(origin + (transform.forward * .25f) + (transform.right * .1f), direction, out hitVert, ledgeDistance, layers))
        {
            Debug.DrawRay(origin + (transform.forward * .25f) + (transform.right * .1f), direction * ledgeDistance, Color.blue);
            origin = hands.position + (transform.forward * .25f) + (transform.right * .5f);
            origin.y = hitVert.point.y - .01f;
            direction = -transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance, layers))
            {
                Debug.Log("Forward Corner Found");

                return true;
            }
        }
        else if (Physics.Raycast(origin + (transform.forward * .25f), direction, out hitVert, ledgeDistance, layers))
        {
            Debug.DrawRay(origin + (transform.forward * .25f), direction * ledgeDistance, Color.blue);
            origin = hands.position + (transform.forward * .25f) + (transform.right * .5f);
            origin.y = hitVert.point.y - .01f;
            direction = -transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance, layers))
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
        if (Physics.Raycast(origin - (transform.forward * .2f) - (transform.right * .4f), direction, out hitVert, ledgeDistance, layers))
        {
            origin = hands.position - (transform.forward * .2f);
            origin.y = hitVert.point.y - .01f;
            direction = -transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance, layers))
            {
                Debug.Log("Corner Found");

                return true;
            }
        }
        else if (Physics.Raycast(origin - (transform.forward * .3f) - (transform.right * .4f), direction, out hitVert, ledgeDistance, layers))
        {
            Debug.DrawRay(origin - (transform.forward * .3f) - (transform.right * .4f), direction * ledgeDistance, Color.blue);
            origin = hands.position - (transform.forward * .3f);
            origin.y = hitVert.point.y - .01f;
            direction = -transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance, layers))
            {
                Debug.Log("Corner Found");

                return true;
            }
        }
        else if (Physics.Raycast(origin + (transform.forward * .25f) - (transform.right * .1f), direction, out hitVert, ledgeDistance, layers))
        {
            Debug.DrawRay(origin - (transform.forward * .3f) - (transform.right * .4f), direction * ledgeDistance, Color.blue);
            Debug.DrawRay(origin + (transform.forward * .25f) - (transform.right * .1f), direction * ledgeDistance, Color.blue);
            origin = hands.position + (transform.forward * .25f) - (transform.right * .5f);
            origin.y = hitVert.point.y - .01f;
            direction = transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance, layers))
            {
                Debug.Log("Forward Corner Found");

                return true;
            }
        }
        else if (Physics.Raycast(origin + (transform.forward * .25f), direction, out hitVert, ledgeDistance, layers))
        {
            Debug.DrawRay(origin + (transform.forward * .25f), direction * ledgeDistance, Color.blue);
            Debug.DrawRay(origin - (transform.forward * .3f) - (transform.right * .4f), direction * ledgeDistance, Color.blue);
            Debug.DrawRay(origin + (transform.forward * .25f) - (transform.right * .1f), direction * ledgeDistance, Color.blue);
            origin = hands.position + (transform.forward * .25f) - (transform.right * .5f);
            origin.y = hitVert.point.y - .01f;
            direction = transform.right;

            Debug.DrawRay(origin, direction * ledgeDistance, Color.red);
            if (Physics.Raycast(origin, direction, out hitHor, ledgeDistance, layers))
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

    // This will determine whether the player is hanging or braced on a ledge
    public bool LedgeTypeCheck()
    {
        float distance = .6f;

        // Amount the rays origin is adjusted horizontally
        Vector3 adjust = (transform.right * .1f);

        Vector3 origin = feet.position - (transform.forward * .2f);
        Vector3 direction = feet.forward;
        Debug.DrawRay(origin + adjust, direction * distance, Color.red);
        if (Physics.Raycast(origin + adjust, direction, distance, layers))
        {
            Debug.DrawRay(origin - adjust, direction * distance, Color.red);
            if (Physics.Raycast(origin - adjust, direction, distance, layers))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            Debug.DrawRay(origin - adjust, direction * distance, Color.red);
            return false;
        }
    }
}
