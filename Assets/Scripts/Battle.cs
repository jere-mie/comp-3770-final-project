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
    float damageMultiplier; // the percentage of correct moves (in decimal form) from the user
    bool lockout; // Lock to one move while in progress
    int borrowedTime; // time added for each correct key press
    bool notTerminated; // checks for a termination condition
    float storedTime; // game time before the script was frozen
    int activeMove; // current move, if there is one 
    bool animationToggle; // toggle for whether animations can be used in the current state of the program
    string buttonPressed; // which of the three move buttons in the GUI is pressed
    IDanceMove robot; // robot dance object
    IDanceMove sprinkler; // sprinkler object
    IDanceMove headbang; // headbang object
    GameObject buttons; // all player attack buttons
    Button headbangButton;
    Button robotButton;
    Button sprinklerButton;
    ColorBlock defaultColors;

    void Awake() {
        winnerImage = GameObject.FindWithTag("WinnerImage");
        winnerImage.SetActive(false);
    }
     
    void Start() {
        battleCanvas = GameObject.FindWithTag("BattleCanvas");
        healthBar = GameObject.FindWithTag("ReputationBar").GetComponent<Slider>();
        pAnimator = GameObject.FindWithTag("Player").GetComponent<Animator>();
        oAnimator = GameObject.FindWithTag("Opponent").GetComponent<Animator>();
        buttons =  GameObject.FindWithTag("AttackButtons");
        headbangButton = GameObject.FindWithTag("Headbang").GetComponent<Button>();
        robotButton = GameObject.FindWithTag("Robot").GetComponent<Button>();
        sprinklerButton = GameObject.FindWithTag("Sprinkler").GetComponent<Button>();
        defaultColors = headbangButton.colors;

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
        buttons.SetActive(false);
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
        buttons.SetActive(true);
        playerCanAttack = true;
    }

    IEnumerator Winner() {
        winnerImage.SetActive(true);
        yield return new WaitForSeconds(3);
        
        SceneManager.LoadScene("Open");
    }
    /// <summary>
    /// Method <c>OnHeadbang</c> triggers when headbang button pressed.
    /// </summary>
    public void OnHeadbang() {
        buttonPressed = "c";
    }
    /// <summary>
    /// Method <c>OnSprinkler</c> triggers when sprinkler button pressed.
    /// </summary>
    public void OnSprinkler() {
        buttonPressed = "b";
    }
    /// <summary>
    /// Method <c>OnRobot</c> triggers when robot button pressed.
    /// </summary>
    public void OnRobot() {
        buttonPressed = "a";
    }
    /// <summary>
    /// Method <c>Dance</c> detects when a button is pressed in-game using buttonPressed, which then modulates the lockout state variable which determines whether arrow key input will be accepted. It also handles game time, timeout, and resetting the script as needed.
    /// </summary>
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
                    ColorBlock cb = robotButton.colors;
                    cb.normalColor = Color.green;
                    robotButton.colors = cb;
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
                    ColorBlock cb = sprinklerButton.colors;
                    cb.normalColor = Color.green;
                    sprinklerButton.colors = cb;
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
                    ColorBlock cb = headbangButton.colors;
                    cb.normalColor = Color.green;
                    headbangButton.colors = cb;
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
                SetOriginal();
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
    /// <summary>
    /// Method <c>Animation</c> controls player animations, using animationToggle to determine whether it should be searching for an animation to play at a given time, since this runs in Dance which runs in Update.
    /// </summary>
    void Animation() {
        if (animationToggle) {
            switch (activeMove) {
                case 0:
                    if (damageMultiplier > 0) {
                        animationTime = (int)(50 * 2.5);
                        pAnimator.SetInteger("dance", 3);
                    }
                    break;
                case 1:
                    if (damageMultiplier > 0) {
                        animationTime = (int)(50 * 2.5);
                        pAnimator.SetInteger("dance", 2);
                    }
                    break;
                case 2:
                    if (damageMultiplier > 0) {
                        animationTime = (int)(30 * 2.5);
                        pAnimator.SetInteger("dance", 1);
                    }
                    break;
            }
            animationToggle = false;
            OnAttack();
        }
    }
    /// <summary>
    /// Method <c>GetInput</c> is called from Dance when the lockout variable, which stops GUI button input, is set. This method checks for user arrow key input and passes it to MatchMove along with the current move.
    /// </summary>
    /// <param name="move">The currently active move, passed by reference so the original is modified.</param>
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
    /// <summary>
    /// Method <c>MatchMove</c> pops from the stack and calculates damage for each accepted move. Also increments borrowedTime, which is used in Dance to control the move timeout. If a move fails/timeout occurs, notTerminated is set and, as seen in Dance, the termination logic occurs.
    /// </summary>
    /// <param name="move">The currently active move, passed by reference so the original is modified.</param>
    /// <param name="key">The key that was pressed by the user, passed from GetInput</param>
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
    /// <summary>
    /// Method <c>Reset</c> resets variables, decrements cooldown, resets stack, all in preparation for the next move to be executed by the user.
    /// </summary>
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
    /// <summary>
    /// Method <c>SetGreen</c> sets a GameObject's colour to green.
    /// </summary>
    /// <param name="r">A Renderer object, passed by reference, that is tied to a GameObject.</param>
    void SetGreen(ref Image img) => img.material.color = Color.green;
    /// <summary>
    /// Method <c>SetOriginal</c> sets a GameObject's colour to white.
    /// </summary>
    void SetOriginal() => headbangButton.colors = robotButton.colors = sprinklerButton.colors = defaultColors;
}