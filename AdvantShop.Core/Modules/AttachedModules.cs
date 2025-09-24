//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Modules
{
    public class AttachedModules
    {
        private static List<Type> _allModules;
        private static List<Module> _activeModules;
        private static bool _isLoaded;
        private static readonly ConcurrentDictionary<string, List<Type>> _moduleTypesByInterface = new ConcurrentDictionary<string, List<Type>>();
        private static readonly ConcurrentDictionary<string, List<Type>> _coreTypesByInterface = new ConcurrentDictionary<string, List<Type>>();
        private static readonly ConcurrentDictionary<string, Type> _typeByStringId = new ConcurrentDictionary<string, Type>();

        private static readonly object SyncCore = new object();

        public static void LoadModules()
        {
            _allModules = new List<Type>();
            _activeModules = new List<Module>();

            var moduleType = typeof(IModuleBase);

            try
            {
                foreach (var assembly in AppDomain.CurrentDomain
                             .GetAssemblies()
                             .Where(item => item.FullName.Contains("AdvantShop.Module")
                                            && !item.FullName.Contains(".CRUSHED")))
                {
                    try
                    {
                        var moduleEntryPointType = assembly
                            .GetTypes()
                            .FirstOrDefault(t => t.GetInterface(moduleType.Name) != null && t.IsClass);
                            
                        if (moduleEntryPointType != null)
                            _allModules.Add(moduleEntryPointType);
                    }
                    catch (Exception exception)
                    {
                        if (exception is ReflectionTypeLoadException typeLoadException)
                        {
                            foreach (var loaderException in typeLoadException.LoaderExceptions)
                            {
                                Debug.Log.Warn("exception at loading module " + assembly.FullName, loaderException);
                            }
                        }
                        else
                        {
                            Debug.Log.Warn("exception at loading module " + assembly.FullName, exception);
                        }

                        var crashedFile = assembly.CodeBase.Replace("file:///", "").Replace(".DLL", ".dll");
                        FileHelpers.DeleteFile(crashedFile + ".CRUSHED");
                        FileHelpers.RenameFile(crashedFile, crashedFile + ".CRUSHED");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error("exception at global modules loading", ex);
                _isLoaded = true;
            }

            _activeModules = ModulesRepository.GetModulesFromDb().Where(m => m.IsInstall && m.Active).ToList();
            _moduleTypesByInterface.Clear();
            _typeByStringId.Clear();
            _coreTypesByInterface.Clear();
            
            _isLoaded = true;
        }
        
        public static List<T> GetModuleInstances<T>()
        {
            var types = GetModules<T>(false);
            if (types == null || types.Count == 0)
                return null;

            var result = new List<T>();

            for (var i = 0; i < types.Count; i++)
            {
                if (types[i] == null)
                    continue;

                var instance = Activator.CreateInstance(types[i]);
                if (instance == null)
                    continue;

                result.Add((T)instance);
            }

            return result;
        }

        public static List<T> GetModuleInstancesWithCore<T>()
        {
            var modules = GetModuleInstances<T>();
            var coreTypes = GetCore<T>();

            if ((modules == null || modules.Count == 0) &&
                (coreTypes == null || coreTypes.Count == 0))
            {
                return null;
            }

            if (modules == null)
                modules = new List<T>();

            if (coreTypes != null && coreTypes.Count > 0)
            {
                for (var i = 0; i < coreTypes.Count; i++)
                {
                    if (coreTypes[i] == null)
                        continue;

                    var instance = Activator.CreateInstance(coreTypes[i]);
                    if (instance == null)
                        continue;

                    modules.Add((T)instance);
                }
            }

            return modules;
        }

        /// <summary>
        /// Get active modules
        /// </summary>
        public static List<Type> GetModules<T>()
        {
            return GetModules<T>(false);
        }

        public static List<Type> GetModules<T>(bool ignoreActive)
        {
            if (!_isLoaded || _activeModules == null || _allModules == null)
                LoadModules();

            if (_activeModules == null || _allModules == null)
            {
                return null;
            }

            var type = typeof(T);

            return _moduleTypesByInterface.GetOrAdd(type.Name + "_" + ignoreActive, key =>
                {
                    var modules =
                        _allModules.Where(
                                x => x != null
                                     && type.IsAssignableFrom(x)
                                     && (ignoreActive
                                         || _activeModules.Any(m => String.Equals(x.Name, m.StringId, StringComparison.OrdinalIgnoreCase))))
                            .ToList();

                    return modules;
                }
            );
        }

        /// <summary>
        /// Get module by id
        /// </summary>
        public static Type GetModuleById(string stringId, bool active = false)
        {
            if (!_isLoaded || _activeModules == null || _allModules == null)
                LoadModules();

            if (_activeModules == null || _allModules == null)
            {
                return null;
            }

            return _typeByStringId.GetOrAdd(stringId + "_" + active, key =>
            {
                var module =
                    _allModules.FirstOrDefault(x => x != null && String.Equals(x.Name, stringId, StringComparison.OrdinalIgnoreCase));

                if (active)
                    return module != null 
                           && _activeModules.Any(x => x != null && String.Equals(module.Name, x.StringId, StringComparison.OrdinalIgnoreCase))
                        ? module
                        : null;

                return module;
            });
        }

        public static List<Type> GetCore<T>()
        {
            var type = typeof(T);

            return _coreTypesByInterface.GetOrAdd(type.Name, key =>
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => x != null 
                                && x.FullName.StartsWith("AdvantShop") 
                                && !x.FullName.Contains("AdvantShop.Module"))
                    .SelectMany(a => a.GetTypes())
                    .Where(x => x != null && x.IsClass && !x.IsAbstract && type.IsAssignableFrom(x))
                    .ToList();
            });
        }

        public static IEnumerable<Type> GetModules()
        {
            if (!_isLoaded || _allModules == null)
                LoadModules();

            if (_allModules == null)
                _allModules = new List<Type>();

            return _allModules.Where(x => x != null);
        }

        public static List<Type> GetAllModules<T>()
        {
            var type = typeof(T);
            return GetModules().Where(x => x != null && type.IsAssignableFrom(x)).ToList();
        }
    }
}
