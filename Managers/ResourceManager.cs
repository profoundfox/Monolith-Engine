using System.Diagnostics.Contracts;
using Monolith.IO;

namespace Monolith.Managers
{
    public class ResourceManager
    {
        public IContentProvider Provider { get; private set; }

        internal string ContentRoot { get; set; } = "Content";

        public ResourceManager()
        {
            Engine.Instance.Content.RootDirectory = ContentRoot;
            
            Provider = new ContentPipelineLoader();
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

        /// <summary>
        /// Generic loader function for the ContentPipeline.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T PipelineLoad<T>(string path)
        {
            Provider = new ContentPipelineLoader();

            return Load<T>(path);
        }
        
        /// <summary>
        /// Generic loader function for raw content, takes in all parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path of the asset in question, relative to rootDir</param>
        /// <param name="rootDir">The root of wher the asset is located, in tandem with the absolutePath</param>
        /// <param name="absolutePath">Wether it should be relative to the root of the hard drive</param>
        /// <returns></returns>
        public T RuntimeLoad<T>(string path, string rootDir, bool absolutePath)
        {
            Provider = new RuntimeContentLoader(rootDir, absolutePath);

            return Load<T>(path);
        }

        /// <summary>
        /// Generic loader function for raw content, takes in only the path, relative to the default root.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path of the asset, relative to the default directory</param>
        /// <returns></returns>
        public T RuntimeLoad<T>(string path)
        {
            return RuntimeLoad<T>(path, ContentRoot, false);
        }

        /// <summary>
        /// Generic loader function for raw content, takes in parameters for loading from an absolute path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">The path of the asset relative to the root directory</param>
        /// <param name="rootDir">The root directory/param>
        /// <returns></returns>
        public T RuntimeLoad<T>(string path, string rootDir)
        {
            return RuntimeLoad<T>(path, rootDir, true);
        }
    }
}