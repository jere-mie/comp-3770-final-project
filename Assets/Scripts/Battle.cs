using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Dance;

public class Battle : MonoBehaviour {
    public int playerPopularityBar = 50;
    private GameObject battleCanvas;
    private Slider healthBar;
    private Animator pAnimator;
    private Animator oAnimator;
    private bool playerCanAttack = true;
    private int animationTime = 0;
    private float attackPower = 0;
    private GameObject winnerImage;
    float damageMultiplier; // arbitrary (for now) damage variable to demonstrate concept
    bool lockout; // Lock to one move while in progress
    int borrowedTime; // time added for each correct key press
    bool notTerminated; // checks for a termination condition
    float storedTime; // game time before the script was frozen
    int activeMove; // current move, if there is one 
    bool animationToggle;
    string buttonPressed;
    IDanceMove robot;
    IDanceMove sprinkler;
    IDanceMove headbang;

    void Awake() {
        winnerImage = GameObject.FindWithTag("WinnerImage");
        winnerImage.SetActive(false);
    }
     
    void Start() {
        battleCanvas = GameObject.FindWithTag("BattleCanvas");
        healthBar = GameObject.FindWithTag("ReputationBar").GetComponent<Slider>();
        pAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();
        oAnimator = GameObject.FindWithTag("Opponent").GetComponent<Animator>();

        damageMultiplier = 0f;
        borrowedTime = 0;
        notTerminated = true;
        activeMove = -1;
        lockout = false;
        storedTime = 0f;
        robot = new Robot();
        sprinkler = new Sprinkler();
        headbang = new Headbang();
        animationToggle = false;
        buttonPressed = "";
    }


    void Update() {
        var timeOffset = storedTime - borrowedTime;
        if (healthBar.value >= 100) {
            // Make winner screen appear
            StartCoroutine(Winner());
        } else {
            Dance();
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
        Debug.Log(attackPower);
        Debug.Log("here");
        StartCoroutine(OpponentAttack());
    }

    IEnumerator OpponentAttack() {
        yield return new WaitForSeconds(3);
        int move = UnityEngine.Random.Range(1, 4);
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
        buttonPressed = "c";
    }

    public void OnSprinkler() {
        buttonPressed = "b";
    }

    public void OnRobot() {
        buttonPressed = "a";
    }
    void Dance() {
    try {
        var timeOffset = storedTime - borrowedTime;
        if (playerCanAttack) {
            if (buttonPressed == "a" && !lockout) {
                if (robot.CurrentCooldown > 0) {
                    Debug.Log($"robot on cooldown; turns left: {robot.CurrentCooldown}");
                } else {
                    Debug.Log($"cooldown = {robot.CurrentCooldown}");
                    Debug.Log("a entered");
                    lockout = true;
                    activeMove = robot.Id;
                    robot.SetCooldown();
                }
            }
            if (buttonPressed == "b" && !lockout) {
                if (sprinkler.CurrentCooldown > 0) {
                    Debug.Log($"sprinkler on cooldown; turns left: {sprinkler.CurrentCooldown}");
                } else {
                    Debug.Log("b entered");
                    lockout = true;
                    activeMove = sprinkler.Id;
                    sprinkler.SetCooldown();
                }
            }
            if (buttonPressed == "c" && !lockout) {
                if (headbang.CurrentCooldown > 0) {
                    Debug.Log($"headbang on cooldown; turns left: {headbang.CurrentCooldown}");
                } else {
                    Debug.Log("c entered");
                    lockout = true;
                    activeMove = headbang.Id;
                    headbang.SetCooldown();
                }
            }
            if (lockout) {
                storedTime += Time.deltaTime;
                switch (activeMove) {
                    case 0: GetInput(ref robot); break;
                    case 1: GetInput(ref sprinkler); break;
                    case 2: GetInput(ref headbang); break;
                    default: break;
                }
            }
            if (timeOffset >= 2) {
                Debug.Log("timeout");
                notTerminated = false;
            }
            if (!notTerminated) {
                Debug.Log("terminating...");
                animationToggle = true;
                switch (activeMove) {
                    case 0: attackPower = robot.Damage * damageMultiplier; break;
                    case 1: attackPower = sprinkler.Damage * damageMultiplier; break;
                    case 2: attackPower = headbang.Damage * damageMultiplier; break;
                    default: break;
                }
                Debug.Log($"damage inflicted: {damageMultiplier * (activeMove == 0 ? robot.Damage : (activeMove == 1 ? sprinkler.Damage : headbang.Damage))}");
                Animation();
                Reset();
            }
        }
    }
    catch (InvalidOperationException e) {
        Debug.Log($"tried to peek when stack was empty: {e}");
    }
}
    void Animation() {
        if (animationToggle) {
            switch (activeMove) {
                case 0:
                    animationTime = (int)(50 * 2.5);
                    pAnimator.SetInteger("dance", 3);
                    break;
                case 1:
                    animationTime = (int)(50 * 2.5);
                    pAnimator.SetInteger("dance", 2);
                    break;
                case 2:
                    animationTime = (int)(30 * 2.5);
                    pAnimator.SetInteger("dance", 1);
                    break;
            }
            animationToggle = false;
            OnAttack();
        }
    }
    void GetInput(ref IDanceMove move) {
        Debug.Log($"made it to GetInput; notTerminated = {notTerminated}");
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Debug.Log("up arrow pressed");
            MatchMove(ref move, KeyCode.UpArrow);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            Debug.Log("down arrow pressed");
            MatchMove(ref move, KeyCode.DownArrow);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            Debug.Log($"Stack: {move.CurrentSequence.Peek()}");
            Debug.Log("left arrow pressed");
            MatchMove(ref move, KeyCode.LeftArrow);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            Debug.Log("right arrow pressed");
            MatchMove(ref move, KeyCode.RightArrow);
        }
    }
    void MatchMove(ref IDanceMove move, KeyCode key) {
        Debug.Log($"move = {move}");
        Debug.Log($"expected key press: {move.CurrentSequence.Peek()}");
        if (move.CurrentSequence.Peek() == key) {
            move.CurrentSequence.Pop();
            borrowedTime += 2;
            Debug.Log("item popped");
        } else {
            Debug.Log("haha what a dumbass");
            notTerminated = false;
        }
        damageMultiplier = (move.FullStackSize - move.CurrentSequence.Count) / move.FullStackSize;
        if (move.CurrentSequence.Count == 0) {
            Debug.Log("move successfully performed"); 
            notTerminated = false;
        }
    }
    void Reset() {
        damageMultiplier = 0f;
        borrowedTime = 0;
        notTerminated = true;
        robot.ResetStack(new(new[] {KeyCode.DownArrow, KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.LeftArrow}));
        sprinkler.ResetStack(new(new[] {KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.DownArrow}));
        headbang.ResetStack(new(new[] {KeyCode.LeftArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.RightArrow}));
        robot.CurrentCooldown--;
        sprinkler.CurrentCooldown--;
        headbang.CurrentCooldown--;
        activeMove = -1;
        lockout = false;
        storedTime = 0f;
        animationToggle = false;
        buttonPressed = "";
    }
}