// First class functionality for a the results of an item being used
public class ItemUseResult : ActionResult {
    Item item;

    // Constructor
    public ItemUseResult(Actor actor, Item item, BaseAction action)
        : base(actor, action)
    {
        this.item = item;
    }

    // Finish this action's result
    protected override void CompleteAction()
    {
        GameLog.Send(item.OnUseString, MessageColour.White);
        ConsumeItem();
        base.CompleteAction();
    }

    // Consume this item after successful use
    private void ConsumeItem()
    {
        actor.RemoveItem(item);
    }
}
