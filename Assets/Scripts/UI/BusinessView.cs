using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Leopotam.EcsLite;

public class BusinessView : MonoBehaviour
{
    [Header("Texts")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI incomeText;

    [Header("Progress")]
    public Slider progressBar;

    [Header("Buttons")]
    public Button levelUpButton;
    public Button upgrade1Button;
    public Button upgrade2Button;

    [Header("Upgrade Info")]
    public TextMeshProUGUI upgrade1InfoText;
    public TextMeshProUGUI upgrade2InfoText;

    [HideInInspector] public int entityId;

    private EcsWorld _world;

    public void Init(EcsWorld world)
    {
        _world = world;

        levelUpButton.onClick.AddListener(OnLevelUpClick);
        upgrade1Button.onClick.AddListener(() => OnUpgradeClick(0));
        upgrade2Button.onClick.AddListener(() => OnUpgradeClick(1));
    }

    private void OnLevelUpClick()
    {
        int eventEntity = _world.NewEntity();
        _world.GetPool<ClickBuyLevelComponent>().Add(eventEntity);
        _world.GetPool<EcsEntityRefComponent>().Add(eventEntity).target = entityId;
    }

    private void OnUpgradeClick(int index)
    {
        int eventEntity = _world.NewEntity();
        _world.GetPool<ClickBuyUpgradeComponent>().Add(eventEntity).upgradeIndex = index;
        _world.GetPool<EcsEntityRefComponent>().Add(eventEntity).target = entityId;
    }
}