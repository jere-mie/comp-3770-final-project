using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Winner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Win());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator Win() {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Open");
    }
}
