using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private float seconds = 1f;

    private void Awake()
    {
        GameObject transitionObj = GameObject.Find("Transition");
        if (transitionObj != null)
        {
            animator = transitionObj.GetComponent<Animator>();
        }
    }
        
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(SceneLoadCoroutine(sceneIndex));
    }

    private IEnumerator SceneLoadCoroutine(int sceneIndex)
    {
        animator.SetTrigger("StartTransition");
        yield return new WaitForSecondsRealtime(seconds);


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false; // detiene el modo Play

#else
           
                Application.Quit();
#endif
    }
}
