using UnityEngine;
using Leopotam.EcsLite;
using TMPro;

public class EcsStartup : MonoBehaviour
{
    private EcsWorld _world;
    private EcsSystems _systems;

    [Header("Configs")]
    [SerializeField] private AllBusinessesConfig allBusinessesConfig;
    [SerializeField] private BusinessNamesConfig namesConfig;

    [Header("UI")]
    [SerializeField] private Transform businessUiParent;
    [SerializeField] private GameObject businessPrefab;
    [SerializeField] private TextMeshProUGUI balanceText;

    private void Start()
    {
        //GameSaveService.Clear();
        _world = new EcsWorld();
        _systems = new EcsSystems(_world);

        _systems
            .Add(new ProgressSystem())
            .Add(new BuyLevelSystem())
            .Add(new BuyUpgradeSystem())
            .Add(new UISystem(namesConfig))
            .Init();

        SpawnInitialEntities();
    }

    private void Update()
    {
        _systems?.Run();
    }

    private void OnDestroy()
    {
        _systems?.Destroy();
        _systems = null;
        _world?.Destroy();
        _world = null;
    }

    private void SpawnInitialEntities()
    {
        var save = GameSaveService.Load();

        var businessPool = _world.GetPool<BusinessComponent>();
        var levelPool = _world.GetPool<LevelComponent>();
        var progressPool = _world.GetPool<IncomeProgressComponent>();
        var upgradePool = _world.GetPool<UpgradeComponent>();
        var multiplierPool = _world.GetPool<IncomeMultiplierComponent>();
        var uiPool = _world.GetPool<UIReferenceComponent>();

        for (int i = 0; i < allBusinessesConfig.businesses.Length; i++)
        {
            var config = allBusinessesConfig.businesses[i];
            var entity = _world.NewEntity();

            businessPool.Add(entity) = new BusinessComponent
            {
                id = config.businessId,
                baseCost = config.baseCost,
                baseIncome = config.baseIncome,
                delay = config.incomeDelay,
                upgrade1 = config.upgrade1,
                upgrade2 = config.upgrade2
            };

            BusinessSave saved = save?.businesses.Find(b => b.id == config.businessId);

            levelPool.Add(entity) = new LevelComponent { value = saved?.level ?? (i == 0 ? 1 : 0) };
            progressPool.Add(entity) = new IncomeProgressComponent { progress = saved?.progress ?? 0 };
            upgradePool.Add(entity) = new UpgradeComponent()
            {
                upgrade1Bought = saved?.upgrade1Bought ?? false,
                upgrade2Bought = saved?.upgrade2Bought ?? false
            };
            float multiplier = 1f;
            if (saved?.upgrade1Bought == true) multiplier += config.upgrade1.incomeMultiplier / 100f;
            if (saved?.upgrade2Bought == true) multiplier += config.upgrade2.incomeMultiplier / 100f;
            multiplierPool.Add(entity) = new IncomeMultiplierComponent { value = multiplier };

            GameObject go = Instantiate(businessPrefab, businessUiParent);
            var view = go.GetComponent<BusinessView>();
            view.entityId = entity;
            view.Init(_world);

            var nameData = GetNameForBusiness(config.businessId);
            view.nameText.text = nameData.businessName;
            view.upgrade1InfoText.text = nameData.upgrade1Name;
            view.upgrade2InfoText.text = nameData.upgrade2Name;

            uiPool.Add(entity) = new UIReferenceComponent
            {
                nameText = view.nameText,
                levelText = view.levelText,
                incomeText = view.incomeText,
                progressBar = view.progressBar,
                levelUpButton = view.levelUpButton,
                upgrade1Button = view.upgrade1Button,
                upgrade2Button = view.upgrade2Button,
                upgrade1InfoText = view.upgrade1InfoText,
                upgrade2InfoText = view.upgrade2InfoText
            };

        }

        int player = _world.NewEntity();
        var balancePool = _world.GetPool<BalanceComponent>();
        balancePool.Add(player) = new BalanceComponent
        {
            value = save?.balance ?? 150f
        };
    }

    private BusinessNameEntry GetNameForBusiness(int id)
    {
        foreach (var entry in namesConfig.entries)
            if (entry.businessId == id)
                return entry;

        return new BusinessNameEntry
        {
            businessName = $"Бизнес {id}",
            upgrade1Name = "Улучшение 1",
            upgrade2Name = "Улучшение 2"
        };
    }
    
    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveProgress();
    }

    private void OnApplicationQuit()
    {
        SaveProgress();
    }

    private void SaveProgress()
    {
        var data = new SaveData();

        var balancePool = _world.GetPool<BalanceComponent>();
        foreach (var e in _world.Filter<BalanceComponent>().End())
            data.balance = balancePool.Get(e).value;

        var businessPool = _world.GetPool<BusinessComponent>();
        var levelPool = _world.GetPool<LevelComponent>();
        var progressPool = _world.GetPool<IncomeProgressComponent>();
        var upgradePool = _world.GetPool<UpgradeComponent>();

        var filter = _world.Filter<BusinessComponent>()
            .Inc<LevelComponent>()
            .Inc<IncomeProgressComponent>()
            .Inc<UpgradeComponent>()
            .End();

        foreach (var entity in filter)
        {
            ref var business = ref businessPool.Get(entity);
            ref var level = ref levelPool.Get(entity);
            ref var progress = ref progressPool.Get(entity);
            ref var upgrade = ref upgradePool.Get(entity);

            data.businesses.Add(new BusinessSave
            {
                id = business.id,
                level = level.value,
                progress = progress.progress,
                upgrade1Bought = upgrade.upgrade1Bought,
                upgrade2Bought = upgrade.upgrade2Bought
            });
        }

        GameSaveService.Save(data);
    }
}