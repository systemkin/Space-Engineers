//Small grid version
//Name all containers for ore and ice (o2/h2 generators too) as a group "Containers" and cockpit as a "Cockpit".
//Counts ice and time to consunme it. Shows results on a screens
public Program()
{
    Runtime.UpdateFrequency=UpdateFrequency.Update100;
}
public void Main(string arg, UpdateType updateSource)
{

    IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName("Containers");                   //Get a group with name "Containers"
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();                                   //Create empty list
    group.GetBlocks(blocks);                                                                        // Get blocks of a group to a list

    List<IMyTerminalBlock> generators = new List<IMyTerminalBlock>();                               //Empty list
    GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(generators);                                //Fill list with a o2/h2 generatoors
    

    double amount = 0;
    MyFixedPoint size = 0;
    MyFixedPoint maxSize = 0;

    foreach (IMyTerminalBlock p in blocks)
    {  
        if(Me.CubeGrid.ToString() == p.CubeGrid.ToString())                                           //If block on local grid
        {
            MyItemType icey = new MyItemType("MyObjectBuilder_Ore", "Ice");                           //Create itemtype "Ice"
            amount += p.GetInventory().GetItemAmount(icey).ToIntSafe();                               //Count amount of ice in a block
            size += p.GetInventory().CurrentVolume;                                                   //Count used volume of a block
            maxSize += p.GetInventory().MaxVolume;                                                    //Count all volume of a block
        }
    }
     foreach (IMyTerminalBlock p in generators)
    {
        if(Me.CubeGrid.ToString() == p.CubeGrid.ToString())
        {
            size -= p.GetInventory().CurrentVolume;                                                    //We dont want to count o2/h2 generator as a container for ore
            maxSize -= p.GetInventory().MaxVolume;
        }
    }
    IMyCockpit cockpit = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit; //Get Cockpit
    
    IMyTextSurface surf = cockpit.GetSurface(1);  
    IMyTextSurface surf2 = cockpit.GetSurface(2);
    surf.ContentType = ContentType.TEXT_AND_IMAGE;
    surf2.ContentType = ContentType.TEXT_AND_IMAGE;
    surf.FontSize= 4;   
    surf2.FontSize= 4;                                                                                  //Some LCD panel programming
    surf.WriteText("Ice: " + amount.ToString() + "\n" + "Time:\n" + amount/(generators.Count*5)+ " s"); 
    
    double precentage = ( ((double) size)/(double) maxSize )*100; 
    surf2.WriteText("Filled:\n" + precentage.ToString("0") + "%");
}
