using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
namespace MoreMountains.Tools
{
    //! AI action으로 원거리 무기를 스왑할 때, obj pooler가 중복해 생성되는 걸 방지합니다
    //! 원거리 무기 simplepooler에 대신 넣어주세요
    //* 그냥 ObjPooler가 여러개 생기는 모든 상황에 넣으면 됨
    public class CustomSimpleObjPooler : MMSimpleObjectPooler
    {
        public override void FillObjectPool()
        {
            if (GameObjectToPool == null)
            {
                return;
            }

            if ((_objectPool != null) && (_objectPool.PooledGameObjects.Count >= PoolSize))
            {
                return;
            }

            // Existing pool name
            string poolName = DetermineObjectPoolName();

            // Check if a pool with the desired name already exists
            GameObject existingPool = GameObject.Find(poolName);
            if (existingPool != null)
            {
                // If the pool already exists, we set our object pool reference to the existing pool and exit the method
                _objectPool = existingPool.GetComponent<MMObjectPool>();
                return;
            }

            // If we reach this point, it means no existing pool was found, so we create a new one
            CreateWaitingPool();

            int objectsToSpawn = PoolSize - _objectPool.PooledGameObjects.Count;

            for (int i = 0; i < objectsToSpawn; i++)
            {
                AddOneObjectToThePool();
            }
        }

    }
}