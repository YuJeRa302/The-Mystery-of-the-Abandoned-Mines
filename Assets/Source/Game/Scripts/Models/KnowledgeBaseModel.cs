using System;

namespace Assets.Source.Game.Scripts.Models
{
    public class KnowledgeBaseModel : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}