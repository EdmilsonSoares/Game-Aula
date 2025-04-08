using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float velocidade = 8f;
    [SerializeField] private float forcaPulo = 5f;
    [SerializeField] private LayerMask layerSolo;
    [SerializeField] private Vector2 tamanhoCaixa;
    [SerializeField] private float distanciaSolo;
    private float inputHorizontal;
    private Rigidbody2D rig2D;
    private SpriteRenderer sprRen;
    private Animator playerAnimator;
    public int coins;

    void Start()
    {
        rig2D = GetComponent<Rigidbody2D>();
        sprRen = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        InputJogador();
        PuloJogador();
    }
    private void FixedUpdate()
    {
        MoverJogador();

    }
    void InputJogador()
    {
        inputHorizontal = Input.GetAxis("Horizontal");
        if (inputHorizontal > 0)
        {
            sprRen.flipX = false; // Olhando para a direita
            playerAnimator.SetBool("walking", true);
        }
        else if (inputHorizontal < 0)
        {
            sprRen.flipX = true; // Olhando para a esquerda
            playerAnimator.SetBool("walking", true);
        }
        else
        {
            playerAnimator.SetBool("walking", false);
        }
    }
    void MoverJogador()
    {
        rig2D.linearVelocity = new Vector2(inputHorizontal * velocidade, rig2D.linearVelocity.y);
    }
    void PuloJogador()
    {
        if (Input.GetButtonDown("Jump") && NoSolo())
        {
            rig2D.AddForce(new Vector2(0f, forcaPulo), ForceMode2D.Impulse);
        }
    }

    private bool NoSolo()
    {
        bool noSolo = Physics2D.BoxCast(transform.position, tamanhoCaixa, 0, -transform.up, distanciaSolo, layerSolo);
        return noSolo;
    }
    // Método para desenha a caixa de colisão do BoxCast no editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position-transform.up * distanciaSolo, tamanhoCaixa);
    }

}
