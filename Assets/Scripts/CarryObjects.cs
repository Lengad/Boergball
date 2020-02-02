using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Input = UnityEngine.Input;

namespace Assets.Scripts
{

    // Credits to u/nandos13 for his script https://www.reddit.com/r/Unity3D/comments/7nul40/simple_gravity_gun_script_well_commented/
    public class CarryObjects : NetworkBehaviour
    {
        private const float MaxPickupDistance = 5f;
        private RaycastHit hit;
        private Rigidbody rigidbodyOfObject;

        [SerializeField] private float throwForce = 80f;

        /// <summary>The offset vector from the object's position to hit point, in local space</summary>
        [SyncVar] private Vector3 hitOffsetLocal;

        /// <summary>The distance we are holding the object at</summary>
        [SyncVar] private float currentPickUpDistance;

        /// <summary>The interpolation state when first grabbed</summary>
        private RigidbodyInterpolation initialInterpolationSetting;

        /// <summary>The difference between player & object rotation, updated when picked up or when rotated by the player</summary>
        [SyncVar] private Vector3 rotationDifferenceEuler;

        /// <summary>Tracks player input to rotate current object. Used and reset every fixedupdate call</summary>
        [SyncVar] private Vector2 rotationInput;

        [SyncVar] private bool isThrowing;
        private CameraFollow cameraFollow;

        void Start()
        {
            cameraFollow = Camera.main.GetComponentInParent<CameraFollow>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!isLocalPlayer)
                return;

            if (!Input.GetMouseButton((int)MouseButton.LeftMouse))
            {
                // We are not holding the mouse button. Release the object and return before checking for a new one
                if (rigidbodyOfObject != null)
                    Drop();

                return;
            }

            if (rigidbodyOfObject == null)
            {
                // We are not holding an object, look for one to pick up

                Debug.DrawLine(transform.position, transform.position + cameraFollow.transform.forward * MaxPickupDistance, Color.yellow);
                if (Physics.Raycast(transform.position, cameraFollow.transform.forward, out hit, MaxPickupDistance))
                {
                    // Don't pick up kinematic rigidbodies (they can't move)
                    if (hit.rigidbody != null && !hit.rigidbody.isKinematic)
                        PickUp();
                }
            }
            else
            {
                // We are already holding an object, listen for rotation input
                if (Input.GetKey(KeyCode.R))
                    rotationInput += new Vector2(rotationInput.x, 1);
                if (Input.GetKey(KeyCode.F))
                    rotationInput -= new Vector2(rotationInput.x, 1);
                if (Input.GetKey(KeyCode.Q))
                    rotationInput += new Vector2(1, rotationInput.y);
                if (Input.GetKey(KeyCode.E))
                    rotationInput -= new Vector2(1, rotationInput.y);
                if (Input.GetMouseButtonDown((int) MouseButton.RightMouse))
                    isThrowing = true;
            }
        }

        void FixedUpdate()
        {
            if (!isLocalPlayer)
                return;

            if (rigidbodyOfObject)
            {
                // Get the destination point for the point on the object we grabbed
                Vector3 holdPoint = transform.position + cameraFollow.transform.forward * currentPickUpDistance;
                Debug.DrawLine(transform.position, holdPoint, Color.blue, Time.fixedDeltaTime);

                // We are holding an object, time to rotate & move it
                // Call the method on the server
                CmdMoveAndRotate(holdPoint, rigidbodyOfObject.gameObject.GetComponent<NetworkIdentity>().netId);
            }
        }

        [Command]
        private void CmdMoveAndRotate(Vector3 holdPoint, NetworkInstanceId netId)
        {
            // Execute it on all Clients
            RpcMoveAndRotate(holdPoint, netId);
        }


        [ClientRpc]
        void RpcMoveAndRotate(Vector3 holdPoint, NetworkInstanceId netid) //It gets passed the same arguments.
        {
            GameObject movedObject = ClientScene.FindLocalObject(netid); //This will have each client get a reference to the thing we want to move by searching for its netID.
            Rigidbody rb = movedObject.GetComponent<Rigidbody>(); //Now we can apply the force!

            // Rotate the object to remain consistent with any changes in player's rotation
            rb.MoveRotation(Quaternion.Euler(rotationDifferenceEuler + transform.rotation.eulerAngles));

            // Apply any intentional rotation input made by the player & clear tracked input
            rb.transform.RotateAround(holdPoint, transform.right, rotationInput.y);
            rb.transform.RotateAround(holdPoint, transform.up, -rotationInput.x);

            // Remove all torque, reset rotation input & store the rotation difference for next FixedUpdate call
            rb.angularVelocity = Vector3.zero;
            rotationInput = Vector2.zero;
            rotationDifferenceEuler = rb.transform.rotation.eulerAngles - transform.rotation.eulerAngles;

            // Calculate object's center position based on the offset we stored
            // NOTE: We need to convert the local-space point back to world coordinates
            Vector3 centerDestination = holdPoint - rb.transform.TransformVector(hitOffsetLocal);

            // Find vector from current position to destination
            // Vector3 toDestination = centerDestination - rb.transform.position;

            // Calculate force
            // Vector3 force = toDestination / Time.fixedDeltaTime;

            // Remove any existing velocity and add force to move to final position
            rb.velocity = Vector3.zero;

            rb.MovePosition(centerDestination); // TODO: Fixes the problems of AddForce, but objects cannot be tossed anymore

            // TODO: Somehow the force is suddenly getting extremely huge when a client picks up an object
            //force = Vector3.ClampMagnitude(force, 100f);
            //rb.AddForce(force, ForceMode.VelocityChange);
            //print("Force = "+force);

            if (isThrowing)
            {
                rb.AddForce(cameraFollow.transform.forward * throwForce, ForceMode.Impulse);
                isThrowing = false;
                Drop();
            }
        }

        private void PickUp()
        {
            // Track rigidbody's initial information
            rigidbodyOfObject = hit.rigidbody;
            initialInterpolationSetting = rigidbodyOfObject.interpolation;
            rotationDifferenceEuler = hit.transform.rotation.eulerAngles - transform.rotation.eulerAngles;
            hitOffsetLocal = hit.transform.InverseTransformVector(hit.point - hit.transform.position);
            currentPickUpDistance = Vector3.Distance(transform.position, hit.point);

            // Set rigidbody's interpolation for proper collision detection when being moved by the player
            rigidbodyOfObject.interpolation = RigidbodyInterpolation.Interpolate;
        }

        private void Drop()
        {
            // Reset the rigidbody to how it was before we grabbed it
            rigidbodyOfObject.interpolation = initialInterpolationSetting;
            rigidbodyOfObject = null;
        }
    }
}
