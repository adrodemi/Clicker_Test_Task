using Leopotam.EcsLite;

public class BuyUpgradeSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        var world = systems.GetWorld();

        var clickPool = world.GetPool<ClickBuyUpgradeComponent>();
        var refPool = world.GetPool<EcsEntityRefComponent>();
        var upgradePool = world.GetPool<UpgradeComponent>();
        var multiplierPool = world.GetPool<IncomeMultiplierComponent>();
        var businessPool = world.GetPool<BusinessComponent>();
        var balancePool = world.GetPool<BalanceComponent>();

        var playerEntity = GetPlayerEntity(world);
        if (playerEntity == -1 || !balancePool.Has(playerEntity)) return;

        ref var balance = ref balancePool.Get(playerEntity);

        var filter = world.Filter<ClickBuyUpgradeComponent>()
                          .Inc<EcsEntityRefComponent>()
                          .End();

        foreach (var eventEntity in filter)
        {
            ref var click = ref clickPool.Get(eventEntity);
            ref var targetRef = ref refPool.Get(eventEntity);
            int entity = targetRef.target;

            if (!upgradePool.Has(entity) || !businessPool.Has(entity) || !multiplierPool.Has(entity))
                continue;

            ref var upgrade = ref upgradePool.Get(entity);
            ref var multiplier = ref multiplierPool.Get(entity);
            ref var business = ref businessPool.Get(entity);

            if (click.upgradeIndex == 0 && !upgrade.upgrade1Bought)
            {
                float cost = business.upgrade1.cost;
                if (balance.value >= cost)
                {
                    balance.value -= cost;
                    upgrade.upgrade1Bought = true;
                    multiplier.value += business.upgrade1.incomeMultiplier / 100f;
                }
            }

            if (click.upgradeIndex == 1 && !upgrade.upgrade2Bought)
            {
                float cost = business.upgrade2.cost;
                if (balance.value >= cost)
                {
                    balance.value -= cost;
                    upgrade.upgrade2Bought = true;
                    multiplier.value += business.upgrade2.incomeMultiplier / 100f;
                }
            }

            world.DelEntity(eventEntity);
        }
    }

    private int GetPlayerEntity(EcsWorld world)
    {
        var filter = world.Filter<BalanceComponent>().End();
        foreach (var e in filter) return e;
        return -1;
    }
}