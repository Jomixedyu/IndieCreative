using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace JxUnity.Pool
{
    public static class GameObjectPool
    {
        public static bool IsEnabled()
        {
            return GameObjectPoolMono.HasInstance;
        }
        /// <summary>
        /// 按对象获取池类型，如果不存在，则返回null
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static string GetObjectType(GameObject go)
        {
            return GameObjectPoolMono.Instance.GetObjectType(go);
        }

        private static void DefaultGetCallBack(GameObject gameObject)
        {
            gameObject.SetActive(true);
        }
        private static void DefaultRecycleCallBack(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        private static GameObjectPoolItem Internal_Register(
            string type,
            GameObject go,
            Action<GameObject> getCb,
            Action<GameObject> recycleCb)
        {
            var inst = GameObjectPoolMono.Instance;
            if (inst.IsExist(type))
            {
                return null;
            }
            return inst.Register(type, go, getCb, recycleCb);
        }

        /// <summary>
        /// 注册池对象，如果对象池已存在，则返回null。
        /// </summary>
        /// <param name="type">池类型</param>
        /// <param name="go">对象原型</param>
        /// <param name="getCb">获取回调</param>
        /// <param name="recycleCb">回收回调</param>
        /// <returns></returns>
        public static GameObjectPoolItem Register(
            string type,
            GameObject go,
            Action<GameObject> getCb,
            Action<GameObject> recycleCb)
        {
            return Internal_Register(type, go, getCb, recycleCb);
        }
        /// <summary>
        /// 注册池对象，物体将自动激活与反激活。如果对象池已存在，则返回null。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="go"></param>
        /// <returns></returns>
        public static GameObjectPoolItem RegisterDefault(string type, GameObject go)
        {
            return Internal_Register(type, go, DefaultGetCallBack, DefaultRecycleCallBack);
        }

        /// <summary>
        /// 获取一个池内对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static GameObject Get(string type)
        {
            return GameObjectPoolMono.Instance.Get(type);
        }
        public static int GetCount(string type)
        {
            return GameObjectPoolMono.Instance.GetCount(type);
        }
        public static GameObjectPoolItem GetPool(string type)
        {
            return GameObjectPoolMono.Instance.GetPool(type);
        }
        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="go"></param>
        public static void Recycle(GameObject go)
        {
            GameObjectPoolMono.Instance.Recycle(go);
        }

        /// <summary>
        /// 卸载未在使用的池对象
        /// </summary>
        /// <param name="type"></param>
        public static void UnloadUnused(string type)
        {
            GameObjectPoolMono.Instance.UnloadUnused(type);
        }
        /// <summary>
        /// 强制卸载池内对象
        /// </summary>
        /// <param name="type"></param>
        public static void ForceUnload(string type)
        {
            GameObjectPoolMono.Instance.ForceUnload(type);
        }
        /// <summary>
        /// 强制卸载所有池内对象
        /// </summary>
        public static void ForceUnloadAll()
        {
            GameObjectPoolMono.Instance.ForceUnloadAll();
        }
        /// <summary>
        /// 删除池
        /// </summary>
        /// <param name="type"></param>
        public static void Delete(string type)
        {
            GameObjectPoolMono.Instance.Delete(type);
        }

        /// <summary>
        /// 删除所有池
        /// </summary>
        public static void DeleteAll()
        {
            GameObjectPoolMono.Instance.DeleteAll();
        }
        /// <summary>
        /// 遍历池
        /// </summary>
        /// <param name="act"></param>
        public static void ForEach(Action<GameObjectPoolItem> act)
        {
            GameObjectPoolMono.Instance.ForEach(act);
        }
        /// <summary>
        /// 当前池内可用
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int GetUsableCount(string type)
        {
            return GameObjectPoolMono.Instance.GetUsableCount(type);
        }
    }
}