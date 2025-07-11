using System;

namespace Assets.Source.Game.Scripts
{
    public class KnowledgeBaseModel : IDisposable
    {
        public KnowledgeBaseModel()
        {
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}