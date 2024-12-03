using UnityEngine;

public class PlayerAnimationsController : MonoBehaviour
{
    private Animator playerAnim;
    private Vector3 movement;
    private float acceleration = 0.8f;
    private float desacceleration = -2f;
    private float velocity = 0f;
    [SerializeField] private int combo = 1;
    [SerializeField] private bool canAttack = true;
    private float timer = 0;
    private bool activateTimer = false;
    private float waitTime = 0;


    private float attackDelay = 0.3f; // Tiempo mínimo entre ataques
    private float lastAttackTime = 0f; // Marca de tiempo del último ataque
    private bool isAttacking = false; // Controla si el personaje está en un ataque
    void Start()
    {
        playerAnim = GetComponent<Animator>();
        movement = Vector3.zero;
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        movement = new Vector3(horizontal, vertical, transform.position.z);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerAnim.SetTrigger("Jump");
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (canAttack)
            {
                AttemptAttack();
            }
        }

        if (!canAttack)
        {
            if (Time.time > waitTime)
            {
                canAttack = true;
            }
        }

        if (activateTimer)
        {
            timer += Time.deltaTime;
            if (timer > 0.4f)
            {
                ResetCombo();
            }
        }
        CheckState();
    }

    void AttemptAttack()
    {
        float timeSinceLastAttack = Time.time - lastAttackTime;

        if (timeSinceLastAttack >= attackDelay || combo == 1)
        {
            // Inicia el ataque
            lastAttackTime = Time.time;

            if (!activateTimer)
            {
                activateTimer = true;
                timer = 0; // Inicia el temporizador solo en el primer ataque
            }

            playerAnim.SetTrigger("Attack");

            combo++;
            isAttacking = true;

            if (combo > 3)
            {
                ResetCombo();
            }
        }
    }

    void ResetCombo()
    {
        combo = 1;
        isAttacking = false;
        activateTimer = false;
        playerAnim.ResetTrigger("Attack");

    }

    private void CheckState()
    {
        if(movement == Vector3.zero)
        {
            velocity += Time.deltaTime * desacceleration;
            velocity = Mathf.Clamp(velocity, 0f, 1f);
            playerAnim.SetFloat("Movement", velocity);
            playerAnim.SetInteger("RandomIdle", Random.Range(1, 3));
        }
        else
        {
            if(Input.GetKey(KeyCode.LeftShift))
            {
                velocity += Time.deltaTime * acceleration;
                velocity = Mathf.Clamp(velocity, 0.5f, 1f);
                playerAnim.SetFloat("Movement", velocity);
            }
            else
            {
                if(velocity > 0.5f)
                {
                    velocity += Time.deltaTime * desacceleration;
                    velocity = Mathf.Clamp(velocity, 0.5f, 1f);
                    playerAnim.SetFloat("Movement", velocity);
                }
                else
                {
                    velocity += Time.deltaTime * acceleration;
                    velocity = Mathf.Clamp(velocity, 0f, 0.5f);
                    playerAnim.SetFloat("Movement", velocity);
                }
            }
        }
    }
}
