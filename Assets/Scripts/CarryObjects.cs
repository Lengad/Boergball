using UnityEngine;
using UnityEngine.UIElements;
using Input = UnityEngine.Input;

namespace Assets.Scripts
{

    // Credits to u/nandos13 for his script https://www.reddit.com/r/Unity3D/comments/7nul40/simple_gravity_gun_script_well_commented/
    public class CarryObjects : MonoBehaviour
    {
        private const float MaxPickupDistance = 2f;
        private RaycastHit hit;
        private Rigidbody rigidbodyOfObject;

        /// <summary>The offset vector from the object's position to hit point, in local space</summary>
        private Vector3 hitOffsetLocal;

        /// <summary>The distance we are holding the object at</summary>
        private float currentPickUpDistance;

        /// <summary>The interpolation state when first grabbed</summary>
        private RigidbodyInterpolation initialInterpolationSetting;

        /// <summary>The difference between player & object rotation, updated when picked up or when rotated by the player</summary>
        private Vector3 rotationDifferenceEuler;

        /// <summary>Tracks player input to rotate current object. Used and reset every fixedupdate call</summary>
        private Vector2 rotationInput;

        // Update is called once per frame
        void Update()
        {
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

                Debug.DrawLine(transform.position, transform.position + transform.forward * MaxPickupDistance, Color.yellow);
                if (Physics.Raycast(transform.position, transform.forward, out hit, MaxPickupDistance))
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
            }
        }

        void FixedUpdate()
        {
            if (rigidbodyOfObject)
            {
                // We are holding an object, time to rotate & move it
                
                // Rotate the object to remain consistent with any changes in player's rotation
                rigidbodyOfObject.MoveRotation(Quaternion.Euler(rotationDifferenceEuler + transform.rotation.eulerAngles));

                // Get the destination point for the point on the object we grabbed
                Vector3 holdPoint = transform.position + transform.forward * currentPickUpDistance;
                Debug.DrawLine(transform.position, holdPoint, Color.blue, Time.fixedDeltaTime);

                // Apply any intentional rotation input made by the player & clear tracked input
                Vector3 currentEuler = rigidbodyOfObject.rotation.eulerAngles;
                rigidbodyOfObject.transform.RotateAround(holdPoint, transform.right, rotationInput.y);
                rigidbodyOfObject.transform.RotateAround(holdPoint, transform.up, -rotationInput.x);

                // Remove all torque, reset rotation input & store the rotation difference for next FixedUpdate call
                rigidbodyOfObject.angularVelocity = Vector3.zero;
                rotationInput = Vector2.zero;
                rotationDifferenceEuler = rigidbodyOfObject.transform.rotation.eulerAngles - transform.rotation.eulerAngles;

                // Calculate object's center position based on the offset we stored
                // NOTE: We need to convert the local-space point back to world coordinates
                Vector3 centerDestination = holdPoint - rigidbodyOfObject.transform.TransformVector(hitOffsetLocal);

                // Find vector from current position to destination
                Vector3 toDestination = centerDestination - rigidbodyOfObject.transform.position;

                // Calculate force
                Vector3 force = toDestination / Time.fixedDeltaTime;

                // Remove any existing velocity and add force to move to final position
                rigidbodyOfObject.velocity = Vector3.zero;
                rigidbodyOfObject.AddForce(force, ForceMode.VelocityChange);
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
