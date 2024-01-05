//Are you need Info about battery
bool PowerCheckBool = true;

//Same for Ice
bool IceCountBool = false;

//Same for fullfilness
bool FullfillnessBool = true;

/* Do you need auto stone ejection
if true, then:
1.Name stone conveyor-sorter as a "Stone Sorter"
2.Name Ejector-connector as "Stone Ejector" */

bool StoneEjectBool = true;

//Cockpit number
int CockpitId = 0;

//Surface numbers
int SurfPowerInt = 1;
int SurfIceInt = 2;
int SurfFullfilnessInt = 3;

Color White = new Color(255,255,255);
Color WarningColor = new Color(255,140,0);
Color EmergencyColor = new Color(255,69,0);
Color Green = new Color(50,205,50);

List<IMyTerminalBlock> GetContainers(){
    List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> cockpits = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> drills = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> generators = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> connectors = new List<IMyTerminalBlock>();

    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(containers);
    GridTerminalSystem.GetBlocksOfType<IMyCockpit>(cockpits);
    GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(drills);
    GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(generators);
    GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors);

    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        //IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName("Containers");
        //group.GetBlocks(blocks);

    blocks.AddRange(containers);
    blocks.AddRange(cockpits);
    blocks.AddRange(drills);
    blocks.AddRange(connectors);

    return blocks;
}
double PowerCheck(IMyTextSurface surface = null) {
    double PowerIn = 0;
    double PowerOut = 0;
    double PowerStored = 0;
    double MaxPowerCap = 0;
    double AllGridsStored = 0;
    double AllGridsCap = 0;

     List<IMyTerminalBlock> batterys = new List<IMyTerminalBlock>();
     GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batterys);

    foreach (IMyBatteryBlock battery in batterys)
        {
            if(Me.CubeGrid.ToString() == battery.CubeGrid.ToString()) {
                PowerIn += battery.CurrentInput;
                PowerOut += battery.CurrentOutput;
                PowerStored += battery.CurrentStoredPower;
                MaxPowerCap += battery.MaxStoredPower;
            }
            AllGridsStored += battery.CurrentStoredPower;
            AllGridsCap += battery.MaxStoredPower;
        }
    if(surface == null)
        return 100*PowerStored/MaxPowerCap;
    surface.ContentType = ContentType.TEXT_AND_IMAGE;
    surface.FontColor = White;
    surface.FontSize= 4;
    double hours = PowerStored/(PowerOut-PowerIn);
    if(PowerStored/MaxPowerCap > 0.999) {
        surface.FontColor = Green;
        if (AllGridsStored/AllGridsCap > 0.999)
            surface.WriteText("All grids\ncharged");
        else
            surface.WriteText("This grid\ncharged");
    }
    else if (PowerIn > PowerOut)
        surface.WriteText("Charging\n" + (100*PowerStored/MaxPowerCap).ToString("0") + "%");
    else {
        surface.FontColor = White;
        if (hours < 5/60)
            surface.FontColor = WarningColor;
        if (hours < 2/60)
            surface.FontColor = EmergencyColor;
        if (hours > 1)
            surface.WriteText((100*PowerStored/MaxPowerCap).ToString("0") + "%\n" + (PowerStored/PowerOut).ToString("0") + " hours");
        else if (hours > 2/60)
            surface.WriteText((100*PowerStored/MaxPowerCap).ToString("0") + "%\n" + (60*PowerStored/PowerOut).ToString("0") + " minutes");
        else
            surface.WriteText((100*PowerStored/MaxPowerCap).ToString("0") + "%\n" + (3600*PowerStored/PowerOut).ToString("0") + " seconds");
    }

    return 100*PowerStored/MaxPowerCap;
}
double IceCount(IMyTextSurface surface = null, List<IMyTerminalBlock> blocks = null) {
    if (blocks == null)
        blocks = GetContainers();
    MyItemType IceItem = new MyItemType("MyObjectBuilder_Ore", "Ice");

    List<IMyTerminalBlock> generators = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(generators);
    blocks.AddRange(generators);

    double IceAmount = 0;
    foreach (IMyTerminalBlock block in blocks)
        if(Me.CubeGrid.ToString() == block.CubeGrid.ToString())
            IceAmount += block.GetInventory().GetItemAmount(IceItem).ToIntSafe();

    if (surface == null)
        return IceAmount;

    surface.ContentType = ContentType.TEXT_AND_IMAGE;
    surface.FontSize= 4;
    
    int OneGeneratorConsumption = 25;
    if(Me.CubeGrid.GridSizeEnum == MyCubeSize.Small)
        OneGeneratorConsumption = 5;
    if (IceAmount > 0.01) {
        surface.FontColor = White;
        if (IceAmount/(generators.Count*5) < 180)
            surface.FontColor = WarningColor;
        surface.WriteText("Ice: " + IceAmount.ToString() + "\n" + "Time:\n" + IceAmount/(generators.Count*OneGeneratorConsumption)+ " s"); 
    }
    else {
        surface.FontColor = WarningColor;
        surface.WriteText("Warning:\n0 Ice");
    }
    return IceAmount;
}
bool StoneEject(List<IMyTerminalBlock> blocks = null) {
    if (blocks == null)
        blocks = GetContainers();
    MyItemType StoneItem = new MyItemType("MyObjectBuilder_Ore", "Stone");
    bool stones = false;
    foreach (IMyTerminalBlock block in blocks)
        if(Me.CubeGrid.ToString() == block.CubeGrid.ToString())
            if(((double) block.GetInventory().GetItemAmount(StoneItem)) > 0)
                stones = true;

    MyInventoryItemFilter StoneFilterItem = new MyInventoryItemFilter(StoneItem);
    List<MyInventoryItemFilter> FilterListStone = new List<MyInventoryItemFilter>{StoneFilterItem};
    List<MyInventoryItem> EjecorInventoryList = new List<MyInventoryItem>();

    IMyConveyorSorter StoneSorter = GridTerminalSystem.GetBlockWithName("Stone Sorter") as IMyConveyorSorter;
    IMyShipConnector StoneEjector = GridTerminalSystem.GetBlockWithName("Stone Ejector") as IMyShipConnector;

    if(((double) StoneSorter.GetInventory().GetItemAmount(StoneItem)) > 0)
        stones = true;

    StoneEjector.GetInventory().GetItems(EjecorInventoryList);
    if
    (
        (
            ((stones) && (EjecorInventoryList.Count == 0))
                ||
            ((StoneEjector.GetInventory().GetItemAmount(StoneItem) > 0) && (EjecorInventoryList.Count == 1))
        )
    )
    {
        StoneSorter.SetFilter(MyConveyorSorterMode.Whitelist, FilterListStone);
        StoneSorter.DrainAll = true;
        StoneEjector.CollectAll = true;
        StoneEjector.ThrowOut = true;
        return true;
    }
    else {
        StoneSorter.DrainAll = false;
        StoneEjector.CollectAll = false;
        StoneEjector.ThrowOut = false;
        StoneSorter.SetFilter(MyConveyorSorterMode.Blacklist, FilterListStone);
        return false;
    }
}
double fullfilness(IMyTextSurface surface = null, List<IMyTerminalBlock> blocks = null) {
    if (blocks == null)
        blocks = GetContainers();
    MyFixedPoint size = 0;
    MyFixedPoint maxSize = 0;
    foreach (IMyTerminalBlock block in blocks)
        if(Me.CubeGrid.ToString() == block.CubeGrid.ToString()) {
            size += block.GetInventory().CurrentVolume;
            maxSize += block.GetInventory().MaxVolume;
        }
    double precentage = ( ((double) size)/(double) maxSize )*100;
    if(surface == null)
        return precentage;

    surface.FontColor = White;
    surface.FontSize= 4;

    if (Double.IsNaN(precentage)) {
        surface.WriteText("Containers\nnot found");
        return -1;
    }
    else {
        surface.WriteText("Filled:\n" + precentage.ToString("0") + "%");
        return precentage;
    }

}
public Program() {
    Runtime.UpdateFrequency=UpdateFrequency.Update10;
}
public void Main(string arg, UpdateType updateSource) {
    List<IMyTerminalBlock> containers = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> cockpits = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> drills = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> generators = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> connectors = new List<IMyTerminalBlock>();

    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(containers);
    GridTerminalSystem.GetBlocksOfType<IMyCockpit>(cockpits);
    GridTerminalSystem.GetBlocksOfType<IMyShipDrill>(drills);
    GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(generators);
    GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectors);

    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        //IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName("Containers");
        //group.GetBlocks(blocks);

    blocks.AddRange(containers);
    blocks.AddRange(cockpits);
    blocks.AddRange(drills);
    blocks.AddRange(connectors);
    IMyCockpit cockpit = cockpits[CockpitId] as IMyCockpit;

    if(PowerCheckBool)
        PowerCheck(cockpit.GetSurface(SurfPowerInt));
    if(IceCountBool)
        IceCount(cockpit.GetSurface(SurfIceInt));
    if (StoneEjectBool)
        StoneEject();
    if (FullfillnessBool)
        fullfilness(cockpit.GetSurface(SurfFullfilnessInt));
}