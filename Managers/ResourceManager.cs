using System.Diagnostics.Contracts;
using Monolith.IO;

namespace Monolith.Managers
{
    public class ResourceManager
    {
        private IContentProvider Provider { get; set; }
         
        /// <summary>
        /// The root path of content.
        /// </summary>
        public string ContentRoot { get; set; } = "Content";

        public ResourceManager()
        {
            Engine.Instance.Content.RootDirectory = ContentRoot;
            
            Provider = new PipelineLoader();
        }

        /// <summary>
        /// Sets the current ContentProvider.
        /// </summary>
        /// <param name="newProvider">The provider which will be used</param>
        public void SetProivder(IContentProvider newProvider)
        {
            Provider = newProvider;
        }

        /// <summary>
        /// Generic loader function that uses the current provider.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Load<T>(string path)
        {
            return Provider.Load<T>(path);
        }

        /// <summary>
        /// Generic loader function that uses a given provider, passes the give provider to the current one.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public T Load<T>(string path, IContentProvider provider)
        {
            Provider = provider;

            return Load<T>(path);
        }

        public void Unload(string path)
        {
            Provider.Unload(path);
        }
        public void ClearCache()
        {
            Provider.ClearCache();
        }
    }
}
