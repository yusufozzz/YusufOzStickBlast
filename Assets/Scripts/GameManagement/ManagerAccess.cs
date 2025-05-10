using System.Collections.Generic;

namespace GameManagement
{
    public static class ManagerAccess
    {
        private static readonly Dictionary<ManagerType, ManagerBase> Managers = new();

        public static void SetManagers(Dictionary<ManagerType, ManagerBase> managers)
        {
            foreach (var manager in managers)
            {
                Managers[manager.Key] = manager.Value;
            }
        }

        public static T GetManager<T>(this ManagerType managerType) where T : ManagerBase
        {
            if (Managers.TryGetValue(managerType, out var manager))
            {
                return (T)manager;
            }

            throw new KeyNotFoundException($"Manager of type {typeof(T).Name} not found.");
        }
    }
}