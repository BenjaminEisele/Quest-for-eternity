using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;


public class RefereeScript : NetworkBehaviour
{
    public List<EnemyScript> enemyList;
    public List<EnemyScript> killedEnemyList;

    public EnemyGenerator enemyGeneratorAccess;
    
    public bool isGameOver;
    private bool areAllEnemiesDead;
    [SyncVar]
    public bool canTransferTurnToPlayer;

    [SyncVar]
    public bool isServersTurn;

    [SyncVar]
    public bool shouldSwitchTargetPlayer;

    public GameObject restartGameButton;
    public GameObject lostImage;
    public GameObject winImage;

    [HideInInspector]
    public int chosenEnemyId;
    [SyncVar]
    public int waveCount = 0;

    [SyncVar]
    public int targetPlayerId = 1;

    public int actionCardInLootTableCount = 0;

    public delegate void PreNewWaveAction();
    public event PreNewWaveAction preNewWaveEvent;

    public delegate void NewWaveAction();
    public event NewWaveAction newWaveEvent;

    public delegate void TurnStartAction();
    public event TurnStartAction turnStartEvent;

    public delegate void RestartGameAction();
    public event RestartGameAction restartGameEvent;

    private GameObject[] card;
    private GameObject[] button;
    private GameObject[] mainCamera;
    private GameObject[] playerHealth;
    private GameObject[] playerScripts;
    private GameObject[] deckText;
    private GameObject[] overlay;

    public List<PlayerScript> playerList;

    public static RefereeScript instance;

    public bool singlePlayerMode;

    //[SyncVar]
    //int[] randomNumbers = new int[4];
    [SyncVar]
    public int randomEnemyCount = 0;
    public readonly SyncList<int> displayCardIdList = new SyncList<int>();
    public List<int> lootIdList;
    Coroutine myCoroutine = null;

    public int lootCardCount;

    [SerializeField]
    public DatabaseMultiplayer databaseMultiplayerAccess;

    public Coroutine glowCoroutine;

    private void Awake()
    {
        instance = this;   
       
    }

    private void Start()
    {
        preNewWaveEvent += CallSwitchEnemyIdNestEvent;
        restartGameEvent += RestartRefereeScript;
        if (playerScripts == null)
        {
            playerScripts = GameObject.FindGameObjectsWithTag("PlayerScriptTag");
            SetPlayerList(playerScripts);
        }
        if(playerList.Count == 2)
        {
            singlePlayerMode = false;
            if (card == null)
            {
                card = GameObject.FindGameObjectsWithTag("Cards");
                DeactivateCards(card);
            }
            if (button == null)
            {
                button = GameObject.FindGameObjectsWithTag("EndTurnButton");
                DeactivateButton(button);
            }
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindGameObjectsWithTag("MainCamera");
                DeactivateCamera(mainCamera);
            }
            if (playerHealth == null)
            {
                playerHealth = GameObject.FindGameObjectsWithTag("Health");
                DeactivateHealth(playerHealth);
            }
            if (deckText == null)
            {
                deckText = GameObject.FindGameObjectsWithTag("DeckText");
                DeactivateDeckText(deckText);
            }
            if (overlay == null)
            {
                overlay = GameObject.FindGameObjectsWithTag("Overlay");
                DeactivateOverlay(overlay);
            }
        }
        else
        {
            singlePlayerMode = true;
            playerList[0].EndTurnSubscription();
            targetPlayerId = 0;
        }
        
