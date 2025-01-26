using UnityEngine;

public enum Player
{
    Player1,
    Player2
}

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Player playerType;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float maxSpeed = 8f;  // Maximum horizontal speed

    private Rigidbody rb;
    private bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Lock only Z-position and all rotations
        rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                        RigidbodyConstraints.FreezeRotationY | 
                        RigidbodyConstraints.FreezeRotationZ | 
                        RigidbodyConstraints.FreezePositionZ;  // This keeps the player in the 2D plane
    }

    void FixedUpdate()  // Using FixedUpdate for physics-based movement
    {
        float horizontalInput = GetHorizontalInput();
        
        // Direct velocity control for more responsive movement
        float targetVelocityX = horizontalInput * maxSpeed;
        rb.linearVelocity = new Vector3(targetVelocityX, rb.linearVelocity.y, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Get input based on player type
        float horizontalInput = GetHorizontalInput();
        bool jumpInput = GetJumpInput();

        // Handle jumping
        if (jumpInput && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Reset vertical velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private float GetHorizontalInput()
    {
        if (playerType == Player.Player1)
        {
            // WASD Controls
            if (Input.GetKey(KeyCode.D)) return 1f;
            if (Input.GetKey(KeyCode.A)) return -1f;
        }
        else
        {
            // Arrow Controls
            if (Input.GetKey(KeyCode.RightArrow)) return 1f;
            if (Input.GetKey(KeyCode.LeftArrow)) return -1f;
        }
        return 0f;
    }

    private bool GetJumpInput()
    {
        if (playerType == Player.Player1)
        {
            return Input.GetKeyDown(KeyCode.W);
        }
        else
        {
            return Input.GetKeyDown(KeyCode.UpArrow);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        // Check if any of the contact points are below the player
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.7f)  // Check if the surface is somewhat horizontal
            {
                isGrounded = true;
                return;
            }
        }
    }
}
