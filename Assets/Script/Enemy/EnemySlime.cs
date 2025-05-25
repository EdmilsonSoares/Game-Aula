using UnityEngine;

public class EnemySlime : MonoBehaviour
{
    public float speed = 2f; // Velocidade do slime
    public float patrolDistance = 5f; // Distância total que o slime irá patrulhar (ex: Se 5, 5/2 para cada lado do ponto inicial)
    private Vector2 startPosition; // Posição inicial do slime (o centro da patrulha)
    private int patrolDirection = -1; // 1 para direita, -1 para esquerda
    private bool facingRight = false; // Para controlar a direção visual do sprite

    void Start()
    {
        startPosition = transform.position; // Guarda a posição inicial do slime
    }

    void Update()
    {
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

    // Método para virar o sprite do inimigo
    void Flip()
    {
        facingRight = !facingRight; // Inverte a flag
        // Inverte a escala X do transform para virar o sprite
        Vector3 newScale = transform.localScale;
        newScale.x *= -1; // Multiplica por -1 para virar
        transform.localScale = newScale;
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