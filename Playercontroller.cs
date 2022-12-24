using UnityEngine;

public class Playercontroller : MonoBehaviour
{
    // Bewegungsgeschwindigkeit des Spielers
    public float moveSpeed = 5f;
    private float currentSpeed;
    // Bewegungsgeschwindigkeit in der Luft
    public float airControl;

    // Bewegungsgeschwindifkeit während des Sprintens
    public float sprintSpeed = 7f;

    // Kraft des Sprungs
    public float jumpForce = 10f;

    // Referenz auf den Rigidbody des Spielers
    private Rigidbody rb;

    // Referenz auf die Kamera
    public Transform cameraTransform;

    // Max und Min Rotationswert der Kamera
    public float maxRotation = 90;
    public float minRotation = -90;
    

    private bool canJump;

    private float xRotation = 0f;
    [Range(0,10)]
    public float mouseSensitivity = 2f;
    private Vector3 movement;

    // Use this for initialization
    void Start()
    {
        // Setzt die aktuelle geschwindgkeit auf die standardt geschwindigket
        currentSpeed = moveSpeed;
        // Sperrt den Mauszeiger im Bildschirm ein
        Cursor.lockState = CursorLockMode.Locked;
        // Den Rigidbody des Spielers speichern
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
       // vertical und horizontal axenwert
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        // Wenn der Spieler nicht mehr WASD drückt wird er stehen bleiben
        if(vertical == 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        // Begrenzt die geschwindigkeit des Spielers auf 15 m/s
        if(rb.velocity.magnitude > 15)
        {
            rb.velocity = rb.velocity.normalized * 15;
        }

        // Bewegung in die Richtung, in die die Kamera schaut
        movement = cameraTransform.forward * vertical
                   + cameraTransform.right * horizontal;

        // Anwenden der Bewegung
        movement.y = 0;
        rb.MovePosition(rb.position + movement * currentSpeed * Time.deltaTime);


        


        // Kamera drehen, basierend auf der Mausbewegung
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the camera based on the y input of the mouse
        xRotation -= mouseY;

        // Clamp the camera rotation
        xRotation = Mathf.Clamp(xRotation, minRotation, maxRotation);

        // Rotiert die Kamera
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
       
        // Rotiert die Y Achse des Spielers auf die Y Achse der Kamera
        transform.Rotate(Vector3.up, mouseX, Space.World);



        // Hüpfen, wenn die Leertaste gedrückt wird
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            canJump = false;
            currentSpeed = airControl;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (vertical > 0)
            {
                rb.AddForce(cameraTransform.forward * jumpForce, ForceMode.Impulse);

            }
        }

        // Wenn LShift gedrückt wird und der Spieler auf dem Boden ist dann Sprintet er
        if (Input.GetKey(KeyCode.LeftShift) && canJump)
        {
            currentSpeed = sprintSpeed;
        } // Falls LShift und der Spieler in der Luft ist dann wird die airControll um 1.5f erhöt
        else if(Input.GetKey(KeyCode.LeftShift) && !canJump)
        {
            currentSpeed = airControl + 1.5f;
        } // Falls der Spieler in der Luft ist wird die Spieler Geschwindigkeit auf airControl geändert
        else if (!canJump)
        {
            currentSpeed = airControl;
        } // Falls der Spieler nicht in der Luft ist wird seine Geschwindigkeit auf airControl geändert
        else if (canJump)
        {
            currentSpeed = moveSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Falls der Spieler den Boden berührt kann er wieder Springen
        if(collision.gameObject.tag == "Ground")
        {
            canJump = true;

        }
    }
}
