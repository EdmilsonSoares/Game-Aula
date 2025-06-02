// Anexe esse script na camera
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Referência ao objeto do jogador
    public float minX, maxX; // Limites da câmera no eixo X
    public float minY, maxY; // Limites da câmera no eixo Y
    public float timeLerp; // Tempo de interpolação da câmera

    private void FixedUpdate()
    {
        if (player != null)
        {
            // Atualiza a posição da câmera para seguir o jogador
            Vector3 novaPosicao = player.position + new Vector3(0, 0, transform.position.z);
            // Interpola a posição da câmera para suavizar o movimento
            novaPosicao = Vector3.Lerp(transform.position, novaPosicao, timeLerp);
            transform.position = novaPosicao;
            // Limita a posição da câmera no eixo X
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), transform.position.y, transform.position.z);
            // Limita a posição da câmera no eixo Y
            transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, minY, maxY), transform.position.z);
        }
    }
}
