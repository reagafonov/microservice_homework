using CommandLine;

namespace MoveCamera;

public class InputOptions
{
    [Option("ip", Required = true, HelpText = "The IP address of the onvif")]
    public string ip { get; set; }
    
    [Option("username", Required = true, HelpText = "The username of the onvif")]
    public string Username { get; set; }
    
    [Option("password", Required = true, HelpText = "The password of the onvif")]
    public string Password { get; set; }
    
    [Option("pan", Required = true, HelpText = "The position of the onvif")]
    public float Pan { get; set; }
    
    [Option("tilt", Required = true, HelpText = "The position of the onvif")]
    public float Tilt { get; set; }
}