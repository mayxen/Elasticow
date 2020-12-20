using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationController : MonoBehaviour
{
    public void SetUpIntroAnimation()
    {

        SystemController.instance.PlayIntroMusic();
    }

    public void SetUpEndAnimation()
    {
        SystemController.instance.PlayEndMusic();
    }

    public void EndIntroAnimation()
    {
        SceneManager.LoadScene("Lvl_0");
    }

    public void EndAnimation()
    {
        SceneManager.LoadScene("Menu");
    }
}
