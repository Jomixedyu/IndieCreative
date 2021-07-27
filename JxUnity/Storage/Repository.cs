using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JxUnity.Storage
{
    public class RepositoryItemConfig
    {
        public string Id;
        public int StackMaxCount;
        public override string ToString()
        {
            return Id;
        }
    }

    public class RepositoryEventArgs
    {
        public string Id;
        public int ChangedCount;
        public bool IsPut;
        public bool IsTake;
        public string Feature;

        public RepositoryEventArgs Save()
        {
            return new RepositoryEventArgs()
            {
                Id = Id,
                ChangedCount = ChangedCount,
                IsPut = IsPut,
                IsTake = IsTake,
                Feature = Feature
            };
        }
        public override string ToString()
        {
            return string.Format("{{Id: {0}, Count: {1}, Feature: {2}}}", Id, ChangedCount, Id);
        }
    }

    public class Repository : IEnumerable<Repository.RepositoryItemInfoReadOnly>
    {
        public class RepositoryItemInfo : IComparable<RepositoryItemInfo>
        {
            public string Id = null;
            public int Count = 0;
            public string Feature = null;

            public RepositoryItemInfo(string id, string feature)
            {
                this.Id = id;
                this.Feature = feature;
            }
            public bool EqualsType(RepositoryItemInfo info)
            {
                return (this.Id == info.Id) && (this.Feature == info.Feature);
            }
            public int CompareTo(RepositoryItemInfo other)
            {
                if (this.Id == null)
                {
                    return 1;
                }
                if (this.Id == other.Id)
                {
                    if (this.Count > other.Count)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;

                    }
                }
                return this.Id.GetHashCode() > other.Id.GetHashCode() ? 1 : -1;
            }

            public override string ToString()
            {
                return string.Format("{{Id: {0}, Count: {1}, Feature: {2}}}", Id, Count, Feature);
            }
        }
        public struct RepositoryItemInfoReadOnly
        {
            private RepositoryItemInfo info;

            public bool HasItem { get => info != null && info.Count != 0; }

            public string Id { get => info?.Id; }
            public int Count { get => info.Count; }
            public string Feature { get => info?.Feature; }

            public RepositoryItemInfoReadOnly(RepositoryItemInfo info)
            {
                this.info = info;
            }
            public override string ToString()
            {
                return this.info?.ToString();
            }
        }

        protected int groupMaxCount;
        protected RepositoryItemInfo[] groups;

        protected RepositoryItemInfo FindGroup(string id, string feature)
        {
            foreach (RepositoryItemInfo item in this.groups)
            {
                if (item == null) continue;
                if (item.Id == id && item.Feature == feature)
                {
                    return item;
                }
            }
            return null;
        }
        protected RepositoryItemInfo FindNofullGroup(string id, string feature)
        {
            var maxCount = this.itemInfos[id].StackMaxCount;
            foreach (RepositoryItemInfo item in groups)
            {
                if (item == null) continue;
                if (item.Id == id && item.Feature == feature && item.Count < maxCount)
                {
                    return item;
                }
            }
            return null;
        }

        protected IEnumerator<RepositoryItemInfo> GetGroupIt(RepositoryItemInfo[] arr, string id, string feature)
        {
            foreach (RepositoryItemInfo item in arr)
            {
                if (item == null) continue;
                if (item.Id == id && item.Feature == feature)
                {
                    yield return item;
                }
            }
        }
        protected IEnumerator<RepositoryItemInfo> FindNofullGroupIt(string id, string feature)
        {
            var maxCount = this.itemInfos[id].StackMaxCount;
            foreach (RepositoryItemInfo item in groups)
            {
                if (item == null) continue;
                if (item.Id == id && item.Feature == feature && item.Count < maxCount)
                {
                    yield return item;
                }
            }
        }

        protected RepositoryEventArgs repoEventObj = new RepositoryEventArgs();

        public event Action<RepositoryEventArgs> OnItemsChanged;

        protected Dictionary<string, RepositoryItemConfig> itemInfos;

        protected RepositoryEventArgs GetEvent(string id, int count, string feature, bool isput)
        {
            this.repoEventObj.Id = id;
            this.repoEventObj.ChangedCount = count;
            this.repoEventObj.Feature = feature;
            this.repoEventObj.IsPut = isput;
            this.repoEventObj.IsTake = !isput;
            return this.repoEventObj;
        }

        public void RegisterItem(IEnumerable<RepositoryItemConfig> info, int groupMaxCount)
        {
            this.itemInfos = new Dictionary<string, RepositoryItemConfig>();
            foreach (var item in info)
            {
                this.itemInfos.Add(item.Id, item);
            }
            this.groupMaxCount = groupMaxCount;
            this.groups = new RepositoryItemInfo[groupMaxCount];
        }
        /// <summary>
        /// 查询是否可以放入对应数量的某样物品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public bool Putable(string id, int count = 1, string feature = null)
        {
            int space = 0;
            for (int i = 0; i < this.groupMaxCount; i++)
            {
                RepositoryItemInfo item = this.groups[i];
                if (item == null)
                {
                    space += this.itemInfos[id].StackMaxCount;
                }
                else if (item.Id == id && item.Feature == feature && item.Count < this.itemInfos[id].StackMaxCount)
                {
                    space += this.itemInfos[id].StackMaxCount - item.Count;
                }
                if (space >= count)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 查询是否可以拿走对应数量的某样物品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public bool Takeable(string id, int count = 1, string feature = null)
        {
            var it = this.GetGroupIt(this.groups, id, feature);
            int hasCount = 0;
            while (it.MoveNext())
            {
                hasCount += it.Current.Count;
                if (hasCount >= count)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 放入物品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public int PutItem(string id, int count = 1, string feature = null)
        {
            if (count <= 0)
            {
                return 0;
            }
            RepositoryItemConfig iteminfo = this.itemInfos[id];
            int putableCount = count;
            for (int i = 0; i < this.groupMaxCount; i++)
            {
                RepositoryItemInfo item = this.groups[i];

                if (item == null)
                {
                    item = new RepositoryItemInfo(id, feature);
                    this.groups[i] = item;
                    if (putableCount >= iteminfo.StackMaxCount)
                    {
                        item.Count = iteminfo.StackMaxCount;
                        putableCount -= item.Count;
                    }
                    else
                    {
                        item.Count += putableCount;
                        putableCount = 0;
                    }
                }
                else if (item.Id == id && item.Feature == feature && item.Count < iteminfo.StackMaxCount)
                {
                    if (item.Count < iteminfo.StackMaxCount)
                    {
                        int ableCount = iteminfo.StackMaxCount - item.Count;
                        if (putableCount >= ableCount)
                        {
                            item.Count = iteminfo.StackMaxCount;
                            putableCount -= ableCount;
                        }
                        else
                        {
                            item.Count -= putableCount;
                            putableCount = 0;
                        }
                    }
                }

                if (putableCount <= 0)
                {
                    break;
                }
            }
            putableCount = Math.Max(putableCount, 0);
            this.OnItemsChanged?.Invoke(this.GetEvent(id, count - putableCount, feature, true));
            return putableCount;
        }
        /// <summary>
        /// 拿走物品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <param name="feature"></param>
        public void TakeItem(string id, int count = 1, string feature = null)
        {
            int taked = count;
            for (int i = 0; i < this.groupMaxCount; i++)
            {
                var item = this.groups[i];
                if (item.Id == id && item.Feature == feature)
                {
                    if (taked <= item.Count)
                    {
                        item.Count -= taked;
                        taked = 0;
                    }
                    else
                    {
                        taked -= item.Count;
                        item.Count = 0;
                        this.groups[i] = null;
                    }
                }
                if (taked <= 0)
                {
                    break;
                }
            }
            taked = Math.Max(taked, 0);
            this.OnItemsChanged?.Invoke(this.GetEvent(id, count - taked, feature, false));
        }
        /// <summary>
        /// 物品是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public bool IsExist(string id, string feature = null)
        {
            return this.FindGroup(id, feature) != null;
        }
        /// <summary>
        /// 获取某个物品的数量
        /// </summary>
        /// <param name="id"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public int Count(string id, string feature = null)
        {
            var it = this.GetGroupIt(this.groups, id, feature);
            int hasCount = 0;
            while (it.MoveNext()) hasCount += it.Current.Count;
            return hasCount;
        }
        /// <summary>
        /// 整理仓库，物体自动叠加，并且按类型存放
        /// </summary>
        public void Arrange()
        {
            var clone = this.groups;
            this.groups = new RepositoryItemInfo[this.groupMaxCount];
            int groupIndex = 0;
            for (int i = 0; i < clone.Length; i++)
            {
                if (clone[i] == null)
                {
                    continue;
                }
                var cfg = this.itemInfos[clone[i].Id];
                if (clone[i].Count == cfg.StackMaxCount)
                {
                    this.groups[groupIndex] = clone[i];
                    clone[i] = null;
                    groupIndex++;
                    continue;
                }
                this.groups[groupIndex] = clone[i];
                for (int af = i + 1; af < clone.Length; af++)
                {
                    if (clone[af] == null)
                    {
                        continue;
                    }
                    if (clone[af].EqualsType(clone[i]))
                    {
                        int need = cfg.StackMaxCount - clone[i].Count;
                        if (need > clone[af].Count)
                        {
                            clone[i].Count += clone[af].Count;
                            clone[af].Count = 0;
                        }
                        else
                        {
                            clone[i].Count += need;
                            clone[af].Count -= need;
                        }
                        if (clone[af].Count == 0)
                        {
                            clone[af] = null;
                        }
                        if (clone[i].Count == cfg.StackMaxCount)
                        {
                            groupIndex++;
                            break;
                        }
                    }
                }

            }
            Array.Sort(this.groups);
        }

        public IEnumerator<RepositoryItemInfoReadOnly> GetEnumerator()
        {
            foreach (RepositoryItemInfo item in this.groups)
            {
                yield return new RepositoryItemInfoReadOnly(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}