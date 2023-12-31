public void Main(string arg, UpdateType updateSource)
{
    IMyBlockGroup group = GridTerminalSystem.GetBlockGroupWithName("Connectors");
    IMyBlockGroup group2 = GridTerminalSystem.GetBlockGroupWithName("Remote controls");

    List<IMyTerminalBlock> connectors = new List<IMyTerminalBlock>();
    List<IMyTerminalBlock> remotes = new List<IMyTerminalBlock>();
    group.GetBlocks(connectors);
    group2.GetBlocks(remotes);

    foreach (IMyRemoteControl p in remotes)
        if(Me.CubeGrid.ToString() != p.CubeGrid.ToString())
                p.DampenersOverride = true;
    foreach (IMyShipConnector p in connectors)
        if(Me.CubeGrid.ToString() == p.CubeGrid.ToString())
            p.Disconnect();
}
