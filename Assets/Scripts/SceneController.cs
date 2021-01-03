using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneController : MonoBehaviour
{
    [SerializeField]
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        //startButton.GetComponent<Button>();
        button.onClick.AddListener(StartGame);
        Debug.Log(button);
        GameObject text = GameObject.Find("Levels Completed Text");
        if (text != null)
        {
            text.GetComponent<Text>().text = "You completed " + StaticValues.levelsCompleted + " levels!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartGame()
    {
        Debug.Log("flag");
        StaticValues.ResetValues();
        SceneManager.LoadScene("Level 1 Scene");
    }
}
