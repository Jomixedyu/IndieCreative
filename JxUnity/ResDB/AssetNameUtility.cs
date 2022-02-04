using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using JxUnity.ResDB.Private;

namespace JxUnity.ResDB
{
    /// <summary>
    /// 路径命名规则：
    /// FULL: 一个资产的完全路径，
    /// PROJ: 项目路径，
    /// ROOT: 从项目根目录开始的相对路径，
    /// ASSET: 从Assets目录开始的相对路径。
    /// </summary>
    public static partial class AssetNameUtility
    {
        private static readonly int AssetsPathLength = "Assets".Length + 1;

        /// <summary>
        /// 将路径格式化为ab包格式的路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string FormatBundleName(string path)
        {
            path = path.Replace('\\', '/');

            var abExt = ResDBConfig.Variant != null ? "." + ResDBConfig.Variant : string.Empty;

            return $"{path}{abExt}";
        }
        /// <summary>
        /// 将ab包格式的路径格式化为普通路径
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static string UnformatBundleName(string bundleName)
        {
            var name = Path.GetFileName(bundleName);
            if (!string.IsNullOrEmpty(ResDBConfig.Variant))
            {
                var ext = "." + ResDBConfig.Variant;
                if (!name.EndsWith(ext))
                {
                    throw new ArgumentException("bundle ext name is not exist");
                }
                name = name.Substring(0, name.Length - ext.Length);
            }
            var dir = Path.GetDirectoryName(bundleName);
            return Path.Combine(dir, name).Replace('\\', '/');
        }

        /// <summary>
        /// 用ab包全名获取去掉ab后缀的路径
        /// </summary>
        /// <param name="bundleName"></param>
        /// <returns></returns>
        public static string GetBundleNameWithoutVariant(string bundleName)
        {
            string ext = "." + ResDBConfig.Variant;
            if (bundleName.EndsWith(ext))
            {
                return bundleName.Substring(0, bundleName.Length - ext.Length);
            }
            return bundleName;
        }

        public static string ROOTToBundleName(string _ROOT)
        {
            return ROOTToASSET(_ROOT).ToLower();
        }

        public static string ROOTToASSET(string _ROOT)
        {
            if (!_ROOT.StartsWith("Assets", true, null))
            {
                return _ROOT;
            }
            return _ROOT.Substring(AssetsPathLength);
        }


        public static string BundleNameToROOT(string bundleName)
        {
            return "Assets/" + BundleNameToASSET(bundleName);
        }
 
        public static string BundleNameToASSET(string bundleName)
        {
            return UnformatBundleName(bundleName);
        }

        public static string GetShortName(string name)
        {
            return Path.GetFileNameWithoutExtension(name);
        }


#if UNITY_EDITOR

        /// <summary>
        /// FullName返回AssetsFullName
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public static string FULLToASSET(string fullName)
        {
            if (fullName.Length == Application.dataPath.Length)
            {
                return string.Empty;
            }
            return fullName.Substring(Application.dataPath.Length + 1).Replace('\\', '/');
        }


        public static string FULLToROOT(string full)
        {
            string proj = GetPROJ();
            if (proj == full)
            {
                return string.Empty;
            }
            return full.Substring(proj.Length + 1).Replace('\\', '/');
        }


        /// <summary>
        /// 获取项目文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string GetPROJ()
        {
            return Application.dataPath.Substring(0, Application.dataPath.IndexOf("/Assets"));
        }

#endif
    }
}