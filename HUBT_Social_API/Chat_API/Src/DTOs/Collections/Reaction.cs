
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Models;

public class Reaction 
{
    public List<string> Reactions { get; set; } = new List<string>();
    public List<string> reactedUserIds { get; set; } = new List<string>();
    
}