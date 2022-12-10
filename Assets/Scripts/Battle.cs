using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Battle : MonoBehaviour {
    public int playerPopularityBar = 50;
    private GameObject battleCanvas;
    private Slider healthBar;
    private Animator pAnimator;
    private Animator oAnimator;
    private bool playerCanAttack = true;
    private int animationTime = 0;
    private int attackPower = 0;
    private GameObject winnerImage;

    void Awake() {
        winnerImage = GameObject.FindWithTag("WinnerImage");
        winnerImage.SetActive(false);
    }
     
    void Start() {
        battleCanvas = GameObject.FindWithTag("BattleCanvas");
        healthBar = GameObject.FindWithTag("ReputationBar").GetComponent<Slider>();
        pAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();
        oAnimator = GameObject.FindWithTag("Opponent").GetComponent<Animator>();
    }


    void Update() {
        if (healthBar.value >= 100) {
            // Make winner screen appear
            StartCoroutine(Winner());
        }
    }

    void FixedUpdate() {
        if (animationTime > 0) 
            animationTime--;
        else {
            pAnimator.SetInteger("dance", 0);
            oAnimator.SetInteger("dance", 0);
        }
    }

    void OnAttack() {
        playerCanAttack = false;
        healthBar.value += attackPower;
        StartCoroutine(OpponentAttack());
    }

    IEnumerator OpponentAttack() {
        yield return new WaitForSeconds(3);
        int move = Random.Range(1, 4);
        switch (move) {
            case 1:
                animationTime = (int)(30 * 2.5);
                break;
            case 2: case 3:
                animationTime = (int)(50 * 2.5);
                break;
        }

        oAnimator.SetInteger("dance", move);
        // Opponent attack here
        attackPower = 5 * move;
        healthBar.value -= attackPower;

        yield return new WaitForSeconds(3);

        playerCanAttack = true;
    }

    IEnumerator Winner() {
        winnerImage.SetActive(true);
        yield return new WaitForSeconds(3);
        
        SceneManager.LoadScene("Open");
    }

    public void OnHeadbang() {
        if (playerCanAttack) {
            // Strategy code here
            attackPower = 5;
            // Activates animation
            animationTime = (int)(30 * 2.5);
            pAnimator.SetInteger("dance", 1);

            OnAttack();
        }
    }

    public void OnSprinkler() {
        if (playerCanAttack) {
            // Strategy code here
            attackPower = 10;
            // Activates animation
            animationTime = (int)(50 * 2.5);
            pAnimator.SetInteger("dance", 2);

            OnAttack();
        }
    }

    public void OnRobot() {
        if (playerCanAttack) {
            // Strategy code here
            attackPower = 15;
            // Activates animation
            animationTime = (int)(50 * 2.5);
            pAnimator.SetInteger("dance", 3);
            
            OnAttack();
        }
    }
}