using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public GameObject transitionObject;

    public bool startUp = false;
    public string startupScene;

    public void Start()
    {
        if (startUp)
        {
            SceneManager.LoadScene(startupScene);
        }
    }

    public void Transition()
    {
        StartCoroutine(DoTransition());
    }

    public void TransitionQUICK()
    {
        StartCoroutine(DoTransitionReset());
    }


    IEnumerator DoTransition()
    {
        // wait 2 seconds
        yield return new WaitForSeconds(2f);

        // trigger animation
        if (transitionObject != null)
        {
            Animator anim = transitionObject.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Trans");
            }
        }

        // wait 1 more second
        yield return new WaitForSeconds(1f);

        // load scene
        SceneManager.LoadScene("Game");
    }

    IEnumerator DoTransitionReset()
    {
        // trigger animation
        if (transitionObject != null)
        {
            Animator anim = transitionObject.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Trans");
            }
        }

        // wait 1 more second
        yield return new WaitForSeconds(1f);

        // load scene
        SceneManager.LoadScene("Start");
    }
}