        shouldSwitchTargetPlayer = true;
        areAllEnemiesDead = false;
        canTransferTurnToPlayer = true;
        isGameOver = false;
        VoiceManager.instance.StartMatchLine();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChooseNewEnemy(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChooseNewEnemy(-1);
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            RpcCallRestartGameEvent();
        }
        if(isGameOver)
        {
            MusicManager.instance.StopGameMusic();
            //VoiceManager.instance.RestartGameVoice();
        }
    }
    private void RestartRefereeScript()
    {
        waveCount = 0;
        foreach (EnemyScript enemy in enemyList)
        {
            Destroy(enemy.gameObject);
        }
        enemyList.Clear();
        foreach (EnemyScript enemy in killedEnemyList)
        {
            Destroy(enemy.gameObject);
        }
        killedEnemyList.Clear();
        isGameOver = false;
        areAllEnemiesDead = false;
        databaseMultiplayerAccess.genericLootList.Clear();
        databaseMultiplayerAccess.genericLootList.AddRange(databaseMultiplayerAccess.savedGenericLootList);
        RandomNumbersSetUpRoot();
        enemyGeneratorAccess.GenerateEnemies(1, false);
        ResetChosenEnemy();
        if (singlePlayerMode)
        {
            targetPlayerId = 0;
        }
        else
        {
            targetPlayerId = 1;
        }
        StopAllCoroutines();
    }
    [ClientRpc]
    public void RpcCallRestartGameEvent()
    {
        if (restartGameEvent != null)
        {
            Debug.Log("Restart event called!");
            restartGameEvent();
            lostImage.SetActive(false);
            winImage.SetActive(false);
        }
    }

    public void Quit()
    {
        playerList[0].transform.root.GetComponentInChildren<PlayerObjectController>().QuitCheck();
    }

    private void CallSwitchEnemyIdNestEvent()
    {
        SwitchPlayerAttackIdNest(false);
    }
    private void SetPlayerList(GameObject[] Scripts)
    {
        foreach (GameObject Script in Scripts)
        {
            playerList.Add(Script.GetComponent<PlayerScript>());
        }
        RandomNumbersSetUpRoot();
        enemyGeneratorAccess.GenerateEnemies(1, false);
    }
    public void RandomNumbersSetUpRoot()
    {
        databaseMultiplayerAccess.updatedLootList.AddRange(databaseMultiplayerAccess.genericLootList);
        DatabasePlayer databasePlayerAccess = playerList[0].transform.root.GetComponentInChildren<DatabasePlayer>();
        if (isServer)
        {
            if (databasePlayerAccess != null)
            {
                RandomNumberGeneration(databaseMultiplayerAccess.updatedLootList.Count);
            }
        }
    }
    public void RandomNumberGeneration(int maximumValue)
    {
        displayCardIdList.Clear();
        if (isServer)
        {
            actionCardInLootTableCount = 0;
            randomEnemyCount = Random.Range(2, 3);
            enemyGeneratorAccess.RandomNumber(randomEnemyCount);
            for (int i = 0; i < 8; i++)
            {
                int randomValue = Random.Range(0, maximumValue);
                if(i == 0)
                {
                    while (!IsLootIdValidAndAction(randomValue))
                    {
                        randomValue = Random.Range(0, maximumValue);
                    }
                    
                }
                else
                {
                    while (!IsLootIdValid(randomValue))
                    {
                        randomValue = Random.Range(0, maximumValue);
                    }
                }
                
                displayCardIdList.Add(databaseMultiplayerAccess.updatedLootList[randomValue]);
                //databaseMultiplayerAccess.genericLootList.Remove(databaseMultiplayerAccess.updatedLootList[randomValue]);
                lootIdList.Add(randomValue);
            }
            databaseMultiplayerAccess.updatedLootList.Clear();
            lootIdList.Clear();
        }
    }
    private bool IsLootIdValid(int inputId)
    {
        for (int i = 0; i < lootIdList.Count; i++)
        {
            DatabasePlayer databasePlayerReference = playerList[0].transform.root.GetComponentInChildren<DatabasePlayer>();
            int trueId = databaseMultiplayerAccess.updatedLootList[inputId];
            Action actionReference = databasePlayerReference.cardList[trueId] as Action;
            if (actionReference)
            {
                if (actionCardInLootTableCount + 1 >= 2)
                {
                    return false;
                }
                else
                {
                    actionCardInLootTableCount++;
                }
            }
            if (inputId == lootIdList[i])
            {
                return false;
            }
            
        }
        return true;
    }
    private bool IsLootIdValidAndAction(int inputId)
    {
        DatabasePlayer databasePlayerReference = playerList[0].transform.root.GetComponentInChildren<DatabasePlayer>();
        int trueId = databaseMultiplayerAccess.updatedLootList[inputId];
        Action actionReference = databasePlayerReference.cardList[trueId] as Action;
        if (!actionReference)
        {
            Debug.Log("i am null!");
            return false;
        }
        Debug.Log($"adding id {trueId} to list");
        //Debug.Log($"everything works, my ID is: {trueId}");
        return true;
    }
    public int GetRandomNumber(int inputIndex)
    {
        return displayCardIdList[inputIndex];
    }
    private void DeactivateHealth(GameObject[] health)
    {
        Vector3 vec = new Vector3(10, 16.37f, 1.050181f);
        if (isServer)
        {
            health[1].transform.localPosition = vec;
        }
        else
        {
            health[0].transform.localPosition = vec;
        }
    }

    private void DeactivateCamera(GameObject[] camera)
    {
        if (isServer)
        {
            camera[1].SetActive(false);
        }
        else
        {
            camera[0].SetActive(false);
        }
    }

    private void DeactivateButton(GameObject[] button)
    {
        if (isServer)
        {
            button[2].SetActive(false);
            button[3].SetActive(false);
        }
        else
        {
            button[0].SetActive(false);
            button[1].SetActive(false);
        }
    }

    private void DeactivateCards(GameObject[] cards)
    {
        if (isServer)
        {
            for (int i = 7; i < cards.Length; i++)
            {
                card[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 1; i < 7; i++)
            {
                card[i].SetActive(false);
            }
        } 
    }

    private void DeactivateDeckText(GameObject[] deckText)
    {
        if (isServer)
        {
            deckText[1].SetActive(false);
        }
        else
        {
            deckText[0].SetActive(false);
        }
    }

    private void DeactivateOverlay(GameObject[] overlay)
    {
        if (isServer)
        {
            overlay[1].SetActive(false);
        }
        else
        {
            overlay[0].SetActive(false);
        }
    }

    public void CallStartTurnEvent()
    {
        if (turnStartEvent != null)
        {
            turnStartEvent();
        }
        if (!singlePlayerMode)
        {
            if (isServer)
            {
                if (!playerList[1].isPlayerAlive)
                {
                    Invoke("RpcCallEndTurnEventForPlayer", 0.1f);
                }
            }
            else
            {
                if (!playerList[0].isPlayerAlive)
                {
                    Invoke("CmdCallEndTurnEventForPlayer", 0.1f);
                }
            }
            
        }
        TurnScript.instance.ShouldStartPlayerTurn(true);
    }

    [ClientRpc]
    private void RpcCallEndTurnEventForPlayer()
    {
        if (isClientOnly)
        {
            playerList[1].turnScriptAccess.CallEndTurnEvent();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdCallEndTurnEventForPlayer()
    {
        playerList[0].turnScriptAccess.CallEndTurnEvent();
    }
    public void CallEndTurnForBothPlayers()
    {
        canTransferTurnToPlayer = true;
        if(singlePlayerMode)
        {
            playerList[0].EndTurnPlayerScript();
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                playerList[i].EndTurnPlayerScript();
            }
        }
        
    }

    [Command(requiresAuthority = false)]
    public void CmdCallEndTurnForBothPlayers()
    {
        canTransferTurnToPlayer = true;
        CallEndTurnForBothPlayers();
    }
    public void ResetChosenEnemy()
    {
        if(!areAllEnemiesDead)
        {
            foreach (EnemyScript enemy in enemyList)
            {
                enemy.ChangeSelectedStatus(false);
            }
            chosenEnemyId = 0;
            enemyList[chosenEnemyId].ChangeSelectedStatus(true);
        }
    }
    public void ChosenEnemyClick(EnemyScript inputEnemy)
    {
        if (!areAllEnemiesDead)
        {
            foreach (EnemyScript enemy in enemyList)
            {
                enemy.ChangeSelectedStatus(false);
            }
        }
        for(int i = 0; i < enemyList.Count; i++)
        {
            if(GameObject.ReferenceEquals(inputEnemy.gameObject, enemyList[i].gameObject))
            {
                chosenEnemyId = i;
                enemyList[chosenEnemyId].ChangeSelectedStatus(true);
                break;
            }
        }
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
        enemyList[chosenEnemyId].ChangeSelectedStatus(true);
    }
    public void NewWaveCheck()
    {
        areAllEnemiesDead = true;
        foreach (EnemyScript enemy in enemyList)
        {
            if (enemy.enemyHealth > 0)
            {
                areAllEnemiesDead = false;
            }
        }
        if (areAllEnemiesDead)
        {
            playerList[0].transform.root.GetComponentInChildren<FieldScript>().FieldClear();
            playerList[1].transform.root.GetComponentInChildren<FieldScript>().FieldClear();
            if (!singlePlayerMode)
            {
                canTransferTurnToPlayer = false;
            }
            if (playerList[0].isThisPlayersTurn)//isServersTurn)
            {
                playerList[0].BeginPreNewWaveCall();
                playerList[0].transform.root.GetComponentInChildren<ChooseNewCardScript>().ChangeGlowEffectStatus(true);

            }
            else
            {
                playerList[1].BeginPreNewWaveCall();
                glowCoroutine = StartCoroutine(ChangeGlowCoroutine());
            }
        }
    }

    private IEnumerator ChangeGlowCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        playerList[1].transform.root.GetComponentInChildren<ChooseNewCardScript>().ChangeGlowEffectStatus(true);
        glowCoroutine = null;
    }

    public void CallPreNewWaveEvent()
    {
        if (isServer)
        {
            if (waveCount < 3)
            {
                if (preNewWaveEvent != null)
                {
                    preNewWaveEvent();
                    waveCount++;
                }
            }
            else
            {
                EndGame(true);
            }
        }
    }
    [ClientRpc]
    private void EndGame(bool didPlayerWin)
    {
        playerList[0].transform.root.GetComponentInChildren<HandScript>().canInteract = false;
        playerList[1].transform.root.GetComponentInChildren<HandScript>().canInteract = false;
        TurnScript.instance.SetPlayerTurnBool(false);
        isGameOver = true;
        if(didPlayerWin)
        {
            winImage.SetActive(true);
        }
        else
        {
            lostImage.SetActive(true);
        }
    }
    
    public void CallNewWaveEvent()
    {
        if (newWaveEvent != null)
        {
            newWaveEvent();
        }
    }
    public void StartNextWaveInitalize()
    {
        if(isClientOnly)
        {
            CmdStartNextWave();
        }
        else
        {
            RpcStartNextWave(true);
        }
    }
    [ClientRpc]
    public void RpcStartNextWave(bool shouldStartEvents)
    {
        StartNextWaveLogic(shouldStartEvents);
    }
    [Command(requiresAuthority = false)]
    private void CmdStartNextWave()
    {
        RpcStartNextWave(true);
    }
    private void StartNextWaveLogic(bool shouldStartEvents)
    {
        areAllEnemiesDead = false;
        if (myCoroutine != null)
        {
            StopCoroutine(myCoroutine);
            if (canTransferTurnToPlayer)
            {
                CallStartTurnEvent();
            }
            myCoroutine = null;
        }
        foreach (EnemyScript enemy in killedEnemyList)
        {
            if(enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }
        killedEnemyList.Clear();
        enemyList.Clear();
        if(waveCount == 3)
        {
            enemyGeneratorAccess.GenerateEnemies(1, false);
        }
        else
        {
            enemyGeneratorAccess.GenerateEnemies(randomEnemyCount, false);
        }
        if (shouldStartEvents)
        {
            CallNewWaveEvent();   
        }
    }
    
    public void RefereeReset()
    {
        isGameOver = false;
        StartNextWaveInitalize();
    }
    public bool GetIsGameOver()
    {
        return isGameOver;
    }
    public void StartForeachEnemyCoroutine()
    {
        if (!isGameOver)
        {
            if(myCoroutine == null)
            {
                myCoroutine = StartCoroutine(ForeachEnemyTurnCoroutine());
            }   
        }
    }
    private IEnumerator ForeachEnemyTurnCoroutine()
    {
        // COROUTINE GETS CALLED ONCE.
        yield return new WaitForSeconds(1.5f);
        if (!areAllEnemiesDead)
        {
            int loopCount = enemyList.Count;
            for (int i = 0; i < loopCount; i++)
            {
                if(enemyList[i].canAttack)
                {
                    int voiceReference = enemyList[i].voiceReference;
                    int enemyDamage = enemyList[i].GenerateAttack();
                    int enemyType = enemyList[i].myEnemyType;
                    if (isClientOnly)
                    {
                        CmdDealDamageToPlayer(enemyDamage, enemyType, voiceReference);
                    }
                    else
                    {
                        RpcDealDamageToPlayer(enemyDamage, enemyType, voiceReference);
                    }
                    UiScript.UpdateFieldDamageText(enemyDamage.ToString(), false);
                }
                else
                {
                    enemyList[i].canAttack = true;
                }
                yield return new WaitForSeconds(0.75f);
            }
            ResetDamageMultiplier();
            if (shouldSwitchTargetPlayer)
            {
                SwitchPlayerAttackIdNest(false);
            }
            shouldSwitchTargetPlayer = true;
        }
        if (canTransferTurnToPlayer)
        {
            CallStartTurnEvent();
        }
        myCoroutine = null;
    }

    [ClientRpc]
    public void RpcDealDamageToPlayer(int inputDamage, int inputType, int voiceReference)
    {
        if(isClientOnly ^ singlePlayerMode)
        {
            DealDamageLogic(inputDamage, inputType, voiceReference);
        }
    }
    [Command(requiresAuthority = false)]
    public void CmdDealDamageToPlayer(int inputDamage, int inputType, int voiceReference)
    {
        DealDamageLogic(inputDamage, inputType, voiceReference);
    }

    public void DealDamageLogic(int inputDamage, int inputType, int voiceReference)
    {
        if (playerList[targetPlayerId].transform.root.GetComponentInChildren<PlayerStatScript>().TakeDamageAndCheckIfDead(inputDamage, inputType, voiceReference))
        {
            TurnScript.instance.ShouldStartPlayerTurn(false);
            //Debug.Log("set to false");
            //playerList[targetPlayerId].isPlayerAlive = false;
            if (AreAllPlayersDead())
            {
                EndGame(false);
            }
        }
        else
        {
           // Debug.Log("set to true");

            //playerList[targetPlayerId].isPlayerAlive = true;
        }
    }
    
    public void SwitchPlayerAttackIdNest(bool isActivatedByEffect)
    {   
        
        if (isServer)
        {
            SwitchPlayerAttackId();
            if (!isActivatedByEffect)
            {
                playerList[0].transform.root.GetComponentInChildren<UiScript>().DestroyIcon(29, 0);
            }
        }
        else
        {
            CmdSwitchPlayerAttackId();
            if (!isActivatedByEffect)
            {
                playerList[1].transform.root.GetComponentInChildren<UiScript>().DestroyIcon(29, 0);
            }
        }
    }

    private void SwitchPlayerAttackId()
    {
        if(!singlePlayerMode)
        {           
            if (targetPlayerId == 1)
            {
                targetPlayerId = 0;
            }
            else
            {
                targetPlayerId++;
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdSwitchPlayerAttackId()
    {
        if (!singlePlayerMode)
        {
            if (targetPlayerId == 1)
            {
                targetPlayerId = 0;
            }
            else
            {
                targetPlayerId++;
            }
        }
    }

    public void SpecialAttackCounterNest(bool shouldSet)
    {
        if (isServer)
        {
            RpcAttackCounter(shouldSet);
        }
        else
        {
            CmdAttackCounter(shouldSet);
        }
    }

    [ClientRpc]
    private void RpcAttackCounter(bool shouldSet)
    {
        if (isClientOnly)
        {
            if(shouldSet)
            {
                enemyList[0].specialAttackCounter = 0;
            }
            else
            {
                enemyList[0].specialAttackCounter++;
            }
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdAttackCounter(bool shouldSet)
    {
        if (shouldSet)
        {
            enemyList[0].specialAttackCounter = 0;
        }
        else
        {
            enemyList[0].specialAttackCounter++;
        }
    }

    public void EnemyGenerationNest()
    {
        enemyGeneratorAccess.GenerateEnemies(1, true);
        if (isServer)
        {
            RpcGenerateEnemy();
        }
        else
        {
            CmdGenerateEnemy();
        }
    }

    [ClientRpc]
    private void RpcGenerateEnemy()
    {
        if (isClientOnly)
        {
            enemyGeneratorAccess.GenerateEnemies(1, true);
        }
    }
    [Command(requiresAuthority = false)]
    private void CmdGenerateEnemy()
    {
        enemyGeneratorAccess.GenerateEnemies(1, true);
    }

    public void HealEnemyRefereeScript()
    {
        if (isServer)
        {
            playerList[0].HealEnemyPlayerScript();
        }
        else
        {
            playerList[1].HealEnemyPlayerScript();
        }
    }

    public bool AreAllPlayersDead()
    {
        bool areAllPlayersDead = true;
        foreach (PlayerScript player in playerList)
        {
            if (player.isPlayerAlive)
            {
                areAllPlayersDead = false;
            }
        }
        return areAllPlayersDead;
    }

    private void ResetDamageMultiplier()
    {
        if (isServer)
        {
            RpcResetDamageMultiplier();
        }
        else
        {
            CmdResetDamageMultiplier();
        }
    }

    [ClientRpc]
    private void RpcResetDamageMultiplier()
    {
        
        if (isClientOnly)
        {
            playerList[1].transform.root.GetComponentInChildren<PlayerStatScript>().damageMultiplier = 1;
            playerList[1].transform.root.GetComponentInChildren<UiScript>().DestroyIcon(26, 1);
        }
        else if(singlePlayerMode)
        {
            playerList[0].transform.root.GetComponentInChildren<PlayerStatScript>().damageMultiplier = 1;
            playerList[0].transform.root.GetComponentInChildren<UiScript>().DestroyIcon(26, 1);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdResetDamageMultiplier()
    {
        playerList[0].transform.root.GetComponentInChildren<PlayerStatScript>().damageMultiplier = 1;
        playerList[0].transform.root.GetComponentInChildren<UiScript>().DestroyIcon(26, 1);
    }

    public void Test()
    {
        waveCount++;
    }
}
