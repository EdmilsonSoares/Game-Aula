using UnityEngine;
using System.Collections;

public class EnemySlime : MonoBehaviour
{
    [SerializeField] private float maxHealth = 50f; // Vida máxima do Slime
    private float currentHealth; // Vida atual do Slime
    [SerializeField] private float speed = 2f; // Velocidade do slime
    [SerializeField] private float patrolDistance = 5f; // Distância total que o slime irá patrulhar (ex: Se 5, 5/2 para cada lado do ponto inicial)
    private Vector2 startPosition; // Posição inicial do slime (o centro da patrulha)
    private int patrolDirection = -1; // 1 para direita, -1 para esquerda
    private bool facingRight = false; // Para controlar a direção visual do sprite
    private Animator enemyAnimator;

    [SerializeField] private ElementType slimeElement; // O elemento deste slime (definido no Inspector)
    private bool hasDivided = false; // Flag para garantir que o slime só se divide uma vez

    void Start()
    {
        startPosition = transform.position; // Guarda a posição inicial do slime
        currentHealth = maxHealth;
        enemyAnimator = GetComponent<Animator>();
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

    public void TakeDamage(float amount, ElementType projectileElement)
    {
        if (currentHealth <= 0) return; // Se já estiver morto, não recebe mais dano
        enemyAnimator.SetTrigger("damaged");
        if (IsElementalAdvantage(projectileElement, slimeElement))
        {
            Debug.Log($"Vantagem Elemental! Slime de {slimeElement} atingido por projétil de {projectileElement}. MORTE INSTANTÂNEA!");
            currentHealth = 0; // Zera a vida para forçar a morte
            // O slime é destruído instantaneamente sem divisão
            Die();
            return; // Sai do método após a morte instantânea
        }
        else // Não há vantagem elemental, aplica dano normal e verifica divisão
        {
            currentHealth -= amount;
            Debug.Log($"Slime {gameObject.name} recebeu {amount} de dano. Vida restante: {currentHealth}");
            // 2. Lógica de Divisão (se este slime AINDA PODE dividir)
            if (!hasDivided)
            {
                DivideSlime();
                hasDivided = true; // Marca que este slime original iniciou o processo de divisão
                //return; // Sai do método para não chamar Die() para o slime original, pois DivideSlime() o destruirá
            }
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private bool IsElementalAdvantage(ElementType projectileElement, ElementType slimeElement)
    {
        // Exemplo: Água tem vantagem contra Fogo
        if (projectileElement == ElementType.Hydro && slimeElement == ElementType.Pyro)
        {
            return true;
        }
        if (projectileElement == ElementType.Electro && slimeElement == ElementType.Hydro)
        {
            return true;
        }
        return false;
    }

    private void DivideSlime()
    {
        // Cria um pequeno offset para os novos slimes não nascerem exatamente no mesmo lugar
        //Vector3 spawnOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        Vector3 spawnOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0.5f, 0);
        // Instancia uma CÓPIA DESTE MESMO PREFAB
        GameObject newSlimeInstance = Instantiate(gameObject, transform.position + spawnOffset, Quaternion.identity);
        EnemySlime newSlimeScript = newSlimeInstance.GetComponent<EnemySlime>();
        if (newSlimeScript != null)
        {
            newSlimeScript.currentHealth = newSlimeScript.maxHealth; // Restaura a vida completa
            newSlimeScript.hasDivided = false; // Permite que esta nova instância se divida no futuro
        }
    }
   
    private void Die()
    {
        // Opcional: Animação de morte, som de morte, spawn de itens
        // Ex: GetComponent<Animator>().SetTrigger("Die");
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false; // Desativa o collider para não colidir mais
        Rigidbody2D rb = GetComponent<Rigidbody2D>(); // Desativa o Rigidbody para parar qualquer movimento residual
        if (rb != null) rb.simulated = false; // Impede que a física o afete
        Destroy(gameObject, 0.5f); // Destrói em 0.5 segundos
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