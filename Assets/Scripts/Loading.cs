using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public string sceneName = "SampleScene";
    
    void Start()
    {
        StartCoroutine(LoadScenes());
    }

    IEnumerator LoadScenes()
    {
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime * 0.8f;
            if (timer >= 1f)
            {
                timer = 1f;
            }
            this.GetComponent<Image>().color = new Color(1, 1, 1, timer);
            yield return null;
        }
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene(sceneName);
        
    }
}
