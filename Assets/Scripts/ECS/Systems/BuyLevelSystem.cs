using Leopotam.EcsLite;
using UnityEngine;

public class BuyLevelSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        var world = systems.GetWorld();

        var clickPool = world.GetPool<ClickBuyLevelComponent>();
        var refPool = world.GetPool<EcsEntityRefComponent>();
        var levelPool = world.GetPool<LevelComponent>();
        var businessPool = world.GetPool<BusinessComponent>();
        var balancePool = world.GetPool<BalanceComponent>();

        int playerEntity = GetPlayerEntity(world);
        if (playerEntity == -1 || !balancePool.Has(playerEntity))
        {
            Debug.LogWarning("Баланс игрока не найден. Покупка уровня невозможна.");
            return;
        }

        ref var balance = ref balancePool.Get(playerEntity);

        var filter = world.Filter<ClickBuyLevelComponent>()
                          .Inc<EcsEntityRefComponent>()
                          .End();

        foreach (var eventEntity in filter)
        {
            ref var targetRef = ref refPool.Get(eventEntity);
            int targetEntity = targetRef.target;

            if (!levelPool.Has(targetEntity) || !businessPool.Has(targetEntity))
            {
                Debug.LogWarning($"Целевая сущность {targetEntity} не содержит нужных компонентов.");
                continue;
            }

            ref var level = ref levelPool.Get(targetEntity);
            ref var business = ref businessPool.Get(targetEntity);

            float cost = (level.value + 1) * business.baseCost;

            if (balance.value >= cost)
            {
                balance.value -= cost;
                level.value++;
                Debug.Log($"Куплен уровень {level.value} для бизнеса {business.id}");
            }
            else
            {
                Debug.Log("Недостаточно средств для покупки уровня.");
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