using System;
using System.Collections.Generic;

namespace RagazziStudios.Core.Infrastructure
{
    /// <summary>
    /// Service Locator central para acessar serviços de infraestrutura.
    /// Permite registrar e resolver interfaces (IAdsService, IStorageService, etc.)
    /// sem acoplamento direto às implementações.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        /// <summary>
        /// Registra uma implementação para uma interface.
        /// </summary>
        public static void Register<T>(T service) where T : class
        {
            var type = typeof(T);
            _services[type] = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Retorna a implementação registrada para a interface.
        /// </summary>
        public static T Get<T>() where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out object service))
            {
                return service as T;
            }

            throw new InvalidOperationException(
                $"Service '{type.Name}' not registered in ServiceLocator. " +
                $"Call ServiceLocator.Register<{type.Name}>() first.");
        }

        /// <summary>
        /// Tenta retornar a implementação, sem exception se não existir.
        /// </summary>
        public static bool TryGet<T>(out T service) where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out object obj))
            {
                service = obj as T;
                return service != null;
            }

            service = null;
            return false;
        }

        /// <summary>
        /// Verifica se um serviço está registrado.
        /// </summary>
        public static bool Has<T>() where T : class =>
            _services.ContainsKey(typeof(T));

        /// <summary>
        /// Remove o registro de um serviço.
        /// </summary>
        public static void Unregister<T>() where T : class =>
            _services.Remove(typeof(T));

        /// <summary>
        /// Remove todos os serviços registrados.
        /// Útil para testes e reset.
        /// </summary>
        public static void Clear() => _services.Clear();
    }
}
