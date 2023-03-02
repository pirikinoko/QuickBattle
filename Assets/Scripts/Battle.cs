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
    //�v���C���[
    int pHP = 15;
    int pMP = 15;
    int pATK = 2;
    int pChargedTurn;
    int resetAtk = 2;
    bool pCharge = false;
    bool pStan;
    bool deffence = false;
    string pState;
    //�G
    int eHP, eMP, eATK;
    int eChargedTurn;
    string  eSkill1, eSkill2;
    string enemyAct;
    string eState;
    public string eName { get; set; }
    bool eCharge = false;
    bool eStan;
    public static string act;
    //�X�L��
    string skillDesctiption;
    string[] skills = new string[6];
    int skillCount;
    public static int skillClicked;
    //�G�t�F�N�g,�A�j���[�V����
    [SerializeField] GameObject particle1, particle2, anim1;
    [SerializeField] Animator slashAnim, biteAnim, fireAnim, darkAnim;
    //���U���g�p
   
    // Start is called before the first frame update
    void Start()
    {
        //�n�C�X�R�A���[�h
        //highScore = PlayerPrefs.GetInt("highScore", 0);
        highScoreText.text = "HighScore: " + highScore;

        //����������
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
        skills[0] = "�q�[��";
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
        //�v���C���[&�G�X�e�[�^�X�\��
        playerHP.text = pHP.ToString();
        playerMP.text = pMP.ToString();
        playerATK.text = pATK.ToString();
        if (pState == "Fire") { playerState.sprite = fire; }�@�@//��Ԉُ�\��
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
        for (int i = 0; i < 6; i++)  //�X�L�������E�B���h�E
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

    void StartBattle()�@//������
    {
        if (count == 0)�@
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
            if (eName == "����")
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
    void Phase1()  //�v���C���[�s���I��
    {
        if (pStan == true) { phase = 3; pStan = false; } //�X�^�����Ă���Ȃ玩���̃^�[�����X�L�b�v���G�̃^�[��
        if (!(turnOver))
        {
            act = null;
            deffence = false;
            if (pCharge && turn > pChargedTurn + 1) { pCharge = false; }
            if (eCharge && turn > eChargedTurn + 1) { eCharge = false; }
            textLog.text = "�s����I�����Ă�������";
            //�s���{�^���A�N�e�B�u()
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
    void Phase2()  //�v���C���[�s�����f
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
                    textLog.text = string.Format("{0}��{1}�_���[�W!", eName, damageGive);
                    eHP -= damageGive;
                    SoundEffect.SETrigger[1] = true;
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    nextButton.SetActive(true);
                    break;

                case "DEF":
                    textLog.text = "�����ł߂Ă���";
                    deffence = true;
                    SoundEffect.SETrigger[2] = true;
                    nextButton.SetActive(true);
                    break;

                case "SKL":
                    textLog.text = "���Z��I�����Ă�������";
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
                    textLog.text = "MP���s�����Ă��܂�";
                }
            }
            if (turnOver && Input.GetMouseButtonDown(0)) { turnOver = false; }
        }



    }
    void Phase3() //�G�s���錾
    {
        if (eStan == true) { phase = 1; eStan = false; }

        else if (eHP <= 0)
        {
            if (eName == "����")
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
                    textLog.text = string.Format("{0}�̍U��!", eName);

                    break;

                case "Skill1":
                    textLog.text = string.Format("{0}��{1}!", eName, eSkill1);
                    break;

                case "Skill2":
                    textLog.text = string.Format("{0}��{1}!", eName, eSkill2);
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
                    textLog.text = string.Format("{0}�_���[�W�󂯂�", damageTake);
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
                textLog.text = "�₯�ǂ̃_���[�W!";
                SoundEffect.SETrigger[0] = true;
                PlayPaticle(particle1);
                turnOver = true;
            }
            else if (pState == "Regen")
            {
                pHP += 1;
                textLog.text = "�蓖��1HP�񕜂���";
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
            if (eName == "����")
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
                textLog.text = "�G�ɂ₯�ǂ̃_���[�W!";
                SoundEffect.SETrigger[0] = true;
                turnOver = true;
            }
            else if (eState == "Regen")
            {
                eHP += 1;
                textLog.text = "�G�͎蓖��1HP�񕜂���";
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
            textLog.text = "�G��|����!";
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
        //��V�����_���ݒ�
        if (!(turnOver))
        {
            textLog.text = "��V��I�����ĉ�����";
            rewardText[0].text = eSkill1;
            rewardText[1].text = eSkill2;
            rnd = Random.Range(1, 7); //1�`5�̐���
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
                textLog.text = eSkill1 + "���擾���܂���";
                skillSlotMini[skillCount - 1].SetActive(true);
                skillInStats[skillCount - 1].text = eSkill1;
                rewardSelect = 999;
                break;
            case 1:
                skills[skillCount] = eSkill2;
                skillText[skillCount].text = eSkill2;
                skillCount++;           
                textLog.text = eSkill2 + "���擾���܂���";
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
    void Phase11() //�v���C���[���S
    {
        if (!turnOver)
        {
            textLog.text = "�v���C���[�����S���܂���";
            nextButton.SetActive(true);
            turnOver = true;
        }         
    }
    void Phase12()
    {
        SceneManager.LoadScene("Main");
    }

    void Phase13() //�N���A���U���g
    {
        textLog.text = "�Q�[�����N���A���܂���!";
        nextButton.SetActive(true);
    }

    void SetEnemy(string spriteName)�@//�G�̏��o�^
    {
        switch (spriteName)
        {
            case "GreenMush":
                eName = "�΃L�m�R";
                eHP = 7;
                eMP = 5;
                eATK = 1;
                eSkill1 = "�X�q����";
                eSkill2 = "�T�{��";
                break;
            case "Pumpking":
                eName = "���ڂ��Ⴈ�΂�";
                eHP = 8;
                eMP = 3;
                eATK = 1;
                eSkill1 = "���݂�";
                eSkill2 = "�܂ݐH��";
                break;
            case "AxeMan":
                eName = "�؂���";
                eHP = 18;
                eMP = 5;
                eATK = 3;
                eSkill1 = "�U��グ��";
                eSkill2 = "�R��";
                break;
            case "Bat":
                eName = "�R�E����";
                eHP = 15;
                eMP = 9;
                eATK = 2;
                eSkill1 = "�h���C��";
                eSkill2 = "���˂�";
                break;
            case "BushiFlame":
                eName = "���̌��m";
                eHP = 12;
                eMP = 5;
                eATK = 1;
                eSkill1 = "���̐n";
                eSkill2 = "�؃g���p���`";
                break;
            case "Cook":
                eName = "�����l";
                eHP = 10;
                eMP = 6;
                eATK = 1;
                eSkill1 = "�g������";
                eSkill2 = "�܂ݐH��";
                break;
            case "Dullahan":
                eName = "�f�����n��";
                eHP = 20;
                eMP = 10;
                eATK = 2;
                eSkill1 = "�n�C�q�[��";
                eSkill2 = "�����e"; 
                break;
            case "Knight":
                eName = "�R�m";
                eHP = 20;
                eMP = 8;
                eATK = 2;
                eSkill1 = "�X���b�V��";
                eSkill2 = "���̐n";
                break;
            case "Magician":
                eName = "���@�g��";
                eHP = 15;
                eMP = 8;
                eATK = 1;
                eSkill1 = "�t�@�C�A";
                eSkill2 = "�`���[�W";
                break;
            case "Satan":
                eName = "����";
                eHP = 30;
                eMP = 12;
                eATK = 3;
                eSkill1 = "�����e";
                eSkill2 = "�h���C��";
                break;
            case "Slime":
                eName = "�X���C��";
                eHP = 8;
                eMP = 5;
                eATK = 1;
                eSkill1 = "�r�߂�";
                eSkill2 = "�蓖��";
                break;
            case "ShieldMan":
                eName = "���g��";
                eHP = 20;
                eMP = 8;
                eATK = 1;
                eSkill1 = "�X�^���A�^�b�N";
                eSkill2 = "�R��";
                break;
        }
    }

    void EnemyAct()�@�@//�G�̍s���ݒ�
    {
        if (eName == "�΃L�m�R")
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
        else if (eName == "���ڂ��Ⴈ�΂�")
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
        else if (eName == "�؂���")
        {
            rnd = Random.Range(1, 4);
            if (rnd == 2 && CheckMP(eSkill1) <= eMP�@&& eCharge == false) { enemyAct = "Skill1"; }
            else if (rnd == 3 && CheckMP(eSkill2) <= eMP) { enemyAct = "Skill2"; }
            else { enemyAct = "Attack"; }
        }
        else if (eName == "�f�����n��")
        {
            rnd = Random.Range(1, 4);
            if (rnd == 2 && CheckMP(eSkill1) <= eMP && eHP < 7) { enemyAct = "Skill1"; }
            else if (rnd == 3 && CheckMP(eSkill2) <= eMP) { enemyAct = "Skill2"; }
            else { enemyAct = "Attack"; }
        }
        else if (eName == "���@�g��")
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

    

    int CheckMP(string skillName) //MP������Ă��邩�`�F�b�N
    {
        int costMP = 0;
        switch (skillName)
        {
            case "�X�q����":
                costMP = 1;
                break;
            case "�q�[��":
                costMP = 2;
                break;
            case "�T�{��":
                costMP = 0;
                break;
            case "���݂�":
                costMP = 2;
                break;
            case "�؃g���p���`":
                costMP = 2;
                break;
            case "���̐n":
                costMP = 2;
                break;
            case "�g������":
                costMP = 2;
                break;
            case "�X�^���A�^�b�N":
                costMP = 4;
                break;
            case "�h���C��":
                costMP = 4;
                break;
            case "�R��":
                costMP = 0;
                break;
            case "���˂�":
                costMP = 1;
                break;
            case "�r�߂�":
                costMP = 1;
                break;
            case "�X���b�V��":
                costMP = 3;
                break;
            case "�t�@�C�A":
                costMP = 4;
                break;
            case "�����e":
                costMP = 5;
                break;
            case "�n�C�q�[��":
                costMP = 5;
                break;
            case "�蓖��":
                costMP = 2;
                break;
            case "�`���[�W":
                costMP = 3;
                break;
            case "�U��グ��":
                costMP = 0;
                break;
            case "�܂ݐH��":
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

            case "�X�q����":
                costMP = 1;
                if (user == "Player")  //�v���C���[�@���@�����X�^�[�@�̎�
                {

                    damageGive = pATK + 1;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}�_���[�W!", damageGive);
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    numSE = 1;

                }
                else
                {
                    damageTake = eATK + 1;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    skillDesctiption = string.Format("{0}�_���[�W!", damageTake);
                    numSE = 0;
                    PlayPaticle(particle1);
                }
                break;

            case "�؃g���p���`":
                costMP = 2;
                if (user == "Player")  //�v���C���[�@���@�����X�^�[�@�̎�
                {

                    damageGive = pATK + 1;
                    pATK += 1;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}�_���[�W!�U���͂�1�㏸", damageGive);
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
                    skillDesctiption = string.Format("{0}�_���[�W!�G�̍U���͂�1�㏸", damageTake);
                    numSE = 0;
                    PlayPaticle(particle1);
                }
                break;

            case "���̐n":
                costMP = 2;
                rnd = Random.Range(1, 3);
                if (user == "Player")  //�v���C���[�@���@�����X�^�[�@�̎�
                {

                    damageGive = pATK + 3;
                    if (pCharge) { damageGive *= 2; }
                    if (rnd == 2) { eState = "Fire"; skillDesctiption = string.Format("{0}�_���[�W!,�G���Ώ�������!", damageGive); }
                    else { skillDesctiption = string.Format("{0}�_���[�W!", damageGive); }
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    numSE = 1;

                }
                else
                {
                    damageTake = eATK + 3;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    if (rnd == 2) { pState = "Fire"; skillDesctiption = string.Format("{0}�_���[�W!,�Ώ����Ă��܂���!", damageTake); }
                    else { skillDesctiption = string.Format("{0}�_���[�W!", damageTake); }
                    numSE = 0;
                    PlayPaticle(particle1);
                }
                break;

            case "�g������":
                costMP = 2;
                if (user == "Player")  //�v���C���[�@���@�����X�^�[�@�̎�
                {

                    damageGive = pATK + 1;
                    if (pCharge) { damageGive *= 2; }
                    eState = "Fire"; 
                    skillDesctiption = string.Format("{0}�_���[�W!,�G���Ώ�������!", damageGive);
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
            skillDesctiption = string.Format("{0}�_���[�W!,�Ώ����Ă��܂���!", damageTake);
                    numSE = 0;
                    PlayPaticle(particle1);
                }
                break;

            case "�X�^���A�^�b�N":
                costMP = 4;
                rnd = Random.Range(1, 3);
                if (user == "Player")  //�v���C���[�@���@�����X�^�[�@�̎�
                {
                    damageGive = pATK + 3;
                    if (pCharge) { damageGive *= 2; }
                    if (rnd == 2) { eStan = true; skillDesctiption = string.Format("{0}�_���[�W!,�G���X�^��������!", damageGive); }
                    else { skillDesctiption = string.Format("{0}�_���[�W!", damageGive); }
                    slashAnim.SetTrigger("On");
                    slashAnim.SetTrigger("Off");
                    numSE = 1;

                }
                else
                {
                    damageTake = eATK + 1;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    if (rnd == 2) { pStan = true; skillDesctiption = string.Format("{0}�_���[�W!,�X�^�����Ă��܂���!", damageTake); }
                    else { skillDesctiption = string.Format("{0}�_���[�W!", damageTake); }
                    numSE = 0;
                    PlayPaticle(particle1);
                }
                break;

            case "���݂�":
                costMP = 2;
                if (user == "Player")  //�v���C���[�@���@�����X�^�[�@�̎�
                {

                    damageGive = pATK + 1;
                    pHP += 1;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}�_���[�W&HP��1��!", damageGive);
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
                    skillDesctiption = string.Format("{0}�_���[�W&�G��HP��1��!", damageTake);
                    numSE = 3;
                    PlayPaticle(particle1);
                }
                break;

            case "�h���C��":
                costMP = 4;
                if (user == "Player")  //�v���C���[�@���@�����X�^�[�@�̎�
                {

                    damageGive = pATK + 2;
                    if (pCharge) { damageGive *= 2; }
                    pHP += damageGive;
                    skillDesctiption = string.Format("{0}�_���[�W,HP���z������", damageGive);
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
                    skillDesctiption = string.Format("{0}�_���[�W,HP���z������Ă��܂���", damageTake);
                    numSE = 7;
                    PlayPaticle(particle1);
                }
                break;

            case "�R��":
                costMP = 0;
               
                if (user == "Player")
                {

                    damageGive = pATK + 2;
                    pHP -= 1;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}�_���[�W,������������", damageGive);
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
                    skillDesctiption = string.Format("{0}�_���[�W,�G�͔�����������", damageTake);
                    PlayPaticle(particle1);
                    numSE = 0;
                }
                break;

            case "���˂�":
                costMP = 1;
                if (user == "Player")
                {

                    damageGive = pATK + 5;
                    pHP -= 1;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}�_���[�W,������������", damageGive);
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
                    skillDesctiption = string.Format("{0}�_���[�W,�G�͔�����������", damageTake);
                    PlayPaticle(particle1);
                    numSE = 0;
                }
                break;
            case "�r�߂�":
                costMP = 1;
                if (user == "Player")
                {

                    damageGive = pATK + 1;
                    eMP -= 1;
                    if (pCharge) { damageGive *= 2; }
                    if (eMP < 0) { eMP = 0; }
                    skillDesctiption = string.Format("{0}�_���[�W,MP��1�������I", damageGive);
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
                    skillDesctiption = string.Format("{0}�_���[�W,MP��1������ꂽ�I", damageTake);
                    numSE = 3;
                    PlayPaticle(particle1);
                }
                break;

            case "�X���b�V��":
                costMP = 3;
                if (user == "Player")
                {

                    damageGive = pATK + 5;
                    eMP -= 3;
                    if (pCharge) { damageGive *= 2; }
                    if (eMP < 0) { eMP = 0; }
                    skillDesctiption = string.Format("{0}�_���[�W,MP��3�������I", damageGive);
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
                    skillDesctiption = string.Format("{0}�_���[�W,MP��3������ꂽ�I", damageTake);
                    numSE = 3;
                    PlayPaticle(particle1);
                }
                break;

            case "�t�@�C�A":
                costMP = 4;
                if (user == "Player")
                {

                    damageGive = pATK * 2;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}�_���[�W�I", damageGive);
                    fireAnim.SetTrigger("On");
                    fireAnim.SetTrigger("Off");
                    numSE = 7;

                }
                else
                {
                    damageTake = eATK * 2;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    skillDesctiption = string.Format("{0}�_���[�W�I", damageTake);
                    numSE = 7;
                    PlayPaticle(particle1);
                }
                break;

            case "�����e":
                costMP = 5;
                if (user == "Player")
                {

                    damageGive = pATK + 7;
                    if (pCharge) { damageGive *= 2; }
                    skillDesctiption = string.Format("{0}�_���[�W�I", damageGive);
                    darkAnim.SetTrigger("On");
                    darkAnim.SetTrigger("Off");
                    numSE = 7;

                }
                else
                {
                    damageTake = eATK + 7;
                    if (eCharge) { damageTake *= 2; }
                    if (deffence) { damageTake /= 2; }
                    skillDesctiption = string.Format("{0}�_���[�W�I", damageTake);
                    numSE = 7;
                    PlayPaticle(particle1);
                }
                break;

            case "�q�[��":
                costMP = 2;
                numSE = 6;
                if (user == "Player")
                {
                    damageTake = -8;
                    skillDesctiption = string.Format("{0}�񕜂���!", -damageTake);
                    PlayPaticle(particle2);
                }
                else if (user == "Enemy")
                {
                    damageGive = -8;
                    skillDesctiption = string.Format("�G��{0}�񕜂���!", -damageGive);
                }
                break;

            case "�n�C�q�[��":
                costMP = 5;
                numSE = 6;
                if (user == "Player")
                {
                    damageTake = -15;
                    skillDesctiption = string.Format("{0}�񕜂���!", -damageTake);
                    PlayPaticle(particle2);
                }
                else if (user == "Enemy")
                {
                    damageGive = -15;
                    skillDesctiption = string.Format("�G��{0}�񕜂���!", -damageGive);
                }
                break;


            case "�蓖��":
                costMP = 2;
                numSE = 4;
                if (user == "Player")
                {
                    pState = "Regen";
                    skillDesctiption = string.Format("�蓖�Ă��Ă���");
                }
                else if (user == "Enemy")
                {
                    eState = "Regen";
                    skillDesctiption = string.Format("�G�͎蓖�Ă��Ă���");
                }
                break;
            case "�`���[�W":
                costMP = 3;
                numSE = 6;
                if (user == "Player")
                {
                    pATK += 2;
                    skillDesctiption = string.Format("�U���͂�2�㏸");
                }
                else if (user == "Enemy")
                {
                    eATK += 2;
                    skillDesctiption = string.Format("�G�̍U���͂�2�㏸");
                }
                break;

            case "�U��グ��":
                costMP = 0;
                numSE = 4;
                if (user == "Player")
                {
                    pCharge = true;
                    pChargedTurn = turn;
                    skillDesctiption = string.Format("�U��グ�Ă���");
                }
                else if (user == "Enemy")
                {
                    eCharge = true;
                    eChargedTurn = turn;
                    skillDesctiption = string.Format("�G�͐U��グ�Ă���");
                }
                break;

            case "�܂ݐH��":
                costMP = 1;
                numSE = 3;
                rnd = Random.Range(1, 3);
                if (user == "Player")
                {
                    damageTake = -6;
                    skillDesctiption = string.Format("{0}�񕜂���!", -damageTake);
                    if (rnd == 2) { pState = "Fire"; skillDesctiption = string.Format("{0}�񕜂������Ώ����Ă��܂���", -damageTake); }
                    Instantiate(particle2, new Vector2(0, 0), Quaternion.identity);
                }
                else if (user == "Enemy")
                {
                    damageGive = -6;
                    skillDesctiption = string.Format("{0}�񕜂���!", -damageGive);
                    if (rnd == 2) { eState = "Fire"; skillDesctiption = string.Format("�G��{0}�񕜂������Ώ����Ă��܂���", -damageGive); }
                }
                break;
            case "�T�{��":
                rnd = Random.Range(1, 4);
                numSE = 4;
                costMP = 0;
                if (user == "Player")
                {
                    if(rnd == 3)
                    {
                        pHP += 1;
                        pMP += 1;
                        skillDesctiption = ("HP��MP��1�񕜂���!");
                    }
                    else { skillDesctiption = ("�T�{���Ă���!"); }

                }
                else if (user == "Enemy")
                {
                    if (rnd == 3)
                    {
                        eHP += 1;
                        eMP += 1;
                        skillDesctiption = ("�G��HP��MP��1�񕜂���!");
                    }
                    else { skillDesctiption = ("�T�{���Ă���!"); }
                }

                break;
        }

        if (deffence && damageTake != 0) { damageTake = System.Math.Max(damageTake, 1); }  //�h�䎞�ŏ��͂P�_���[�W
        pHP -= damageTake; eHP -= damageGive;�@�@�@
        if (user == "Player") { pMP -= costMP; }
        else { eMP -= costMP; }
        SoundEffect.SETrigger[numSE] = true;
        textLog.text = skillDesctiption;

    }�@//�X�L���g�p
    void PlayPaticle(GameObject particle)//�p�[�e�B�N������
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
                case "�X�q����":                 
                    infoText.text = "�U���� + 1�̃_���[�W";
                    break;
                case "�q�[��":
                    infoText.text = "8HP��";    
                    break;
                case "�T�{��":
                    infoText.text = "���܂�HP��MP��1�񕜂���";
                    break;
                case "���݂�":
                    infoText.text = "�U���� + 1�̃_���[�W,1HP��";
                    break;
                case "�܂ݐH��":
                    infoText.text = "6HP�񕜂��邪,�m���ŉΏ�";
                    break;
                case "�؃g���p���`":
                    infoText.text = "�U���� + 1�̃_���[�W,�U����1����";
                    break;
                case "���̐n":
                    infoText.text = "�U���� + 3�̃_���[�W,�m���ŉΏ�";
                    break;
                case "�g������":
                    infoText.text = "�U���� + 1�̃_���[�W,�Ώ��t�^";
                    break;
                case "�X�^���A�^�b�N":
                    infoText.text = "�U���� + 3�̃_���[�W,�m���ŃX�^���t�^";
                    break;
                case "�h���C��":
                    infoText.text = "�U���� + 3�̃_���[�W,�^�����_���[�W�̕��񕜂���";
                    break;
                case "�R��":
                    infoText.text = "�U���� + 2�̃_���[�W,�������󂯂�";
                    break;
                case "���˂�":
                    infoText.text = "�U���� + 5�̃_���[�W,�������󂯂�";
                    break;
                case "�r�߂�":
                    infoText.text = "�U���� + 1�̃_���[�W,MP��1���";
                    break;
                case "�X���b�V��":
                    infoText.text = "�U���� + 5�̃_���[�W,MP��3���";
                    break;
                case "�t�@�C�A":
                    infoText.text = "�U���� �~ 2�̃_���[�W";
                    break;
                case "�����e":
                    infoText.text = "�U���� + 7�̃_���[�W,MP��3���";
                    break;
                case "�n�C�q�[��":
                    infoText.text = "15HP�񕜂���";
                    break;
                case "�蓖��":
                    infoText.text = "���^�[��1HP�񕜂���,�Ώ��𒼂���";
                    break;
                case "�`���[�W":
                    infoText.text = "�U���͂�2����(�퓬��)";
                    break;
                case "�U��グ��":
                    infoText.text = "���̃^�[���̍U����2�{�̃_���[�W";
                    break;
            }
            costMP = CheckMP(skillName);
            infoMP.text = "����MP: " + costMP.ToString();
        }
        else
        {
            skillInfoFrame.SetActive(false);
            infoMP.text = null;
            infoName.text = null;
            infoText.text = null;
        }
      
    }
    void Reward3() �@�@//3�ڂ̕�V�̐ݒ�
    {
        switch (rnd)
        {
            case 1:
                pHP += 10;
                textLog.text = "HP��10�񕜂���!";
                break;
            case 2:
                pMP += 10;
                textLog.text = "MP��10�񕜂���!";
                break;
            case 3:
                pHP += 7; pMP += 7;
                textLog.text = "HP��MP��7�񕜂���!";
                break;
            case 4:
                pATK += 1;
                resetAtk += 1;
                textLog.text = "�U���͂�1��������!";
                break;
            case 5:
                pHP += 10; pMP += 10;
                textLog.text = "HP��MP��10�񕜂���!";
                break;
            case 6:
                pATK += 1;
                resetAtk += 1;
                textLog.text = "�U���͂�1��������!";
                break;

        }
    }

    [SerializeField] Text finalHP, finalMP, finalATK, enemyKilled, finalScore;
    int showHP, showMP, showATK, showScore, num;
    IEnumerator ShowResult()�@�@//���U���g�\��
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
            finalScore.text = "�X�R�A: " + showScore;
            enemyKilled.text = "�~"+killed + "kill";
            SoundEffect.SETrigger[10] = true;
            yield return new WaitForSeconds(2.0f);
            showScore *= killed;
            finalScore.text = "�X�R�A: " + showScore;
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
    }�@//���l���������߂Â���

   

}
