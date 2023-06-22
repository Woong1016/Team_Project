using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoHome : MonoBehaviour
{
    public void Change()
    {
        SceneManager.LoadScene("Home");
    }
}