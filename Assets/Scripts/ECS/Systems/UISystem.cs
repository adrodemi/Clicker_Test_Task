using Leopotam.EcsLite;
using TMPro;
using UnityEngine;

public class UISystem : IEcsRunSystem
{
    private readonly BusinessNamesConfig _namesConfig;

    public UISystem(BusinessNamesConfig namesConfig)
    {
        _namesConfig = namesConfig;
    }

    public void Run(IEcsSystems systems)
    {
        var world = systems.GetWorld();

        var uiPool = world.GetPool<UIReferenceComponent>();
        var businessPool = world.GetPool<BusinessComponent>();
        var levelPool = world.GetPool<LevelComponent>();
        var progressPool = world.GetPool<IncomeProgressComponent>();
        var multiplierPool = world.GetPool<IncomeMultiplierComponent>();
        var upgradePool = world.GetPool<UpgradeComponent>();
        var balancePool = world.GetPool<BalanceComponent>();

        foreach (var e in world.Filter<BalanceComponent>().End())
        {
            ref var balance = ref balancePool.Get(e);
            var textObject = GameObject.Find("BalanceText");
            if (textObject != null)
            {
                var tmp = textObject.GetComponent<TextMeshProUGUI>();
                tmp.text = $"Баланс: {balance.value:0}$";
            }
        }

        var filter = world.Filter<UIReferenceComponent>()
            .Inc<BusinessComponent>()
            .Inc<LevelComponent>()
            .Inc<IncomeProgressComponent>()
            .Inc<IncomeMultiplierComponent>()
            .Inc<UpgradeComponent>()
            .End();

        foreach (var entity in filter)
        {
            ref var ui = ref uiPool.Get(entity);
            ref var business = ref businessPool.Get(entity);
            ref var level = ref levelPool.Get(entity);
            ref var progress = ref progressPool.Get(entity);
            ref var multiplier = ref multiplierPool.Get(entity);
            ref var upgrade = ref upgradePool.Get(entity);

            var nameEntry = GetNameEntry(business.id);

            ui.nameText.text = nameEntry.businessName;

            float income = level.value * business.baseIncome * multiplier.value;
            ui.levelText.text = $"LVL\n{level.value}";
            ui.incomeText.text = $"Доход\n{income:0}$";
            ui.progressBar.value = Mathf.Clamp01(progress.progress / business.delay);

            float levelUpCost = (level.value + 1) * business.baseCost;
            ui.levelUpButton.GetComponentInChildren<TextMeshProUGUI>().text = $"LVL UP\nЦена: {levelUpCost:0}$";

            string upg1Title = nameEntry.upgrade1Name;
            string upg1Income = $"+ {business.upgrade1.incomeMultiplier:0}%";
            if (upgrade.upgrade1Bought)
            {
                ui.upgrade1Button.interactable = false;
                ui.upgrade1Button.GetComponentInChildren<TextMeshProUGUI>().text = "Куплено";
                ui.upgrade1InfoText.text = $"{upg1Title}\nДоход: {upg1Income}\nКуплено";
            }
            else
            {
                ui.upgrade1Button.interactable = true;
                float cost = business.upgrade1.cost;
                ui.upgrade1Button.GetComponentInChildren<TextMeshProUGUI>().text = $"{cost:0}$";
                ui.upgrade1InfoText.text = $"{upg1Title}\nДоход: {upg1Income}\nЦена: {cost:0}$";
            }

            string upg2Title = nameEntry.upgrade2Name;
            string upg2Income = $"+ {business.upgrade2.incomeMultiplier:0}%";
            if (upgrade.upgrade2Bought)
            {
                ui.upgrade2Button.interactable = false;
                ui.upgrade2Button.GetComponentInChildren<TextMeshProUGUI>().text = "Куплено";
                ui.upgrade2InfoText.text = $"{upg2Title}\nДоход: {upg2Income}\nКуплено";
            }
            else
            {
                ui.upgrade2Button.interactable = true;
                float cost = business.upgrade2.cost;
                ui.upgrade2Button.GetComponentInChildren<TextMeshProUGUI>().text = $"{cost:0}$";
                ui.upgrade2InfoText.text = $"{upg2Title}\nДоход: {upg2Income}\nЦена: {cost:0}$";
            }
        }
    }

    private BusinessNameEntry GetNameEntry(int id)
    {
        foreach (var entry in _namesConfig.entries)
            if (entry.businessId == id)
                return entry;

        return new BusinessNameEntry
        {
            businessName = $"Бизнес {id}",
            upgrade1Name = "Улучшение 1",
            upgrade2Name = "Улучшение 2"
        };
    }
}