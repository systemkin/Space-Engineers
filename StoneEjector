//Name Conveyor Sorter as "Stone Sorter"
//Name Connector as Stone Ejector
//Or make your own names at 18 and 19 lines
public Program()
{
    Runtime.UpdateFrequency=UpdateFrequency.Update100;
}
public void Main(string arg, UpdateType updateSource)
{
    List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> cockpits = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> drills = new List<IMyTerminalBlock>();

    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(containers);
    GridTerminalSystem.GetBlocksOfType<IMyCockpit>(cockpits);  
    GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(drills);
    
    IMyConveyorSorter StoneSorter = GridTerminalSystem.GetBlockWithName("Stone Sorter") as IMyConveyorSorter;
    IMyShipConnector StoneEjector = GridTerminalSystem.GetBlockWithName("Stone Ejector") as IMyShipConnector;

    bool stones = false;
    MyItemType StoneItem = new MyItemType("MyObjectBuilder_Ore", "Stone");

    MyInventoryItemFilter StoneFilterItem = new MyInventoryItemFilter(StoneItem);

    List<MyInventoryItemFilter> FilterListStone = new List<MyInventoryItemFilter>{StoneFilterItem};

    foreach (IMyTerminalBlock p in containers)
        if(Me.CubeGrid.ToString() == p.CubeGrid.ToString())
            if(((double) p.GetInventory().GetItemAmount(StoneItem)) > 0) {
                stones = true;
                break;
            }
    if (!stones)
    foreach (IMyTerminalBlock p in cockpits)  
        if(Me.CubeGrid.ToString() == p.CubeGrid.ToString())
            if(((double) p.GetInventory().GetItemAmount(StoneItem)) > 0) {
                stones = true;
                break;
            }
    if (!stones)
    foreach (IMyTerminalBlock p in drills)  
        if(Me.CubeGrid.ToString() == p.CubeGrid.ToString())
            if(((double) p.GetInventory().GetItemAmount(StoneItem)) > 0) {
                stones = true;
                break;
            }
    List<MyInventoryItem> EjecorInventoryList = new List<MyInventoryItem>();
    StoneEjector.GetInventory().GetItems(EjecorInventoryList);

    if(
    ((stones) && (EjecorInventoryList.Count == 0)) 
        ||
    ((StoneEjector.GetInventory().GetItemAmount(StoneItem) > 0) && (EjecorInventoryList.Count == 1))
    )
    {
        StoneSorter.SetFilter(MyConveyorSorterMode.Whitelist, FilterListStone); 
        StoneEjector.CollectAll = true;
        StoneEjector.ThrowOut = true;
    }
    else {
        StoneEjector.CollectAll = false;
        StoneEjector.ThrowOut = false;
        StoneSorter.SetFilter(MyConveyorSorterMode.Blacklist, FilterListStone);
    }

}
