using UnityEngine;

public class Rope : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private GameObject objectStart;
    [SerializeField] private GameObject objectEnd;

    [Header("Rope Properties")]
    [SerializeField] private float ropeLength = 5f;
    [SerializeField] private float springForce = 50f;
    [SerializeField] private float ropeAdjustSpeed = 1f;
    
    [Header("Rope Limits")]
    [SerializeField] private float minRopeLength = 2f;
    [SerializeField] private float maxRopeLength = 15f;
    
    private LineRenderer lineRenderer;
    private Rigidbody2D rbStart;
    private Rigidbody2D rbEnd;
    private float adjustTimer = 0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get or add LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        // Configure LineRenderer
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        
        // Optional: Set the material/color of the line
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;

        // Get rigidbodies
        rbStart = objectStart.GetComponent<Rigidbody2D>();
        rbEnd = objectEnd.GetComponent<Rigidbody2D>();

        // Clamp initial rope length to valid range
        ropeLength = Mathf.Clamp(ropeLength, minRopeLength, maxRopeLength);
    }

    void Update()
    {
        // Handle rope length adjustment for numbers 1 and 2
        adjustTimer += Time.deltaTime;
        
        if (adjustTimer >= 0.1f)
        {
            if (Input.GetKey(KeyCode.Alpha1)) // Extend rope
            {
                ropeLength = Mathf.Min(ropeLength + ropeAdjustSpeed, maxRopeLength);
                adjustTimer = 0f;
            }
            else if (Input.GetKey(KeyCode.Alpha2)) // Shorten rope
            {
                ropeLength = Mathf.Max(ropeLength - ropeAdjustSpeed, minRopeLength);
                adjustTimer = 0f;
            }
        }

        // Handle pulling start object to end object
        if (Input.GetKey(KeyCode.Return)) // Pull start to end (extending)
        {
            PullStartToEnd(true);
        }
        else if (Input.GetKey(KeyCode.Delete)) // Pull start to end (shortening)
        {
            PullStartToEnd(false);
        }
    }

    private void PullStartToEnd(bool extending)
    {
        if (objectStart != null && objectEnd != null)
        {
            Renderer startRenderer = objectStart.GetComponent<Renderer>();
            Renderer endRenderer = objectEnd.GetComponent<Renderer>();

            if (startRenderer != null && endRenderer != null)
            {
                Vector3 startPos = startRenderer.bounds.center;
                Vector3 endPos = endRenderer.bounds.center;
                Vector2 direction = (endPos - startPos).normalized;
                float currentDistance = Vector2.Distance(startPos, endPos);

                if (extending && currentDistance < maxRopeLength)
                {
                    if (rbStart != null)
                    {
                        rbStart.AddForce(direction * springForce);
                    }
                    else
                    {
                        objectStart.transform.position += (Vector3)direction * Time.deltaTime * springForce * 0.01f;
                    }
                }
                else if (!extending && currentDistance > minRopeLength)
                {
                    if (rbStart != null)
                    {
                        rbStart.AddForce(-direction * springForce);
                    }
                    else
                    {
                        objectStart.transform.position -= (Vector3)direction * Time.deltaTime * springForce * 0.01f;
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (objectStart != null && objectEnd != null)
        {
            // Get renderers for both objects
            Renderer startRenderer = objectStart.GetComponent<Renderer>();
            Renderer endRenderer = objectEnd.GetComponent<Renderer>();

            if (startRenderer != null && endRenderer != null)
            {
                // Calculate center points using bounds
                Vector3 startPos = startRenderer.bounds.center;
                Vector3 endPos = endRenderer.bounds.center;
                
                // Lock Z axis
                startPos.z = transform.position.z;
                endPos.z = transform.position.z;

                // Calculate current distance between objects
                float currentDistance = Vector2.Distance(startPos, endPos);

                // If distance is greater than rope length, pull end object towards start object
                if (currentDistance > ropeLength)
                {
                    Vector2 ropeDirection = (endPos - startPos).normalized;
                    float excess = currentDistance - ropeLength;

                    if (rbEnd != null)
                    {
                        // Apply force only to end object
                        Vector2 force = -ropeDirection * excess * springForce;
                        rbEnd.AddForce(force);
                    }
                    else
                    {
                        // If no rigidbody, directly adjust position of end object
                        Vector3 adjustment = -ropeDirection * excess;
                        objectEnd.transform.position += adjustment;
                    }
                }
                
                // Update line positions
                lineRenderer.SetPosition(0, startPos);
                lineRenderer.SetPosition(1, endPos);
            }
        }
    }
}
