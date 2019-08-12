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
        base.CompleteAction();
        GameLog.Send(item.OnUseString, MessageColour.White);
        ConsumeItem();
    }

    // Consume this item after successful use
    void ConsumeItem()
    {
        actor.RemoveItem(item);
    }
}
