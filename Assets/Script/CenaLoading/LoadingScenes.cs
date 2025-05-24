using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;

public class LoadingScenes : MonoBehaviour
{
    [SerializeField] private Image loadingImage; // Image Radial fill
    [SerializeField] private TMP_Text loadingText;
    private float count = 0f;

    void Start()
    {
        StartCoroutine(CarregarAsync(CenaLoadGerenciador.Instance.NomeProxCena)); // Inicia o carregamento assíncrono da próxima cena
    }

    IEnumerator CarregarAsync(String nomeProxCena)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nomeProxCena);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progresso = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Normaliza o progresso entre 0 e 1

            if (progresso < 1f)
            {
                loadingImage.fillAmount = progresso; // Usa 'progresso' se demorar mais
                loadingText.text = Mathf.RoundToInt(progresso * 100) + "%\nLoading...";
                Debug.Log("Progresso (Carregamento Lento): " + progresso + " Barra: " + loadingImage.fillAmount);
            }
            else if (loadingImage.fillAmount < 1f) // Trecho para rodar a animação de loading mesmo que a cena carregue rápido demais
            {
                loadingImage.fillAmount = count; // Usa 'count' se carregamento for rápido
                Debug.Log("Count (Carregamento Rápido): " + count + " Barra: " + loadingImage.fillAmount);
                loadingText.text = Mathf.RoundToInt(count * 100) + "%\nLoading...";
                count += 0.01f; // Essa variável controla a velocidade do carregamento
            }

            yield return null;

            if (asyncLoad.progress >= 0.9f)
            {
                if (loadingImage.fillAmount >= 1f) // Espera a barra completar (seja por count ou progresso)
                {
                    asyncLoad.allowSceneActivation = true; // Ativa a próxima cena
                }
            }
        }
        // A próxima cena foi carregada e ativada. Destrói a cena de loading
        Destroy(gameObject.scene.GetRootGameObjects()[0]);
    }
}