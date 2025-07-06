namespace Drogecode.Knrm.Oefenrooster.PreCom.Models;

public class MsgOut
{
    public int MsgOutID { get; set; }
    public string ControlID { get; set; }
    public int MsgInID { get; set; }
    public DateTime Timestamp { get; set; }
    public DateTime ValidTo { get; set; }
    public string Text { get; set; }
    public MsgIn MsgIn { get; set; }
}
