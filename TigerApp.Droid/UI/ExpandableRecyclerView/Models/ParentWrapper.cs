namespace TigerApp.Droid.UI.ExpandableRecyclerView.Models
{
    public class ParentWrapper
    {
        public ParentWrapper(object parentObject, long stableId)
        {
            ParentObject = parentObject;
            StableId = stableId;
            Expanded = false;
        }

        public object ParentObject { get; set; }

        public bool Expanded { get; set; }

        public long StableId { get; set; }
    }
}