namespace TigerApp.Droid.UI.ExpandableRecyclerView.ClickListeners
{
    public interface IExpandCollapseListener
    {
        void OnRecyclerViewItemExpanded(int position);

        void OnRecyclerViewItemCollapsed(int position);
    }
}