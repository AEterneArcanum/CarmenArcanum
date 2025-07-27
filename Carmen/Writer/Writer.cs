using Arcane.Carmen.AST;

namespace Arcane.Carmen.Writer
{
    public abstract class Writer : IWriter
    {
        /// <summary>
        /// Writer log book.
        /// </summary>
        public WriterLog Log { get; init; }

        /// <summary>
        /// Error occured within the writer.
        /// </summary>
        public event Action<string>? OnWriterError;
        /// <summary>
        /// Write an error to the log.
        /// </summary>
        /// <param name="message"></param>
        private protected void LogError(string message)
        {
            Log.Log(message, Logging.LogLevel.Error);
            OnWriterError?.Invoke(message);
        }

        protected Writer()
        {
            Log = new WriterLog();
        }

        /// <summary>
        /// Clear the writer's buffer.
        /// </summary>
        public abstract void Clear();
        /// <summary>
        /// Ensure all nodes in collection are valid on this writer.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public bool ValidateNodes(IEnumerable<ASTNode> nodes)
        {
            bool clean = true;
            foreach (ASTNode node in nodes)
            {
                if (!ValidateNode(node))
                {
                    LogError($"Node {node.GetType()} not supported!"); 
                    clean = false; 
                    continue;
                }
                else if (node is IHasInnerNodes container && !ValidateNodes(container.Children))
                {
                    clean = false; 
                    continue;
                }
            }
            return clean;
        }
        /// <summary>
        /// Ensure target node is valid on this writer.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public abstract bool ValidateNode(ASTNode node);
        /// <summary>
        /// Write all nodes in the collection to the buffer.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="filename"></param>
        /// <param name="sexpr"></param>
        /// <param name="clear"></param>
        public void WriteNodes(IEnumerable<ASTNode> nodes)
        {
            foreach (var node in nodes)
            {
                WriteNode(node);
            }
        }
        /// <summary>
        /// Write target node to the writer's buffer.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public abstract void WriteNode(ASTNode node, bool isSubExpression = false);
        /// <summary>
        /// Save the written buffer to target file.
        /// </summary>
        /// <param name="filename"></param>
        public abstract void SaveFile(string filename);
    }
}
