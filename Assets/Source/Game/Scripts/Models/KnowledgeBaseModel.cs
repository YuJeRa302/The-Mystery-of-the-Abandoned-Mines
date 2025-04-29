using System;

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