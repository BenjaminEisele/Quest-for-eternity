using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class RefereeScript : MonoBehaviour
{
    [SerializeField]
    EnemyScript targetEnemy; //veliau noretusi padaryti, kad net nereiketu nieko tampyti per inspektoriu, kitaip sakant kad viskas po kapotu butu.

    //public PlayerStatScript playerAccess;

    public List<EnemyScript> enemyList;

    //[SerializeField]
    //TurnScript turnScriptAccess;

    [SerializeField]
    EnemyGenerator ennemyGeneratorAccess;

    private bool isGameOver;
    bool areAllEnemiesDead;
    private bool canTransferTurnToPlayer;

    public GameObject restartGameButton;
    public GameObject lostImage;
    public GameObject winImage;

    [SerializeField]
    int chosenEnemyId;

    [SerializeField]
    ChooseNewCardScript chooseNewCardAccess;

    public delegate void PreNewWaveAction();
    public static event PreNewWaveAction preNewWaveEvent;

    public delegate void NewWaveAction();
    public static event NewWaveAction newWaveEvent;

    public delegate void TurnStartAction();
    public static event TurnStartAction turnStartEvent;

    public static RefereeScript instance;

    private void Awake()
    {
            if(instance == null) { instance = this; }
    }

    private void Start()
    {
        areAllEnemiesDead = false;
        canTransferTurnToPlayer = true;
        TurnScript.restartGameEvent += RefereeReset;
        ennemyGeneratorAccess.GenerateEnemies(1);
        //enemyList.Add(targetEnemy);
        isGameOver = false;
        restartGameButton.SetActive(false);
        winImage.SetActive(false);
        lostImage.SetActive(false);
       // turnStartEvent();
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChooseNewEnemy(1);
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChooseNewEnemy(-1);
        }
    }
    public void ResetChosenEnemy()
    {
        foreach(EnemyScript enemy in enemyList)
        {
            enemy.ChangeSelectedStatus(false);
        }
        chosenEnemyId = 0;
        targetEnemy = enemyList[chosenEnemyId];
        enemyList[chosenEnemyId].ChangeSelectedStatus(true);
    }
    private void ChooseNewEnemy(int inputDirection)
    {
        enemyList[chosenEnemyId].ChangeSelectedStatus(false);
        if (chosenEnemyId + inputDirection < enemyList.Count && chosenEnemyId + inputDirection >= 0)
        {
            chosenEnemyId += inputDirection;
        }
        else if(chosenEnemyId + inputDirection >= enemyList.Count)
        {
            chosenEnemyId = 0;
        }
        else if (chosenEnemyId + inputDirection < 0)
        {
            chosenEnemyId = enemyList.Count - 1;
        }


        targetEnemy = enemyList[chosenEnemyId];
        enemyList[chosenEnemyId].ChangeSelectedStatus(true);
    }
    private void EndGame(bool didPlayerWin)
    {
        TurnScript.instance.SetPlayerTurnBool(false);
        Debug.Log("game end");
        isGameOver = true;
        string winnerName;
        if(didPlayerWin)
        {
            winnerName = "The player";
            winImage.SetActive(true);
        }
        else
        {
            winnerName = "The enemy";
            lostImage.SetActive(true);
        }
        //UiScript.UpdateGameOverText($"Game over! {winnerName} is victorious!");
        restartGameButton.SetActive(true);
    }

    public void StartNextWave(bool shouldStartEvents)
    {
        //Debug.Log("new wave!");
        areAllEnemiesDead = false;
        foreach (EnemyScript enemy in enemyList)
        {
            Destroy(enemy.gameObject);
        }
        enemyList.Clear();
        ennemyGeneratorAccess.GenerateEnemies(Random.Range(1, 3));
        ResetChosenEnemy();
        
        if (shouldStartEvents)
        {
            CallNewWaveEvent();   
        }
    }
    public void CallPreNewWaveEvent()
    {
        if (preNewWaveEvent != null)
        {
            preNewWaveEvent();
        }
    }
    public void CallNewWaveEvent()
    {
        if (newWaveEvent != null)
        {
            newWaveEvent();
        }
    }

    public void CallStartTurnEvent()
    {
        canTransferTurnToPlayer = true;
        if (turnStartEvent != null)
        {
            turnStartEvent();
        }
        TurnScript.instance.ShouldStartPlayerTurn(true);
    }
    public void RefereeReset()
    {
        isGameOver = false;
        /*foreach (EnemyScript enemy in enemyList)
        {
            enemy.ResetEnemy();
        } */
        StartNextWave(false);
        PlayerStatScript.instance.ResetPlayer();
    }
    public bool GetIsGameOver()
    {
        return isGameOver;
    }
    public void StartForeachEnemyCoroutine()
    {
        if(!isGameOver)
        {
            StartCoroutine(ForeachEnemyTurnCoroutine());
            /* foreach (EnemyScript enemy in enemyList)
             {
                 StartCoroutine(EnemyTurnCoroutine(enemy));
             } 
             turnScriptAccess.ShouldStartPlayerTurn(true);*/
            Debug.Log("zzoz");//ar galima sitoj vietoj padaryti kad visa logika eitu tik per turn script puse?
        }
    }
    public void dealDamageToEnemy(int inputDamage)
    {
        if (!targetEnemy.TakeDamageAndCheckIfDead(inputDamage))
        {
            areAllEnemiesDead = true;
            foreach (EnemyScript enemy in enemyList)
            {
                if(enemy.enemyHealth > 0)
                {
                    areAllEnemiesDead = false;
                }
            }
            if(areAllEnemiesDead)
            {
                // for debugging purposes the value is false. But later on it should be switched back to TRUE
                //StartNextWave(false);
                //chooseNewCardAccess.DisplayCards();
                canTransferTurnToPlayer = false;
                CallPreNewWaveEvent();
            }
        }    
    }

    public void dealDamageToPlayer(int inputDamage)
    {
        if(PlayerStatScript.instance.TakeDamageAndCheckIfDead(inputDamage))
        {
            //turnScriptAccess.isPlayersTurn = false;
            TurnScript.instance.ShouldStartPlayerTurn(false);
            EndGame(false);
        }
        //playerAccess.playerHealth -= inputDamage;
    }
    
    private IEnumerator ForeachEnemyTurnCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        if (!areAllEnemiesDead)
        {
            foreach (EnemyScript enemy in enemyList)
            {
                int enemyDamage = enemy.GenerateAttack();
                dealDamageToPlayer(enemyDamage);
                UiScript.UpdateFieldDamageText(enemyDamage.ToString(), false);
                yield return new WaitForSeconds(0.75f);
            }
        }
        

        if(canTransferTurnToPlayer)
        {
            CallStartTurnEvent(); 
        }
        
       // Debug.Log("attack over");
        //turnScriptAccess.ShouldStartPlayerTurn(true);
    }
}
