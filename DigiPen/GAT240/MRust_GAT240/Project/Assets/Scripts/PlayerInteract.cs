﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{

    public struct LookingAt
    {
        public Transform playerCamera;
    }
    public struct Looking
    {
        public Transform playerCamera;
    }
    public struct Use
    {
        public Transform playerCamera;
    }
    public struct LookingAway
    {
        public Transform playerCamera;

    }

    public float maxInteractDistance = 10.0f;
    public float interactionRadius = 0.5f;

    public Camera interactorCamera;

    public Transform interactedObject;
    public Transform prevInteractedObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit hit;

        // If we hit something, send the use command
        if(LookRaycast(out hit))
        {
            // clicked on object
            if (Input.GetMouseButtonDown(0))
            {
                SendUse(hit.transform.gameObject);
            }

            // looking at new object
            if (hit.transform != interactedObject)
            {
                prevInteractedObject = interactedObject;
                interactedObject = hit.transform;

                // looking away from prev object
                if(prevInteractedObject != null)
                {
                    SendLookAway(prevInteractedObject.gameObject);
                }

                if(interactedObject != null)
                {
                    SendLookAt(interactedObject.gameObject);
                }
            }

            SendLooking(interactedObject.gameObject);
        }
        else
        {
            // looking away
            if(interactedObject != null)
            {
                SendLookAway(interactedObject.gameObject);
            }

            prevInteractedObject = interactedObject;
            interactedObject = null;
        }
    }
    void SendLookAt(GameObject go)
    {
        Debug.Log("PlayerInteract.SendLookAt");
        LookingAt lookAt = new LookingAt();
        lookAt.playerCamera = interactorCamera.transform;
        FFMessageBoard<LookingAt>.SendToLocalToAllConnected(lookAt, go);
    }
    void SendLookAway(GameObject go)
    {
        Debug.Log("PlayerInteract.SendLookAway");
        LookingAway lookAway = new LookingAway();
        lookAway.playerCamera = interactorCamera.transform;
        FFMessageBoard<LookingAway>.SendToLocalToAllConnected(lookAway, go);
    }
    void SendLooking(GameObject go)
    {
        Debug.Log("PlayerInteract.SendLooking");
        Looking looking = new Looking();
        looking.playerCamera = interactorCamera.transform;
        FFMessageBoard<Looking>.SendToLocalToAllConnected(looking, go);
    }
    void SendUse(GameObject go)
    {
        Debug.Log("PlayerInteract.SendUse");
        Use u = new Use();
        u.playerCamera = transform;
        FFMessageBoard<Use>.SendToLocalToAllConnected(u, go);
    }

    bool LookRaycast(out RaycastHit hit)
    {
        return Physics.SphereCast(
            interactorCamera.transform.position,
            interactionRadius,
            interactorCamera.transform.forward,
            out hit, maxInteractDistance);
    }
}
