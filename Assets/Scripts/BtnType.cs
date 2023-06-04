using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnType : MonoBehaviour
{
    public BTNType currentType;
    public void OnBtnclick()
    {
        switch (currentType)
        {
            case BTNType.Start:
                Debug.Log("시작");
                SceneManager.LoadScene("Map_v1");
                break;
        }
    }
}