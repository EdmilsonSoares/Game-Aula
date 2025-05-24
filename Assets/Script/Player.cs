using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float velocidade = 8f;
    [SerializeField] private float forcaPulo = 5f;
    [SerializeField] private LayerMask layerSolo;
    [SerializeField] private Vector2 tamanhoCaixa;
    [SerializeField] private float distanciaSolo;
    private float inputHorizontal;
    private Rigidbody2D playerRb;
    private SpriteRenderer sprRen;
    private Animator playerAnimator;
    public int coins;
    private bool lookingRight = true;
    private bool jumpInDash = true;
    private bool canDash = true;
    private bool isDashing = false;
    [Header("Configurações do Dash")] // Ajuda a organizar no Inspector
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f; // Duração real do dash
    [SerializeField] private float dashCooldown = 0.5f; // Tempo de recarga entre dashes

    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        sprRen = GetComponent<SpriteRenderer>(); // Pegue a referência mesmo que não use flipX
        playerAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        GerenciadorDeAnimacao();
        PuloJogador();
        DashInput();
    }

    private void FixedUpdate()
    {
        MoverJogador();
    }

    void GerenciadorDeAnimacao()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        if (inputHorizontal > 0 && !isDashing)
        {
            // Muda a escala do objeto para a direita
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            playerAnimator.SetBool("walking", NoSolo());
            lookingRight = true;
        }
        else if (inputHorizontal < 0 && !isDashing)
        {
            // Muda a escala do objeto para a esquerda
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            playerAnimator.SetBool("walking", NoSolo());
            lookingRight = false;
        }
        else
        {
            playerAnimator.SetBool("walking", false);
        }

        if (!NoSolo() && !isDashing)
        {
            playerAnimator.SetBool("jumping", true);
        }
        else
        {
            playerAnimator.SetBool("jumping", false);
        }
        if (Input.GetButtonDown("Fire1") && !playerAnimator.GetBool("walking"))
        {
            playerAnimator.SetTrigger("fire");
        }
    }

    void MoverJogador()
    {
        if (!isDashing)
        {
            playerRb.linearVelocity = new Vector2(inputHorizontal * velocidade, playerRb.linearVelocity.y);
        }
    }

    void PuloJogador()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (NoSolo())
            {
                playerRb.AddForce(new Vector2(0f, forcaPulo), ForceMode2D.Impulse);
                if (isDashing)
                    jumpInDash = false;
            }
            else if (isDashing && jumpInDash)
            {
                playerRb.AddForce(new Vector2(0f, forcaPulo), ForceMode2D.Impulse);
                jumpInDash = false;
            }
        }
    }

    private bool NoSolo()
    {
        bool noSolo = Physics2D.BoxCast(transform.position, tamanhoCaixa, 0, -transform.up, distanciaSolo, layerSolo);
        return noSolo;
    }

    // --- Lógica do Dash ---

    private void DashInput()
    {
        if (Input.GetButtonDown("Fire3") && canDash && !isDashing && NoSolo()) // Configurado para botão L
        {
            StartCoroutine(PerformDash()); // Inicia a corrotina do dash
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;

        float dashDirection = lookingRight ? 1f : -1f;
        playerRb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);
        playerAnimator.SetBool("dashing", true);

        // Espera pelo tempo de duração do dash
        yield return new WaitForSeconds(dashDuration);

        // Fim do Dash
        StopDash();
        playerAnimator.SetBool("dashing", false);

        // Inicia o cooldown do dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true; // Permite um novo dash após o cooldown
    }

    private void StopDash()
    {
        isDashing = false;
        jumpInDash = true;
        playerRb.linearVelocity = new Vector2(0f, playerRb.linearVelocity.y);
    }

    // --- Fim da Lógica do Dash ---

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - transform.up * distanciaSolo, tamanhoCaixa);
    }

}
