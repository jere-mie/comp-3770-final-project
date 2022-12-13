using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Lose());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator Lose()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("OpenWorld");
    }
}
