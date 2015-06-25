using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// A non-instantiable base entity which defines 
/// members available across all entities.
/// </summary>

[DataContract]
public abstract class EntityBase
{
    public Guid Id { get; set; }
    public DateTime lastUpdatedTimestamp { get; set; }

}