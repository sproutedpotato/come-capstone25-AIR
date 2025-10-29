using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void OnClickS1Button()
    {
        SceneManager.LoadScene("S1");
    }
    public void OnClickS2Button()
    {
        SceneManager.LoadScene("S2");
    }
    public void OnClickS3Button()
    {
        SceneManager.LoadScene("S3");
    }
}
