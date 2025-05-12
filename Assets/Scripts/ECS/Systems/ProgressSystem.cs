using Leopotam.EcsLite;
using UnityEngine;

public class ProgressSystem : IEcsRunSystem
{
    public void Run(IEcsSystems systems)
    {
        var world = systems.GetWorld();

        var businessPool = world.GetPool<BusinessComponent>();
        var levelPool  = world.GetPool<LevelComponent>();
        var progressPool = world.GetPool<IncomeProgressComponent>();
        var multiplierPool = world.GetPool<IncomeMultiplierComponent>();
        var balancePool = world.GetPool<BalanceComponent>();

        var businessFilter = world.Filter<BusinessComponent>()
            .Inc<LevelComponent>()
            .Inc<IncomeProgressComponent>()
            .Inc<IncomeMultiplierComponent>()
            .End();

        float deltaTime = Time.deltaTime;

        foreach (var entity in businessFilter)
        {
            ref var business = ref businessPool.Get(entity);
            ref var level = ref levelPool.Get(entity);
            ref var progress = ref progressPool.Get(entity);
            ref var multiplier = ref multiplierPool.Get(entity);

            if (level.value <= 0) continue;

            progress.progress += deltaTime;

            if (progress.progress >= business.delay)
            {
                float income = level.value * business.baseIncome * multiplier.value;

                var playerEntity = GetPlayerEntity(world);
                if (playerEntity != -1)
                {
                    ref var balance = ref balancePool.Get(playerEntity);
                    balance.value += income;
                }

                progress.progress = 0f;
            }
        }
    }

    private int GetPlayerEntity(EcsWorld world)
    {
        var balancePool = world.GetPool<BalanceComponent>();
        var playerFilter = world.Filter<BalanceComponent>().End();

        foreach (var entity in playerFilter)
            return entity;

        return -1;
    }
}