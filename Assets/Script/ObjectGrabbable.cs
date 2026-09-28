using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;

        objectRigidbody.useGravity = false;
        objectRigidbody.isKinematic = true;

        objectRigidbody.linearVelocity = Vector3.zero;
        objectRigidbody.angularVelocity = Vector3.zero;
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;

        objectRigidbody.isKinematic = false;
        objectRigidbody.useGravity = true;

        objectRigidbody.linearVelocity = Vector3.zero;
        objectRigidbody.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(
                transform.position,
                objectGrabPointTransform.position,
                Time.fixedDeltaTime * lerpSpeed
            );

            objectRigidbody.MovePosition(newPosition);
        }
    }
}