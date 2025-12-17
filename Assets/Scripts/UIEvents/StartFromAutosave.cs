using UnityEngine;
using System.Collections;

public class StartFromAutosave : MonoBehaviour
{
    public void OnButtonClick()
    {
        if (SceneHistory.Instance != null)
        {
            SceneHistory.Instance.LoadFromFile("Autosave");
        }
    }
}