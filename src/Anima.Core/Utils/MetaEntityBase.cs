namespace Anima.Core.Utils;

public abstract class MetaEntityBase<TMeta> : EntityBase where TMeta : BaseMetaInfo
{
    
    [ForeignKey("Id")]
    public virtual TMeta MetaInfo {get;set;} = null!;
}