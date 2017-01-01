using System.Collections.Generic;

namespace TigerApp.Droid.UI.ExpandableRecyclerView.Models
{
    public interface IParentObject
    {
        List<object> ChildObjectList { get;}
    }
}