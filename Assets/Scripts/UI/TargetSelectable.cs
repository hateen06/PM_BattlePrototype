using UnityEngine;
using UnityEngine.UI;

public class TargetSelectable : MonoBehaviour
{
    [SerializeField] private UnitHP unitHP;
    [SerializeField] private BattleUIController battleUIController;
    [SerializeField] private Image slotImage;

    private static TargetSelectable currentSelectedTarget;

    private Color normalColor = Color.white;
    private Color selectedColor = new Color(1f, 0.8f, 0.8f, 1f);

    public void OnClickTarget()
    {
        if (unitHP == null || battleUIController == null)
            return;

        if (unitHP.IsDead())
            return;

        if (currentSelectedTarget != null)
        {
            currentSelectedTarget.ResetVisual();
        }

        currentSelectedTarget = this;
        SetSelectedVisual();

        battleUIController.SetTarget(unitHP);
    }

    private void SetSelectedVisual()
    {
        if (slotImage != null)
        {
            slotImage.color = selectedColor;
        }
    }

    private void ResetVisual()
    {
        if (slotImage != null)
        {
            slotImage.color = normalColor;
        }
    }
}