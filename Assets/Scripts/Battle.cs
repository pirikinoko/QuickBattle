using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Battle : MonoBehaviour
{
    MousePointer mousePointer;
    public bool inBattle { get; set; }
    public static int phase;
    int highScore;
    public static bool turnOver = false;
    public Sprite enemySprite { get; set; }
    [SerializeField] Sprite fire, regen;
    [SerializeField] GameObject battlePanel, battlePanelFront, resultPanel, atkButton, defButton, skillButton, nextButton, backButton, rewardButton, skillInfoFrame;
    [SerializeField] GameObject[] skillSlot, skillSlotMini;
    [SerializeField] SpriteRenderer spriteRenderer, playerState, enemyState;
    [SerializeField] Text textLog, playerHP, playerMP, playerATK, enemyName, enemyHP, enemyMP, enemyATK,  infoName, infoMP, infoText, highScoreText;
    [SerializeField] Text[] rewardText = new Text[3];
    [SerializeField] Text[] skillText = new Text[6];
    [SerializeField] Text[] skillInStats = new Text[6];
    public int count = 0;
    public string infoTarget { get; set; }
    public static int rewardSelect;
    bool gameEnd;
    int turn;
    int rnd;
    int killed;
    int damageGive, damageTake;
    //プレイヤー
    int pHP = 15;
    int pMP = 15;
    int pATK = 2;
    int pChargedTurn;
    int resetAtk = 2;
    bool pCharge = false;
    bool pStan;
    bool deffence = false;
    string pState;
    //敵
    int eHP, eMP, eATK;
    int eChargedTurn;
    string  eSkill1, eSkill2;
    string enemyAct;
    string eState;
    public string eName { get; set; }
    bool eCharge = false;
    bool eStan;
    public static string act;
    //スキル
    string skillDesctiption;
    string[] skills = new string[6];
    int skillCount;
    public static int skillClicked;
    //エフェクト,アニメーション
    [SerializeField] GameObject particle1, particle2, anim1;
    [SerializeField] Animator slashAnim, biteAnim, fireAnim, darkAnim;
    //リザルト用
   
    // Start is called before the first frame update
    void Start()
    {
        //ハイスコアロード
        //highScore = PlayerPrefs.GetInt("highScore", 0);
        highScoreText.text = "HighScore: " + highScore;

        //初期化処理
        battlePanel.SetActive(false);
        battlePanelFront.SetActive(false);
        resultPanel.SetActive(false);
        atkButton.SetActive(false);
        defButton.SetActive(false);
        skillButton.SetActive(false);
        nextButton.SetActive(false);
        backButton.SetActive(false);
        rewardButton.SetActive(false);
        skillInfoFrame.SetActive(false);
        gameEnd = false;
        infoMP.text = null;
        infoName.text = null;
        infoText.text = null;
        mousePointer = GameObject.Find("MousePoiner").GetComponent<MousePointer>();
        skills[0] = "ヒール";
        skillCount = 0;
        killed = 0;
        textLog.text = null;
        for (int i = 0; i < 6; i++) 
        {
            if (skills[i] != null) { skillCount++; }
            skillSlot[i].SetActive(false);
            skillSlotMini[i].SetActive(false);  
            skillText[i].text = skills[i];
            skillInStats[i].text = null;
        }
        for (int i = 0; i < skillCount; i++)
        {
            skillSlotMini[i].SetActive(true);
            skillInStats[i].text = skills[i];
        }
        phase = 0;
        //BGM
        BGM.musicTrigger[0] = true;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤー&敵ステータス表示
        playerHP.text = pHP.ToString();
        playerMP.text = pMP.ToString();
        playerATK.text = pATK.ToString();
        if (pState == "Fire") { playerState.sprite = fire; }　　//状態異常表示
        else if (pState == "Regen") { playerState.sprite = regen; }
        else { playerState.sprite = null; }
        if (inBattle)
        {
            StartBattle();
            BattleSystem();
            enemyName.text = eName;
            enemyHP.text = eHP.ToString();
            enemyMP.text = eMP.ToString();
            enemyATK.text = eATK.ToString();
            if (eState == "Fire") { enemyState.sprite = fire; }
            else if (eState == "Regen") { enemyState.sprite = regen; }
            else { enemyState.sprite = null; }
        }
        infoTarget = null;
        for (int i = 0; i < 6; i++)  //スキル説明ウィンドウ
        {
            if (phase != 8)
            {
                if (mousePointer.colName != null && mousePointer.colName.Contains((i + 1).ToString()))
                {
                    infoTarget = skills[i];
                }
            }
            else
            {
                if (mousePointer.colName != null && mousePointer.colName.Contains((i + 1).ToString()))
                {
                    if(i == 0) { infoTarget = eSkill1; }
                    else if (i == 1) { infoTarget = eSkill2; }
                }
            }
        }
        ShowSkillInfo(infoTarget);

        if(!(inBattle && gameEnd))
        {
            StartCoroutine(ShowResult());
            gameEnd = false;
        }
    }

    void StartBattle()　//初期化
    {
        if (count == 0)　
        {        
            battlePanel.SetActive(true);
            battlePanelFront.SetActive(true);
            turn = 1;
            count = 1;
            phase = 1;
            damageGive = 0;
            damageTake = 0;                  
            deffence = false;
            pCharge = false;
            eCharge = false;
            turnOver = false;
            pStan = false;
            eStan = false;
            pState = null;
            eState = null;
            skillClicked = 999;
            rewardSelect = 999;
            spriteRenderer.sprite = enemySprite;
            SetEnemy(enemySprite.name);
            //BGM
            if (eName == "魔王")
            {
                BGM.musicTrigger[2] = true;
            }
            else { BGM.musicTrigger[1] = true; }
        }
    }

    void BattleSystem()
    {
        switch (phase)
        {
            case 1:
                Phase1();
                break;
            case 2:
                Phase2();
                break;
            case 3:
                Phase3();
                break;
            case 4:
                Phase4();
                break;
            case 5:
                Phase5();
                break;
            case 6:
                Phase6();
                break;
            case 7:
                Phase7();
                break;
            case 8:
                Phase8();
                break;
            case 9:
                Phase9();
                break;
            case 10:
                Phase10();
                break;
            case 11:
               Phase11(); 
                break;
            case 12:
               Phase12();   
                break;
            case 13:
                Phase13();
                break;
        }
    }
    void Phase1()  //プレイヤー行動選択
    {
        if (pStan == true) { phase = 3; pStan = false; } //スタンしているなら自分のターンをスキップし敵のターン
        if (!(turnOver))
        {
            act = null;
            deffence = false;
            if (pCharge && turn > pChargedTurn + 1) { pCharge = false; }
            if (eCharge && turn > eChargedTurn + 1) { eCharge = false; }
            textLog.text = "行動を選択してください";
            //行動ボタンアクティブ()
            atkButton.SetActive(true);
            defButton.SetActive(true);
            skillButton.SetActive(true);
            nextButton.SetActive(false);
            backButton.SetActive(false);
            damageGive = 0;
            damageTake = 0;
            for (int i = 0; i < skillSlot.Length; i++)
            {
                skillSlot[i].SetActive(false);
            }
            turnOver = true;
        }
        
    }
    void Phase2()  //プレイヤー行動反映
    {
        if (!(turnOver))
        {
            atkButton.SetActive(false);
            defButton.SetActive(false);
            skillButton.SetActive(false);
            switch (act)
            {
                case "ATK":
                    damageGive = pATK;
                    if (pCharge) { damageGive *= 2; }
                    textLog.text = string.Format("{0}に{1}ダメージ!", eName, damageGive);
                    eHP -= damageGive;
                    SoundEffect.SETrigger[1] = true;
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    nextButton.SetActive(true);
                    break;

                case "DEF":
                    textLog.text = "守りを固めている";
                    deffence = true;
                    SoundEffect.SETrigger[2] = true;
                    nextButton.SetActive(true);
                    break;

                case "SKL":
                    textLog.text = "特技を選択してください";
                    for (int i = 0; i < skillCount; i++)
                    {
                        skillSlot[i].SetActive(true);
                    }
                    backButton.SetActive(true);

                    break;
                case "Back":
                    for (int i = 0; i < skillCount; i++)
                    {
                        skillSlot[i].SetActive(false);
                    }
                    phase = 1;
                    return;

            }
            turnOver = true;
        }
        if (act == "SKL")
        {
            if (skillClicked != 999)
            {
                if (CheckMP(skills[skillClicked]) <= pMP)
                {
                    Skill(skills[skillClicked], "Player");
                    skillClicked = 999;
                    nextButton.SetActive(true);
                    backButton.SetActive(false);
                    for (int i = 0; i < skillSlot.Length; i++)
                    {
                        skillSlot[i].SetActive(false);
                    }
                    act = null;

                }
                else
                {
                    skillClicked = 999;
                    textLog.text = "MPが不足しています";
                }
            }
            if (turnOver && Input.GetMouseButtonDown(0)) { turnOver = false; }
        }



    }
    void Phase3() //敵行動宣言
    {
        if (eStan == true) { phase = 1; eStan = false; }

        else if (eHP <= 0)
        {
            if (eName == "魔王")
            {
                gameEnd = true;
            }
            phase = 7;
            turnOver = false;
            killed++;
        }
        else if (!(turnOver))
        {
            damageGive = 0;
            damageTake = 0;
            EnemyAct();
            SoundEffect.SETrigger[10] = true;

            switch (enemyAct)
            {
                case "Attack":
                    textLog.text = string.Format("{0}の攻撃!", eName);

                    break;

                case "Skill1":
                    textLog.text = string.Format("{0}の{1}!", eName, eSkill1);
                    break;

                case "Skill2":
                    textLog.text = string.Format("{0}の{1}!", eName, eSkill2);
                    break;

            }
            turnOver = true;
        }
    }
    void Phase4()
    {
        if (pHP <= 0) { phase = 11; }
        else if (!(turnOver))
        {
            switch (enemyAct)
            {
                case "Attack":
                    damageTake = eATK;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    damageTake = System.Math.Max(damageTake, 1);
                    textLog.text = string.Format("{0}ダメージ受けた", damageTake);
                    pHP -= damageTake;
                    SoundEffect.SETrigger[0] = true;
                    PlayPaticle(particle1);
                    break;

                case "Skill1":
                    Skill(eSkill1, "Enemy");
                    break;

                case "Skill2":
                    Skill(eSkill2, "Enemy");
                    break;

            }
            turnOver = true;
        }
            
    }
    void Phase5()
    {
        if (pHP <= 0) { phase = 11; turnOver = false; }
        else if (!turnOver)
        {
            if (pState == "Fire")
            {
                pHP -= 1;
                textLog.text = "やけどのダメージ!";
                SoundEffect.SETrigger[0] = true;
                PlayPaticle(particle1);
                turnOver = true;
            }
            else if (pState == "Regen")
            {
                pHP += 1;
                textLog.text = "手当で1HP回復した";
                SoundEffect.SETrigger[4] = true;
                turnOver = true;
            }
            else { phase++; }
        }       
    }
    void Phase6()
    {
        if (eHP <= 0)
        {
            if (eName == "魔王")
            {
                gameEnd = true;
            }
            phase = 7;
            turnOver = false;
            killed++;
        }
        else if (!turnOver)
        {
            if (eState == "Fire")
            {
                eHP -= 1;
                textLog.text = "敵にやけどのダメージ!";
                SoundEffect.SETrigger[0] = true;
                turnOver = true;
            }
            else if (eState == "Regen")
            {
                eHP += 1;
                textLog.text = "敵は手当で1HP回復した";
                SoundEffect.SETrigger[4] = true;
                turnOver = true;
            }
            else { phase = 1; }
            turn++;
        }
           

    }
    void Phase7()
    {
        if (!(turnOver))
        {
            textLog.text = "敵を倒した!";
            pATK = resetAtk;
            spriteRenderer.sprite = null;
            turnOver = true;
            SoundEffect.SETrigger[9] = true;
        }      
    }

    
    void Phase8()
    {
        nextButton.SetActive(false);
        rewardButton.SetActive(true);
        //報酬ランダム設定
        if (!(turnOver))
        {
            textLog.text = "報酬を選択して下さい";
            rewardText[0].text = eSkill1;
            rewardText[1].text = eSkill2;
            rnd = Random.Range(1, 7); //1～5の整数
            switch (rnd)
            {
                case 1:
                    rewardText[2].text = "HP+10";
                    break;
                case 2:
                    rewardText[2].text = "MP+10";
                    break;
                case 3:
                    rewardText[2].text = "HP&MP+7";
                    break;
                case 4:
                    rewardText[2].text = "ATK+1";
                    break;
                case 5:
                    rewardText[2].text = "HP&MP+10";
                    break;
                case 6:
                    rewardText[2].text = "ATK+1";
                    break;

            }
        }
    
        turnOver = true;
    }
    void Phase9()
    {
        nextButton.SetActive(true);
        rewardButton.SetActive(false);
        switch (rewardSelect)
        {
            case 0:
                skills[skillCount] = eSkill1;
                skillText[skillCount].text = eSkill1;
                skillCount++;          
                textLog.text = eSkill1 + "を取得しました";
                skillSlotMini[skillCount - 1].SetActive(true);
                skillInStats[skillCount - 1].text = eSkill1;
                rewardSelect = 999;
                break;
            case 1:
                skills[skillCount] = eSkill2;
                skillText[skillCount].text = eSkill2;
                skillCount++;           
                textLog.text = eSkill2 + "を取得しました";
                skillSlotMini[skillCount - 1].SetActive(true);
                skillInStats[skillCount - 1].text = eSkill2;
                rewardSelect = 999;
                break;
            case 2:
                Reward3();
                rewardSelect = 999;
                break;
        }
                                                                                

    }
    void Phase10()
    {
        inBattle = false;
        battlePanel.SetActive(false);
        battlePanelFront.SetActive(false);
        count = 0;
        pState = null;
        BGM.musicTrigger[0] = true;
    }
    void Phase11() //プレイヤー死亡
    {
        if (!turnOver)
        {
            textLog.text = "プレイヤーが死亡しました";
            nextButton.SetActive(true);
            turnOver = true;
        }         
    }
    void Phase12()
    {
        SceneManager.LoadScene("Main");
    }

    void Phase13() //クリアリザルト
    {
        textLog.text = "ゲームをクリアしました!";
        nextButton.SetActive(true);
    }

    void SetEnemy(string spriteName)　//敵の情報登録
    {
        switch (spriteName)
        {
            case "GreenMush":
                eName = "緑キノコ";
                eHP = 7;
                eMP = 5;
                eATK = 1;
                eSkill1 = "帽子投げ";
                eSkill2 = "サボる";
                break;
            case "Pumpking":
                eName = "かぼちゃおばけ";
                eHP = 8;
                eMP = 3;
                eATK = 1;
                eSkill1 = "かみつく";
                eSkill2 = "つまみ食い";
                break;
            case "AxeMan":
                eName = "木こり";
                eHP = 18;
                eMP = 5;
                eATK = 3;
                eSkill1 = "振り上げる";
                eSkill2 = "蹴る";
                break;
            case "Bat":
                eName = "コウモリ";
                eHP = 15;
                eMP = 9;
                eATK = 2;
                eSkill1 = "ドレイン";
                eSkill2 = "頭突き";
                break;
            case "BushiFlame":
                eName = "炎の剣士";
                eHP = 12;
                eMP = 5;
                eATK = 1;
                eSkill1 = "炎の刃";
                eSkill2 = "筋トレパンチ";
                break;
            case "Cook":
                eName = "料理人";
                eHP = 10;
                eMP = 6;
                eATK = 1;
                eSkill1 = "揚げたて";
                eSkill2 = "つまみ食い";
                break;
            case "Dullahan":
                eName = "デュラハン";
                eHP = 20;
                eMP = 10;
                eATK = 2;
                eSkill1 = "ハイヒール";
                eSkill2 = "魔導弾"; 
                break;
            case "Knight":
                eName = "騎士";
                eHP = 20;
                eMP = 8;
                eATK = 2;
                eSkill1 = "スラッシュ";
                eSkill2 = "炎の刃";
                break;
            case "Magician":
                eName = "魔法使い";
                eHP = 15;
                eMP = 8;
                eATK = 1;
                eSkill1 = "ファイア";
                eSkill2 = "チャージ";
                break;
            case "Satan":
                eName = "魔王";
                eHP = 30;
                eMP = 12;
                eATK = 3;
                eSkill1 = "魔導弾";
                eSkill2 = "ドレイン";
                break;
            case "Slime":
                eName = "スライム";
                eHP = 8;
                eMP = 5;
                eATK = 1;
                eSkill1 = "舐める";
                eSkill2 = "手当て";
                break;
            case "ShieldMan":
                eName = "盾使い";
                eHP = 20;
                eMP = 8;
                eATK = 1;
                eSkill1 = "スタンアタック";
                eSkill2 = "蹴る";
                break;
        }
    }

    void EnemyAct()　　//敵の行動設定
    {
        if (eName == "緑キノコ")
        {
            if(turn == 1)
            {
                enemyAct = "Skill2";
            }
            else if (eMP >= 1)
            {
                enemyAct = "Skill1";
            }
            else
            {
                enemyAct = "Attack";
            }
        }
        else if (eName == "かぼちゃおばけ")
        {
            if(turn == 3 && eMP >= 1)
            {
                enemyAct = "Skill2";
            }
            else if (eMP >= 2)
            {
                enemyAct = "Skill1";
            }
            else
            {
                enemyAct = "Attack";
            }
        }
        else if (eName == "木こり")
        {
            rnd = Random.Range(1, 4);
            if (rnd == 2 && CheckMP(eSkill1) <= eMP　&& eCharge == false) { enemyAct = "Skill1"; }
            else if (rnd == 3 && CheckMP(eSkill2) <= eMP) { enemyAct = "Skill2"; }
            else { enemyAct = "Attack"; }
        }
        else if (eName == "デュラハン")
        {
            rnd = Random.Range(1, 4);
            if (rnd == 2 && CheckMP(eSkill1) <= eMP && eHP < 7) { enemyAct = "Skill1"; }
            else if (rnd == 3 && CheckMP(eSkill2) <= eMP) { enemyAct = "Skill2"; }
            else { enemyAct = "Attack"; }
        }
        else if (eName == "魔法使い")
        {
            rnd = Random.Range(1, 4);
            if(turn == 1 && CheckMP(eSkill2) <= eMP) { enemyAct = "Skill2"; }
            else if (rnd == 2 && CheckMP(eSkill1) <= eMP) { enemyAct = "Skill1"; }
            else if (rnd == 3 && CheckMP(eSkill2) <= eMP) { enemyAct = "Skill2"; }
            else { enemyAct = "Attack"; }
        }
        else 
        {
            rnd = Random.Range(1, 4);           
            if(rnd == 2 && CheckMP(eSkill1) <= eMP) { enemyAct = "Skill1"; }
            else if (rnd == 3 && CheckMP(eSkill2) <= eMP) { enemyAct = "Skill2"; }
            else { enemyAct = "Attack"; }
        }

    }

    

    int CheckMP(string skillName) //MPが足りているかチェック
    {
        int costMP = 0;
        switch (skillName)
        {
            case "帽子投げ":
                costMP = 1;
                break;
            case "ヒール":
                costMP = 2;
                break;
            case "サボる":
                costMP = 0;
                break;
            case "かみつく":
                costMP = 2;
                break;
            case "筋トレパンチ":
                costMP = 2;
                break;
            case "炎の刃":
                costMP = 2;
                break;
            case "揚げたて":
                costMP = 2;
                break;
            case "スタンアタック":
                costMP = 4;
                break;
            case "ドレイン":
                costMP = 4;
                break;
            case "蹴る":
                costMP = 0;
                break;
            case "頭突き":
                costMP = 1;
                break;
            case "舐める":
                costMP = 1;
                break;
            case "スラッシュ":
                costMP = 3;
                break;
            case "ファイア":
                costMP = 4;
                break;
            case "魔導弾":
                costMP = 5;
                break;
            case "ハイヒール":
                costMP = 5;
                break;
            case "手当て":
                costMP = 2;
                break;
            case "チャージ":
                costMP = 3;
                break;
            case "振り上げる":
                costMP = 0;
                break;
            case "つまみ食い":
                costMP = 1;
                break;
        }
        return costMP;

    }
    void Skill(string skillName, string user)
    {
        int numSE = 0;
        int costMP = 1;
        switch (skillName)
        {

            case "帽子投げ":
                costMP = 1;
                if (user == "Player")  //プレイヤー　→　モンスター　の時
                {

                    damageGive = pATK + 1;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}ダメージ!", damageGive);
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    numSE = 1;

                }
                else
                {
                    damageTake = eATK + 1;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    skillDesctiption = string.Format("{0}ダメージ!", damageTake);
                    numSE = 0;
                    PlayPaticle(particle1);
                }
                break;

            case "筋トレパンチ":
                costMP = 2;
                if (user == "Player")  //プレイヤー　→　モンスター　の時
                {

                    damageGive = pATK + 1;
                    pATK += 1;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}ダメージ!攻撃力が1上昇", damageGive);
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    numSE = 1;

                }
                else
                {
                    damageTake = eATK + 1;
                    eATK += 1;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    skillDesctiption = string.Format("{0}ダメージ!敵の攻撃力が1上昇", damageTake);
                    numSE = 0;
                    PlayPaticle(particle1);
                }
                break;

            case "炎の刃":
                costMP = 2;
                rnd = Random.Range(1, 3);
                if (user == "Player")  //プレイヤー　→　モンスター　の時
                {

                    damageGive = pATK + 3;
                    if (pCharge) { damageGive *= 2; }
                    if (rnd == 2) { eState = "Fire"; skillDesctiption = string.Format("{0}ダメージ!,敵を火傷させた!", damageGive); }
                    else { skillDesctiption = string.Format("{0}ダメージ!", damageGive); }
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    numSE = 1;

                }
                else
                {
                    damageTake = eATK + 3;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    if (rnd == 2) { pState = "Fire"; skillDesctiption = string.Format("{0}ダメージ!,火傷してしまった!", damageTake); }
                    else { skillDesctiption = string.Format("{0}ダメージ!", damageTake); }
                    numSE = 0;
                    PlayPaticle(particle1);
                }
                break;

            case "揚げたて":
                costMP = 2;
                if (user == "Player")  //プレイヤー　→　モンスター　の時
                {

                    damageGive = pATK + 1;
                    if (pCharge) { damageGive *= 2; }
                    eState = "Fire"; 
                    skillDesctiption = string.Format("{0}ダメージ!,敵を火傷させた!", damageGive);
                    fireAnim.SetTrigger("On");
                    fireAnim.SetTrigger("Off");
                    numSE = 1;

                }
                else
                {
                    damageTake = eATK + 1;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    pState = "Fire"; 
            skillDesctiption = string.Format("{0}ダメージ!,火傷してしまった!", damageTake);
                    numSE = 0;
                    PlayPaticle(particle1);
                }
                break;

            case "スタンアタック":
                costMP = 4;
                rnd = Random.Range(1, 3);
                if (user == "Player")  //プレイヤー　→　モンスター　の時
                {
                    damageGive = pATK + 3;
                    if (pCharge) { damageGive *= 2; }
                    if (rnd == 2) { eStan = true; skillDesctiption = string.Format("{0}ダメージ!,敵をスタンさせた!", damageGive); }
                    else { skillDesctiption = string.Format("{0}ダメージ!", damageGive); }
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    numSE = 1;

                }
                else
                {
                    damageTake = eATK + 1;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    if (rnd == 2) { pStan = true; skillDesctiption = string.Format("{0}ダメージ!,スタンしてしまった!", damageTake); }
                    else { skillDesctiption = string.Format("{0}ダメージ!", damageTake); }
                    numSE = 0;
                    PlayPaticle(particle1);
                }
                break;

            case "かみつく":
                costMP = 2;
                if (user == "Player")  //プレイヤー　→　モンスター　の時
                {

                    damageGive = pATK + 1;
                    pHP += 1;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}ダメージ&HPが1回復!", damageGive);
                    biteAnim.SetTrigger("On");
                    biteAnim.SetTrigger("Off");
                    numSE = 3;

                }
                else
                {
                    damageTake = eATK + 1;
                    eHP += 1;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    skillDesctiption = string.Format("{0}ダメージ&敵のHPが1回復!", damageTake);
                    numSE = 3;
                    PlayPaticle(particle1);
                }
                break;

            case "ドレイン":
                costMP = 4;
                if (user == "Player")  //プレイヤー　→　モンスター　の時
                {

                    damageGive = pATK + 2;
                    if (pCharge) { damageGive *= 2; }
                    pHP += damageGive;
                    skillDesctiption = string.Format("{0}ダメージ,HPを吸収した", damageGive);
                    darkAnim.SetTrigger("On");
                    darkAnim.SetTrigger("Off");
                    numSE = 7;

                }
                else
                {
                    damageTake = eATK + 2;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    eHP += damageTake;
                    skillDesctiption = string.Format("{0}ダメージ,HPを吸収されてしまった", damageTake);
                    numSE = 7;
                    PlayPaticle(particle1);
                }
                break;

            case "蹴る":
                costMP = 0;
               
                if (user == "Player")
                {

                    damageGive = pATK + 2;
                    pHP -= 1;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}ダメージ,反動をうけた", damageGive);
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    numSE = 1;

                }
                else
                {
                    damageTake = eATK + 2;
                    eHP -= 1;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    skillDesctiption = string.Format("{0}ダメージ,敵は反動をうけた", damageTake);
                    PlayPaticle(particle1);
                    numSE = 0;
                }
                break;

            case "頭突き":
                costMP = 1;
                if (user == "Player")
                {

                    damageGive = pATK + 5;
                    pHP -= 1;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}ダメージ,反動をうけた", damageGive);
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    numSE = 1;

                }
                else
                {
                    damageTake = eATK + 5;
                    eHP -= 1;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    skillDesctiption = string.Format("{0}ダメージ,敵は反動をうけた", damageTake);
                    PlayPaticle(particle1);
                    numSE = 0;
                }
                break;
            case "舐める":
                costMP = 1;
                if (user == "Player")
                {

                    damageGive = pATK + 1;
                    eMP -= 1;
                    if (pCharge) { damageGive *= 2; }
                    if (eMP < 0) { eMP = 0; }
                    skillDesctiption = string.Format("{0}ダメージ,MPを1下げた！", damageGive);
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    numSE = 3;

                }
                else
                {
                    damageTake = eATK + 1;
                    pMP -= 1;
                    if (pMP < 0) { pMP = 0; }
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    skillDesctiption = string.Format("{0}ダメージ,MPを1下げられた！", damageTake);
                    numSE = 3;
                    PlayPaticle(particle1);
                }
                break;

            case "スラッシュ":
                costMP = 3;
                if (user == "Player")
                {

                    damageGive = pATK + 5;
                    eMP -= 3;
                    if (pCharge) { damageGive *= 2; }
                    if (eMP < 0) { eMP = 0; }
                    skillDesctiption = string.Format("{0}ダメージ,MPを3下げた！", damageGive);
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    numSE = 3;

                }
                else
                {
                    damageTake = eATK + 5;
                    pMP -= 3;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    if (pMP < 0) { pMP = 0; }
                    skillDesctiption = string.Format("{0}ダメージ,MPを3下げられた！", damageTake);
                    numSE = 3;
                    PlayPaticle(particle1);
                }
                break;

            case "ファイア":
                costMP = 4;
                if (user == "Player")
                {

                    damageGive = pATK * 2;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}ダメージ！", damageGive);
                    fireAnim.SetTrigger("On");
                    fireAnim.SetTrigger("Off");
                    numSE = 7;

                }
                else
                {
                    damageTake = eATK * 2;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    skillDesctiption = string.Format("{0}ダメージ！", damageTake);
                    numSE = 7;
                    PlayPaticle(particle1);
                }
                break;

            case "魔導弾":
                costMP = 5;
                if (user == "Player")
                {

                    damageGive = pATK + 7;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}ダメージ！", damageGive);
                    darkAnim.SetTrigger("On");
                    darkAnim.SetTrigger("Off");
                    numSE = 7;

                }
                else
                {
                    damageTake = eATK + 7;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    skillDesctiption = string.Format("{0}ダメージ！", damageTake);
                    numSE = 7;
                    PlayPaticle(particle1);
                }
                break;

            case "ヒール":
                costMP = 2;
                numSE = 6;
                if (user == "Player")
                {
                    damageTake = -8;
                    skillDesctiption = string.Format("{0}回復した!", -damageTake);
                    PlayPaticle(particle2);
                }
                else if (user == "Enemy")
                {
                    damageGive = -8;
                    skillDesctiption = string.Format("敵は{0}回復した!", -damageGive);
                }
                break;

            case "ハイヒール":
                costMP = 5;
                numSE = 6;
                if (user == "Player")
                {
                    damageTake = -15;
                    skillDesctiption = string.Format("{0}回復した!", -damageTake);
                    PlayPaticle(particle2);
                }
                else if (user == "Enemy")
                {
                    damageGive = -15;
                    skillDesctiption = string.Format("敵は{0}回復した!", -damageGive);
                }
                break;


            case "手当て":
                costMP = 2;
                numSE = 4;
                if (user == "Player")
                {
                    pState = "Regen";
                    skillDesctiption = string.Format("手当てしている");
                }
                else if (user == "Enemy")
                {
                    eState = "Regen";
                    skillDesctiption = string.Format("敵は手当てしている");
                }
                break;
            case "チャージ":
                costMP = 3;
                numSE = 6;
                if (user == "Player")
                {
                    pATK += 2;
                    skillDesctiption = string.Format("攻撃力が2上昇");
                }
                else if (user == "Enemy")
                {
                    eATK += 2;
                    skillDesctiption = string.Format("敵の攻撃力が2上昇");
                }
                break;

            case "振り上げる":
                costMP = 0;
                numSE = 4;
                if (user == "Player")
                {
                    pCharge = true;
                    pChargedTurn = turn;
                    skillDesctiption = string.Format("振り上げている");
                }
                else if (user == "Enemy")
                {
                    eCharge = true;
                    eChargedTurn = turn;
                    skillDesctiption = string.Format("敵は振り上げている");
                }
                break;

            case "つまみ食い":
                costMP = 1;
                numSE = 3;
                rnd = Random.Range(1, 3);
                if (user == "Player")
                {
                    damageTake = -6;
                    skillDesctiption = string.Format("{0}回復した!", -damageTake);
                    if (rnd == 2) { pState = "Fire"; skillDesctiption = string.Format("{0}回復したが火傷してしまった", -damageTake); }
                    Instantiate(particle2, new Vector2(0, 0), Quaternion.identity);
                }
                else if (user == "Enemy")
                {
                    damageGive = -6;
                    skillDesctiption = string.Format("{0}回復した!", -damageGive);
                    if (rnd == 2) { eState = "Fire"; skillDesctiption = string.Format("敵は{0}回復したが火傷してしまった", -damageGive); }
                }
                break;
            case "サボる":
                rnd = Random.Range(1, 4);
                numSE = 4;
                costMP = 0;
                if (user == "Player")
                {
                    if(rnd == 3)
                    {
                        pHP += 1;
                        pMP += 1;
                        skillDesctiption = ("HPとMPが1回復した!");
                    }
                    else { skillDesctiption = ("サボっている!"); }

                }
                else if (user == "Enemy")
                {
                    if (rnd == 3)
                    {
                        eHP += 1;
                        eMP += 1;
                        skillDesctiption = ("敵のHPとMPが1回復した!");
                    }
                    else { skillDesctiption = ("サボっている!"); }
                }

                break;
        }

        if (deffence && damageTake != 0) { damageTake = System.Math.Max(damageTake, 1); }  //防御時最小は１ダメージ
        pHP -= damageTake; eHP -= damageGive;　　　
        if (user == "Player") { pMP -= costMP; }
        else { eMP -= costMP; }
        SoundEffect.SETrigger[numSE] = true;
        textLog.text = skillDesctiption;

    }　//スキル使用
    void PlayPaticle(GameObject particle)//パーティクル生成
    {
        Instantiate(particle, new Vector2(0, 0), Quaternion.identity); 
    }
    void ShowSkillInfo(string skillName)
    {
        if(skillName != null)
        {
            skillInfoFrame.SetActive(true);
            infoName.text = skillName;
            int costMP = 0;
            switch (skillName)
            {
                case "帽子投げ":                 
                    infoText.text = "攻撃力 + 1のダメージ";
                    break;
                case "ヒール":
                    infoText.text = "8HP回復";    
                    break;
                case "サボる":
                    infoText.text = "たまにHPとMPが1回復する";
                    break;
                case "かみつく":
                    infoText.text = "攻撃力 + 1のダメージ,1HP回復";
                    break;
                case "つまみ食い":
                    infoText.text = "6HP回復するが,確率で火傷";
                    break;
                case "筋トレパンチ":
                    infoText.text = "攻撃力 + 1のダメージ,攻撃力1増加";
                    break;
                case "炎の刃":
                    infoText.text = "攻撃力 + 3のダメージ,確率で火傷";
                    break;
                case "揚げたて":
                    infoText.text = "攻撃力 + 1のダメージ,火傷付与";
                    break;
                case "スタンアタック":
                    infoText.text = "攻撃力 + 3のダメージ,確率でスタン付与";
                    break;
                case "ドレイン":
                    infoText.text = "攻撃力 + 3のダメージ,与えたダメージの分回復する";
                    break;
                case "蹴る":
                    infoText.text = "攻撃力 + 2のダメージ,反動を受ける";
                    break;
                case "頭突き":
                    infoText.text = "攻撃力 + 5のダメージ,反動を受ける";
                    break;
                case "舐める":
                    infoText.text = "攻撃力 + 1のダメージ,MPを1削る";
                    break;
                case "スラッシュ":
                    infoText.text = "攻撃力 + 5のダメージ,MPを3削る";
                    break;
                case "ファイア":
                    infoText.text = "攻撃力 × 2のダメージ";
                    break;
                case "魔道弾":
                    infoText.text = "攻撃力 + 7のダメージ,MPを3削る";
                    break;
                case "ハイヒール":
                    infoText.text = "15HP回復する";
                    break;
                case "手当て":
                    infoText.text = "毎ターン1HP回復する,火傷を直せる";
                    break;
                case "チャージ":
                    infoText.text = "攻撃力が2増加(戦闘中)";
                    break;
                case "振り上げる":
                    infoText.text = "次のターンの攻撃が2倍のダメージ";
                    break;
            }
            costMP = CheckMP(skillName);
            infoMP.text = "消費MP: " + costMP.ToString();
        }
        else
        {
            skillInfoFrame.SetActive(false);
            infoMP.text = null;
            infoName.text = null;
            infoText.text = null;
        }
      
    }
    void Reward3() 　　//3つ目の報酬の設定
    {
        switch (rnd)
        {
            case 1:
                pHP += 10;
                textLog.text = "HPが10回復した!";
                break;
            case 2:
                pMP += 10;
                textLog.text = "MPが10回復した!";
                break;
            case 3:
                pHP += 7; pMP += 7;
                textLog.text = "HPとMPが7回復した!";
                break;
            case 4:
                pATK += 1;
                resetAtk += 1;
                textLog.text = "攻撃力が1増加した!";
                break;
            case 5:
                pHP += 10; pMP += 10;
                textLog.text = "HPとMPが10回復した!";
                break;
            case 6:
                pATK += 1;
                resetAtk += 1;
                textLog.text = "攻撃力が1増加した!";
                break;

        }
    }

    [SerializeField] Text finalHP, finalMP, finalATK, enemyKilled, finalScore;
    int showHP, showMP, showATK, showScore, num;
    IEnumerator ShowResult()　　//リザルト表示
    {
        if (gameEnd)
        {
            resultPanel.gameObject.SetActive(true);
            finalHP.text = null;
            finalMP.text = null;
            finalATK.text = null;
            finalScore.text = null;
            enemyKilled.text = null;
            showHP = showMP = showATK = showScore = 0;
            num = 0;

            yield return StartCoroutine(GainResultNumber(showHP, pHP, finalHP, "HP: "));
            yield return StartCoroutine(GainResultNumber(showMP, pMP, finalMP, "MP: "));
            yield return StartCoroutine(GainResultNumber(showATK, pATK, finalATK, "ATK: "));
            yield return new WaitForSeconds(1.0f);
            showScore = pHP * 10 + pMP * 10 + (pATK * 30);
            finalScore.text = "スコア: " + showScore;
            enemyKilled.text = "×"+killed + "kill";
            SoundEffect.SETrigger[10] = true;
            yield return new WaitForSeconds(2.0f);
            showScore *= killed;
            finalScore.text = "スコア: " + showScore;
            SoundEffect.SETrigger[8] = true;
            enemyKilled.text = null;

            highScore = showScore;
            //PlayerPrefs.SetInt("highScore", highScore);
        }
    }

    IEnumerator GainResultNumber(int target, int goal, Text targetText, string letter)
    {
        while(target < goal)
        {
            target++;
            yield return new WaitForSeconds(0.05f);
            targetText.text = letter + target;
        }
        if (target >= goal)
        {
            SoundEffect.SETrigger[8] = true;
            num++;
        }
    }　//数値をゆっくり近づける

   

}
