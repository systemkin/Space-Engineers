//Get knowledge about remaining of ice and energy (in addition over default hud - % of charge) in your ship. Also precentage of fullnesness of containers
//Small grid version
//Name all containers for ore and ice (o2/h2 generators too) as a group "Containers".

//Are you need Info about battery
bool PowerCheckMode = true;


public Program()
{
    Runtime.UpdateFrequency=UpdateFrequency.Update100;
}
public void Main(string arg, UpdateType updateSource)
{
    //Get a group with name "Containers"
    IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName("Containers");

    //Create empty list
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

    // Get blocks of a group to a list
    group.GetBlocks(blocks);

    //Empty list
    List<IMyTerminalBlock> generators = new List<IMyTerminalBlock>();

    //Fill list with a o2/h2 generatoors
    GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(generators);
    

    List<IMyTerminalBlock> batterys = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocksOfType<IMyBatteryBlock>(batterys);

    List<IMyCockpit> Cockpits = new List<IMyCockpit>();
    GridTerminalSystem.GetBlocksOfType<IMyCockpit>(Cockpits);

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
    double PowerIn = 0;
    double PowerOut = 0;
    double PowerStored = 0;
    double MaxPowerCap = 0;
    double AllGridsStored = 0;
    double AllGridsCap = 0;
    if (PowerCheckMode)
    {
        foreach (IMyBatteryBlock battery in batterys)
        {
            if(Me.CubeGrid.ToString() == battery.CubeGrid.ToString())
            {
                PowerIn += battery.CurrentInput;
                PowerOut += battery.CurrentOutput;
                PowerStored += battery.CurrentStoredPower;
                MaxPowerCap += battery.MaxStoredPower;
            }
                AllGridsStored += battery.CurrentStoredPower;
                AllGridsCap += battery.MaxStoredPower;
        }  
    }
    //Yo if you have more than 1 cockpit charnge this line
    IMyCockpit cockpit = Cockpits[0];

    IMyTextSurface surf = cockpit.GetSurface(1);  
    IMyTextSurface surf2 = cockpit.GetSurface(2);   
    IMyTextSurface surf3 = cockpit.GetSurface(3);

    surf.ContentType = ContentType.TEXT_AND_IMAGE;
    surf2.ContentType = ContentType.TEXT_AND_IMAGE;
    

    Color White = new Color(255,255,255);
    Color WarningColor = new Color(255,140,0);
    Color EmergencyColor = new Color(255,69,0);
    Color Green = new Color(50,205,50);

    if (amount > 0.01) 
    {
        surf.FontColor = White;
        //If time for consume ice is lower than 180 seconds - warning
        if (amount/(generators.Count*5) < 180)
            surf.FontColor = WarningColor;
        surf.FontSize= 4;
        surf.WriteText("Ice: " + amount.ToString() + "\n" + "Time:\n" + amount/(generators.Count*5)+ " s"); 
    }
    else
    {
        surf.FontColor = WarningColor;
        surf.FontSize= 4;
        surf.WriteText("Warning:\n0 Ice"); 
    }
    if(PowerCheckMode)
    {
        surf3.ContentType = ContentType.TEXT_AND_IMAGE;
        surf3.FontColor = White;
        surf3.FontSize= 4;
        double hours = PowerStored/(PowerOut-PowerIn);
        if(PowerStored/MaxPowerCap > 0.999)
        {
            surf3.FontColor = Green;
            if (AllGridsStored/AllGridsCap > 0.999)
                surf3.WriteText("All grids\ncharged");
            else 
                surf3.WriteText("This grid\ncharged");
        }
        else if (PowerIn > PowerOut)
           surf3.WriteText("Charging\n" + (100*PowerStored/MaxPowerCap).ToString("0") + "%"); 
        else
        {   
            
            Echo(PowerIn.ToString());
            if (hours < 5/60)
                surf3.FontColor = WarningColor;
            if (hours < 2/60)
                surf3.FontColor = EmergencyColor;
            if (hours > 1)
                surf3.WriteText((100*PowerStored/MaxPowerCap).ToString("0") + "%\n" + (PowerStored/PowerOut).ToString("0") + " hours");
            else if (hours > 2/60)
                surf3.WriteText((100*PowerStored/MaxPowerCap).ToString("0") + "%\n" + (60*PowerStored/PowerOut).ToString("0") + " minutes");
            else 
                surf3.WriteText((100*PowerStored/MaxPowerCap).ToString("0") + "%\n" + (3600*PowerStored/PowerOut).ToString("0") + " seconds");
        }
    }

    
    double precentage = ( ((double) size)/(double) maxSize )*100; 
    if (Double.IsNaN(precentage))
        surf2.WriteText("Containers\nnot found");
    else
        surf2.WriteText("Filled:\n" + precentage.ToString("0") + "%");
}
