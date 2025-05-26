using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int coins;
    [SerializeField] private float velocidade = 8f;
    [SerializeField] private GameObject arma;
    [Header("Configurações do Jump")]
    [SerializeField] private float forcaPulo = 5f;
    [SerializeField] private LayerMask layerSolo;
    [SerializeField] private Vector2 tamanhoCaixa;
    [SerializeField] private float distanciaSolo;
    private float inputHorizontal;
    private Rigidbody2D playerRb;
    private SpriteRenderer sprRen;
    private Animator playerAnimator;
    private bool lookingRight = true;
    private bool jumpInDash = true;
    private bool canDash = true;
    private bool isDashing = false;
    [Header("Configurações do Dash")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f; // Duração real do dash
    [SerializeField] private float dashCooldown = 0.5f; // Tempo de recarga entre dashes
    [Header("Configurações de Vida")]
    [SerializeField] private float maxHealth = 100f; // Vida máxima do jogador
    private float currentHealth; // Vida atual do jogador
    [SerializeField] private string[] damageDealingLayers; // Nomes das camadas que causam dano (configure no Inspector!)
    private bool isDead = false; // Flag para saber se o jogador está morto
    private string invulnerableLayerName = "NoReaction"; // Nome da Layer de invulnerabilidade
    private float invulnerabilityDuration = 2f; // Duração da invulnerabilidade em segundos
    private int originalLayer; // Guarda a layer original do player
    private bool isInvulnerable = false; // Flag para indicar se o player está invulnerável
    [Header("Configurações de Dano em lava")]
    [SerializeField] private float continuousDamageTickInterval = 0.5f; // Intervalo de tempo entre cada tick de dano (ex: 0.5 segundos)
    [SerializeField] private string continuousDamageLayerName = "Lava"; // Nome da Layer que causa dano contínuo (configure no Inspector!)
    private float lastContinuousDamageTime; // Armazena o último momento em que o player sofreu dano contínuo
    private bool inLava = false;

    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        originalLayer = gameObject.layer;
    }

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        sprRen = GetComponent<SpriteRenderer>(); // Pegue a referência para piscar o sprite ao receber dano
        playerAnimator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;
        GerenciadorDeAnimacao();
        PuloJogador();
        DashInput();
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        MoverJogador();
    }

    void GerenciadorDeAnimacao()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        if (inputHorizontal > 0 && !isDashing)
        {
            // Muda a escala do objeto para a direita
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            lookingRight = true;
            if (NoSolo() || inLava)
                playerAnimator.SetBool("walking", true);
        }
        else if (inputHorizontal < 0 && !isDashing)
        {
            // Muda a escala do objeto para a esquerda
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            lookingRight = false;
            if (NoSolo() || inLava)
                playerAnimator.SetBool("walking", true);
        }
        else
        {
            playerAnimator.SetBool("walking", false);
        }

        if (!NoSolo() && !isDashing && !inLava)
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
            if (NoSolo() || inLava)
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
        yield return new WaitForSeconds(dashDuration); // Espera pelo tempo de duração do dash
        StopDash(); // Fim do Dash
        playerAnimator.SetBool("dashing", false);
        yield return new WaitForSeconds(dashCooldown); // Inicia o cooldown do dash
        canDash = true; // Permite um novo dash após o cooldown
    }

    private void StopDash()
    {
        isDashing = false;
        jumpInDash = true;
        playerRb.linearVelocity = new Vector2(0f, playerRb.linearVelocity.y);
    }

    // Este método é chamado quando o jogador colide com outro objeto físico
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return; // Se o jogador já está morto, não receba mais dano
        string collidedLayerName = LayerMask.LayerToName(collision.gameObject.layer); // Obtém o nome da camada do objeto da colisão

        foreach (string layer in damageDealingLayers) // Verifica se a camada do objeto colidido é uma das camadas que causam dano
        {
            if (collidedLayerName == layer)
            {
                DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>(); // Obtem o script DamageDealer do objeto colidido
                float damageAmount = 0f;
                if (damageDealer != null)
                {
                    damageAmount = damageDealer.GetDamageAmount(); // Se o objeto tem um DamageDealer, pega o dano dele
                }
                else
                {
                    // Se a camada está marcada como dano, mas o objeto não tem um DamageDealer,
                    // aplica um dano padrão (isso indica uma configuração faltando)
                    damageAmount = 10f; // Dano padrão caso o DamageDealer esteja faltando
                }
                TakeDamage(damageAmount);
                return; // Sai do loop e do método OnCollisionEnter2D após aplicar o dano
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead) return; // Se o jogador está morto não processa dano.

        string collidedLayerName = LayerMask.LayerToName(collision.gameObject.layer);
        if (collidedLayerName == continuousDamageLayerName) // Verifica se a camada com a qual está em contato é a camada de dano contínuo (ex: "Lava")
        {
            inLava = true;
            if (isInvulnerable) return; // Se o jogador está invulnerável não processa dano

            if (Time.time >= lastContinuousDamageTime + continuousDamageTickInterval) // Verifica se já passou tempo suficiente para aplicar o próximo tick de dano
            {
                DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();
                float damageToApply = damageDealer.GetDamageAmount();
                TakeDamage(damageToApply);
                lastContinuousDamageTime = Time.time; // Atualiza o tempo do último dano contínuo
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        string collidedLayerName = LayerMask.LayerToName(collision.gameObject.layer);

        if (collidedLayerName == continuousDamageLayerName) // Verifica se a saída da colisão é com a camada de lava
        {
            inLava = false;
        }
    }
    // Método para o jogador receber dano
    public void TakeDamage(float amount)
    {
        if (isDead || isInvulnerable) return; // Não recebe dano se já estiver morto ou invulnerável

        currentHealth -= amount;  // Aplica o dano
        Debug.Log($"Player recebeu {amount} de dano. Vida restante: {currentHealth}");
        playerAnimator.SetTrigger("damaged");

        if (currentHealth > 0 && !inLava) // Só inicia invulnerabilidade se não for um hit fatal
        {
            StartCoroutine(Invulnerability());
        }
        if (currentHealth <= 0)
        {
            Die(); // Chama o método de morte se a vida for <= 0
        }
    }

    private IEnumerator Invulnerability()
    {
        isInvulnerable = true; // Marca o player como invulnerável
        gameObject.layer = LayerMask.NameToLayer(invulnerableLayerName); // Muda a camada do jogador para a camada de invulnerabilidade
        Color originalColor = sprRen.color;
        // Loop para piscar o sprite
        float blinkInterval = 0.1f; // Intervalo de piscar
        float timer = 0f;
        while (timer < invulnerabilityDuration)
        {
            sprRen.color = (sprRen.color == originalColor) ? Color.clear : originalColor; // Alterna entre visível e invisível
            yield return new WaitForSeconds(blinkInterval);
            timer += 0.1f;
            blinkInterval -= 0.005f;
        }
        sprRen.color = originalColor; // Garante que o sprite volte à cor original no final
        gameObject.layer = originalLayer; // Volta a layer do jogador para a original
        isInvulnerable = false; // Player não é mais invulnerável
    }

    // Método para lidar com a morte do jogador
    private void Die()
    {
        if (isDead) return; // Garante que a morte não seja processada múltiplas vezes

        isDead = true; // Marca o jogador como morto
        Debug.Log("Player morreu!");

        if (playerRb != null)
        {
            // Desativa o Rigidbody para parar qualquer movimento residual e colisões
            playerRb.linearVelocity = Vector2.zero;
            playerRb.simulated = false; // Desativa completamente a física
        }

        // Desativa o collider do player para que ele não colida mais
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }
        arma.SetActive(false);
        // Ativa a animação de morte (se houver)
        if (playerAnimator != null)
        {
            //playerAnimator.SetTrigger("Die");
            // Opcional: Se a animação de morte tiver um tempo fixo e você quiser esperar
            // StartCoroutine(HandleDeathAnimation(playerAnimator.GetCurrentAnimatorStateInfo(0).length));
        }

        // Lógica de Game Over (ex: carregar cena, exibir tela de Game Over)
        // Por enquanto, apenas um debug, mas aqui você chamaria um GameManager
        Debug.Log("Fim de Jogo!");
        // Exemplo: GameManager.Instance.GameOver();
        // Ou Destroy(gameObject, 3f); // Destrói o player após 3 segundos
        Destroy(gameObject, 0.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - transform.up * distanciaSolo, tamanhoCaixa);
    }

}
