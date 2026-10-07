namespace Anima.Core.Utils;

public abstract class MetaEntity<TMeta> : EntityBase where TMeta : BaseMetaInfo
{
    
    [ForeignKey("Id")]
    public virtual TMeta MetaInfo {get;set;} = null!;
}