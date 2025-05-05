using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using UnityEngine.UI;

public class LoadingScenes : MonoBehaviour
{
    [SerializeField] private Image loadingImage; // Image Radial fill
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
                Debug.Log("Progresso (Carregamento Lento): " + progresso + " Barra: " + loadingImage.fillAmount);
            }
            else if (loadingImage.fillAmount < 1f)
            {
                loadingImage.fillAmount = count; // Usa 'count' se carregamento for rápido
                Debug.Log("Count (Carregamento Rápido): " + count + " Barra: " + loadingImage.fillAmount);
                count += 0.0002f;
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