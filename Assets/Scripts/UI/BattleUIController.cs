using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class BattleUIController : MonoBehaviour
{
    private enum ActionType
    {
        None,
        Attack,
        Guard,
        Skill
    }

    [Header("UI Text")]
    [SerializeField] private TMP_Text logText;
    [SerializeField] private TMP_Text turnText;

    [Header("Action Button Images")]
    [SerializeField] private Image attackButtonImage;
    [SerializeField] private Image guardButtonImage;
    [SerializeField] private Image skillButtonImage;

    [Header("Units")]
    [SerializeField] private UnitHP[] enemyUnits;
    [SerializeField] private UnitHP[] allyUnits;

    [Header("Skill Data")]
    [SerializeField] private SkillData attackData;
    [SerializeField] private SkillData guardData;
    [SerializeField] private SkillData skillData;

    [Header("Responsive Layout")]
    [SerializeField] private bool autoArrangeUI = true;
    [SerializeField] private RectTransform battleFieldArea;
    [SerializeField] private RectTransform enemyRow;
    [SerializeField] private RectTransform allyRow;
    [SerializeField] private RectTransform actionPanel;
    [SerializeField] private RectTransform[] enemySlots;
    [SerializeField] private RectTransform[] allySlots;
    [SerializeField] private RectTransform[] actionButtons;
    [SerializeField] private float rowSidePadding = 120f;
    [SerializeField] private float rowSpacing = 32f;
    [SerializeField] private float buttonSidePadding = 96f;
    [SerializeField] private float buttonSpacing = 24f;

    [Header("Visual Polish")]
    [SerializeField] private Image topInfoPanelImage;
    [SerializeField] private Image logPanelImage;
    [SerializeField] private Image controlPanelImage;

    [Header("Text Clarity")]
    [SerializeField] private TMP_FontAsset uiFontOverride;
    [SerializeField] private bool enableTextAutoSizing = true;
    [SerializeField] private float minAutoFontSize = 18f;
    [SerializeField] private float maxAutoFontSize = 44f;
    [SerializeField] private float textOutlineWidth = 0.12f;
    [SerializeField] private Color textOutlineColor = new Color(0f, 0f, 0f, 0.55f);

    private int currentTurn = 1;
    private ActionType currentAction = ActionType.None;
    private UnitHP currentTarget;
    private bool battleEnded;

    private readonly List<TMP_Text> cachedTexts = new List<TMP_Text>();
    private Vector2Int lastScreenSize;

    private readonly Color normalButtonColor = new Color(0.90f, 0.95f, 1f, 1f);
    private readonly Color selectedButtonColor = new Color(1f, 0.86f, 0.45f, 1f);

    private void Awake()
    {
        PrepareUI(forceTextRefresh: true);
    }

    private void OnEnable()
    {
        PrepareUI(forceTextRefresh: true);
    }

    private void OnValidate()
    {
        PrepareUI(forceTextRefresh: true);
    }

    private void Start()
    {
        if (turnText != null)
            turnText.text = $"TURN {currentTurn}";
    }

    private void Update()
    {
        if (!autoArrangeUI)
            return;

        if (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
            PrepareUI(forceTextRefresh: false);
    }

    public void OnClickAttack()
    {
        if (battleEnded)
            return;

        currentAction = ActionType.Attack;
        logText.text = $"{attackData.skillName} selected";
        UpdateActionButtonVisual();
    }

    public void OnClickGuard()
    {
        if (battleEnded)
            return;

        currentAction = ActionType.Guard;
        logText.text = $"{guardData.skillName} selected";
        UpdateActionButtonVisual();
    }

    public void OnClickSkill()
    {
        if (battleEnded)
            return;

        currentAction = ActionType.Skill;
        logText.text = $"{skillData.skillName} selected";
        UpdateActionButtonVisual();
    }

    public void SetTarget(UnitHP target)
    {
        if (battleEnded || target == null || target.IsDead())
            return;

        currentTarget = target;
        logText.text = $"{target.name} targeted";
    }

    public void OnClickStart()
    {
        if (battleEnded)
            return;

        currentTurn++;
        turnText.text = $"TURN {currentTurn}";

        bool playerActionSucceeded = ExecutePlayerAction();
        if (!playerActionSucceeded)
            return;

        EvaluateBattleResult(enemyUnits, allyUnits);
        if (battleEnded)
            return;

        ExecuteEnemyTurn();
        EvaluateBattleResult(enemyUnits, allyUnits);
    }

    private bool ExecutePlayerAction()
    {
        switch (currentAction)
        {
            case ActionType.Attack:
                return TryUseSkill(attackData, "Choose a target for attack.");
            case ActionType.Guard:
                logText.text = "Guard stance is active.";
                return true;
            case ActionType.Skill:
                return TryUseSkill(skillData, "Choose a target for skill.");
            default:
                logText.text = "Choose an action first.";
                return false;
        }
    }

    private bool TryUseSkill(SkillData data, string failMessage)
    {
        if (data == null)
        {
            logText.text = "Skill data is missing.";
            return false;
        }

        if (data.isGuard)
        {
            logText.text = $"{data.skillName} used";
            return true;
        }

        if (currentTarget != null && !currentTarget.IsDead())
        {
            currentTarget.TakeDamage(data.damage);
            logText.text = $"{currentTarget.name} took {data.damage} damage";
            return true;
        }

        logText.text = failMessage;
        return false;
    }

    private void ExecuteEnemyTurn()
    {
        UnitHP targetAlly = GetFirstAliveUnit(allyUnits);
        if (targetAlly == null)
            return;

        targetAlly.TakeDamage(10);
        logText.text = $"Enemy attack! {targetAlly.name} took 10 damage";
    }

    private UnitHP GetFirstAliveUnit(UnitHP[] units)
    {
        foreach (UnitHP unit in units)
        {
            if (unit != null && !unit.IsDead())
                return unit;
        }

        return null;
    }

    private bool AreAllUnitsDead(UnitHP[] units)
    {
        foreach (UnitHP unit in units)
        {
            if (unit != null && !unit.IsDead())
                return false;
        }

        return true;
    }

    private void EvaluateBattleResult(UnitHP[] enemies, UnitHP[] allies)
    {
        if (AreAllUnitsDead(enemies))
        {
            battleEnded = true;
            logText.text = "Victory!";
            return;
        }

        if (AreAllUnitsDead(allies))
        {
            battleEnded = true;
            logText.text = "Defeat...";
        }
    }

    private void PrepareUI(bool forceTextRefresh)
    {
        AutoBindMissingReferences();
        NormalizeCanvasRoot();
        ApplyResponsiveLayout();
        ApplyVisualPolish();
        RefreshTextCache(forceTextRefresh);
        ApplyTextClarity();
        UpdateActionButtonVisual();

        lastScreenSize = new Vector2Int(Screen.width, Screen.height);
    }

    private void AutoBindMissingReferences()
    {
        if (battleFieldArea == null) battleFieldArea = FindRect("BattleFieldArea");
        if (enemyRow == null) enemyRow = FindRect("EnemyRow");
        if (allyRow == null) allyRow = FindRect("AllyRow");
        if (actionPanel == null) actionPanel = FindRect("BottomSkillPanel");

        if (enemySlots == null || enemySlots.Length == 0)
            enemySlots = FindRects("EnemySlot1", "EnemySlot2", "EnemySlot3");

        if (allySlots == null || allySlots.Length == 0)
            allySlots = FindRects("AllySlot1", "AllySlot2", "AllySlot3");

        if (actionButtons == null || actionButtons.Length == 0)
            actionButtons = FindRects("AttackButton", "GuardButton", "SkillButton");

        if (attackButtonImage == null) attackButtonImage = FindImage("AttackButton");
        if (guardButtonImage == null) guardButtonImage = FindImage("GuardButton");
        if (skillButtonImage == null) skillButtonImage = FindImage("SkillButton");

        if (topInfoPanelImage == null) topInfoPanelImage = FindImage("TopInfoPanel");
        if (logPanelImage == null) logPanelImage = FindImage("LogPanel");
        if (controlPanelImage == null) controlPanelImage = FindImage("ControlPanel");

        if (logText == null) logText = FindText("LogText");
        if (turnText == null) turnText = FindText("TurnText");
    }

    private void NormalizeCanvasRoot()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
            return;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (canvasRect == null)
            return;

        canvasRect.localScale = Vector3.one;
        canvasRect.anchorMin = Vector2.zero;
        canvasRect.anchorMax = Vector2.one;
        canvasRect.offsetMin = Vector2.zero;
        canvasRect.offsetMax = Vector2.zero;
        canvasRect.pivot = new Vector2(0.5f, 0.5f);
    }

    private void ApplyResponsiveLayout()
    {
        if (!autoArrangeUI)
            return;

        if (battleFieldArea != null)
        {
            battleFieldArea.anchorMin = new Vector2(0.05f, 0.34f);
            battleFieldArea.anchorMax = new Vector2(0.95f, 0.90f);
            battleFieldArea.offsetMin = Vector2.zero;
            battleFieldArea.offsetMax = Vector2.zero;
        }

        if (enemyRow != null)
        {
            enemyRow.anchorMin = new Vector2(0f, 1f);
            enemyRow.anchorMax = new Vector2(1f, 1f);
            enemyRow.pivot = new Vector2(0.5f, 1f);
            enemyRow.anchoredPosition = new Vector2(0f, -100f);
            enemyRow.sizeDelta = new Vector2(-140f, 170f);
        }

        if (allyRow != null)
        {
            allyRow.anchorMin = new Vector2(0f, 0f);
            allyRow.anchorMax = new Vector2(1f, 0f);
            allyRow.pivot = new Vector2(0.5f, 0f);
            allyRow.anchoredPosition = new Vector2(0f, 100f);
            allyRow.sizeDelta = new Vector2(-140f, 170f);
        }

        if (actionPanel != null)
        {
            actionPanel.anchorMin = new Vector2(0.5f, 0f);
            actionPanel.anchorMax = new Vector2(0.5f, 0f);
            actionPanel.pivot = new Vector2(0.5f, 0f);
            actionPanel.anchoredPosition = new Vector2(0f, 70f);
            actionPanel.sizeDelta = new Vector2(Mathf.Clamp(GetCanvasWidth() * 0.72f, 760f, 1100f), 180f);
        }

        ArrangeInRow(enemyRow, enemySlots, rowSidePadding, rowSpacing, new Vector2(200f, 150f));
        ArrangeInRow(allyRow, allySlots, rowSidePadding, rowSpacing, new Vector2(200f, 150f));
        ArrangeInRow(actionPanel, actionButtons, buttonSidePadding, buttonSpacing, new Vector2(210f, 105f));
    }

    private void ArrangeInRow(RectTransform container, RectTransform[] elements, float sidePadding, float preferredSpacing, Vector2 preferredElementSize)
    {
        if (container == null || elements == null || elements.Length == 0)
            return;

        List<RectTransform> validElements = new List<RectTransform>();
        foreach (RectTransform element in elements)
        {
            if (element != null)
                validElements.Add(element);
        }

        if (validElements.Count == 0)
            return;

        float containerWidth = container.rect.width;
        if (containerWidth <= 0f)
            containerWidth = container.sizeDelta.x;
        if (containerWidth <= 0f)
            return;

        float availableWidth = Mathf.Max(0f, containerWidth - (sidePadding * 2f));
        float widthByCount = (availableWidth - preferredSpacing * (validElements.Count - 1)) / validElements.Count;
        float elementWidth = Mathf.Clamp(widthByCount, 110f, preferredElementSize.x);
        float spacing = validElements.Count > 1
            ? Mathf.Max(16f, (availableWidth - elementWidth * validElements.Count) / (validElements.Count - 1))
            : 0f;

        float totalWidth = elementWidth * validElements.Count + spacing * Mathf.Max(0, validElements.Count - 1);
        float startX = (-totalWidth * 0.5f) + (elementWidth * 0.5f);

        for (int i = 0; i < validElements.Count; i++)
        {
            RectTransform element = validElements[i];
            element.anchorMin = new Vector2(0.5f, 0.5f);
            element.anchorMax = new Vector2(0.5f, 0.5f);
            element.pivot = new Vector2(0.5f, 0.5f);
            element.sizeDelta = new Vector2(elementWidth, preferredElementSize.y);
            element.anchoredPosition = new Vector2(startX + (elementWidth + spacing) * i, 0f);
        }
    }

    private void ApplyVisualPolish()
    {
        SetImageColor(topInfoPanelImage, new Color(0.07f, 0.12f, 0.18f, 0.92f));
        SetImageColor(logPanelImage, new Color(0.08f, 0.10f, 0.14f, 0.92f));
        SetImageColor(controlPanelImage, new Color(0.07f, 0.10f, 0.15f, 0.92f));
    }

    private void RefreshTextCache(bool forceRefresh)
    {
        if (!forceRefresh && cachedTexts.Count > 0)
            return;

        cachedTexts.Clear();
        TMP_Text[] texts = FindObjectsOfType<TMP_Text>(true);
        foreach (TMP_Text text in texts)
        {
            if (text != null)
                cachedTexts.Add(text);
        }
    }

    private void ApplyTextClarity()
    {
        foreach (TMP_Text text in cachedTexts)
        {
            if (text == null)
                continue;

            if (uiFontOverride != null)
                text.font = uiFontOverride;

            text.extraPadding = true;
            text.enableAutoSizing = enableTextAutoSizing;
            text.fontSizeMin = minAutoFontSize;
            text.fontSizeMax = maxAutoFontSize;
            text.outlineWidth = textOutlineWidth;
            text.outlineColor = textOutlineColor;

            if (text == turnText || text == logText)
                text.fontStyle = FontStyles.Bold;

            RectTransform textRect = text.rectTransform;
            textRect.anchoredPosition = new Vector2(
                Mathf.Round(textRect.anchoredPosition.x),
                Mathf.Round(textRect.anchoredPosition.y));
        }
    }

    private void UpdateActionButtonVisual()
    {
        if (attackButtonImage != null)
            attackButtonImage.color = currentAction == ActionType.Attack ? selectedButtonColor : normalButtonColor;

        if (guardButtonImage != null)
            guardButtonImage.color = currentAction == ActionType.Guard ? selectedButtonColor : normalButtonColor;

        if (skillButtonImage != null)
            skillButtonImage.color = currentAction == ActionType.Skill ? selectedButtonColor : normalButtonColor;
    }

    private float GetCanvasWidth()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
            return 1920f;

        RectTransform rect = canvas.GetComponent<RectTransform>();
        if (rect == null || rect.rect.width <= 0f)
            return 1920f;

        return rect.rect.width;
    }

    private static RectTransform FindRect(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        return obj != null ? obj.GetComponent<RectTransform>() : null;
    }

    private static RectTransform[] FindRects(params string[] objectNames)
    {
        List<RectTransform> results = new List<RectTransform>();
        foreach (string objectName in objectNames)
        {
            RectTransform rect = FindRect(objectName);
            if (rect != null)
                results.Add(rect);
        }

        return results.ToArray();
    }

    private static Image FindImage(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        return obj != null ? obj.GetComponent<Image>() : null;
    }

    private static TMP_Text FindText(string objectName)
    {
        GameObject obj = GameObject.Find(objectName);
        return obj != null ? obj.GetComponent<TMP_Text>() : null;
    }

    private static void SetImageColor(Image image, Color color)
    {
        if (image != null)
            image.color = color;
    }
}
