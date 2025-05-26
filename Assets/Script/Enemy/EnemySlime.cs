using UnityEngine;

public class EnemySlime : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f; // Vida máxima do Slime
    private float currentHealth; // Vida atual do Slime
    public float speed = 2f; // Velocidade do slime
    public float patrolDistance = 5f; // Distância total que o slime irá patrulhar (ex: Se 5, 5/2 para cada lado do ponto inicial)
    private Vector2 startPosition; // Posição inicial do slime (o centro da patrulha)
    private int patrolDirection = -1; // 1 para direita, -1 para esquerda
    private bool facingRight = false; // Para controlar a direção visual do sprite

    void Start()
    {
        startPosition = transform.position; // Guarda a posição inicial do slime
        currentHealth = maxHealth;
    }

    void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        if (currentHealth <= 0) return; // Se o slime estiver morto, ele não deve patrulhar
        // Calcula o limite esquerdo e direito da patrulha
        float leftLimit = startPosition.x - patrolDistance / 2f;
        float rightLimit = startPosition.x + patrolDistance / 2f;
        // Move o slime na direção atual
        transform.Translate(Vector2.right * patrolDirection * speed * Time.deltaTime);
        // Verifica se o slime atingiu o limite da patrulha
        if (patrolDirection > 0 && transform.position.x >= rightLimit) // Se movendo para a direita e atingiu o limite direito
        {
            patrolDirection = -1; // Vira para a esquerda
            Flip(); // Vira o sprite
        }
        else if (patrolDirection < 0 && transform.position.x <= leftLimit) // Se movendo para a esquerda e atingiu o limite esquerdo
        {
            patrolDirection = 1; // Vira para a direita
            Flip(); // Vira o sprite
        }
    }

    void Flip()
    {
        facingRight = !facingRight; // Inverte a flag
        Vector3 newScale = transform.localScale; // Inverte a escala X do transform para virar o sprite
        newScale.x *= -1; // Multiplica por -1 para virar
        transform.localScale = newScale;
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0) return; // Se já estiver morto, não recebe mais dano

        currentHealth -= amount;
        Debug.Log($"Slime {gameObject.name} recebeu {amount} de dano. Vida restante: {currentHealth}");
        // Opcional: Adicionar feedback visual/sonoro de dano aqui (ex: piscar, som de hit)
        if (currentHealth <= 0)
        {
            Die(); // Chama o método de morte quando a vida chega a zero
        }
    }

    private void Die()
    {
        Debug.Log($"Slime {gameObject.name} morreu!");
        // Opcional: Animação de morte, som de morte, spawn de itens
        // Ex: GetComponent<Animator>().SetTrigger("Die");
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; // Desativa o collider para não colidir mais
        Rigidbody2D rb = GetComponent<Rigidbody2D>(); // Desativa o Rigidbody para parar qualquer movimento residual
        if (rb != null) rb.simulated = false; // Impede que a física o afete
        Destroy(gameObject, 1f); // Destrói em 0.5 segundos
    }

    // Desenha os limites da patrulha no editor para visualização
    void OnDrawGizmos()
    {
        // Garante que startPosition esteja inicializado no editor
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(new Vector2(startPosition.x - patrolDistance / 2f, transform.position.y),
                            new Vector2(startPosition.x + patrolDistance / 2f, transform.position.y));
            Gizmos.DrawWireSphere(new Vector3(startPosition.x - patrolDistance / 2f, transform.position.y, 0), 0.2f);
            Gizmos.DrawWireSphere(new Vector3(startPosition.x + patrolDistance / 2f, transform.position.y, 0), 0.2f);
        }
        else // Desenha os limites mesmo sem estar rodando o jogo
        {
            // Usa a posição atual do objeto como startPosition no editor
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector2(transform.position.x - patrolDistance / 2f, transform.position.y),
                            new Vector2(transform.position.x + patrolDistance / 2f, transform.position.y));
            Gizmos.DrawWireSphere(new Vector3(transform.position.x - patrolDistance / 2f, transform.position.y, 0), 0.2f);
            Gizmos.DrawWireSphere(new Vector3(transform.position.x + patrolDistance / 2f, transform.position.y, 0), 0.2f);
        }
    }
}