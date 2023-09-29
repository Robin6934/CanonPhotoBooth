using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class WpfController : ControllerBase
{
    [HttpGet("triggerAction")]
    public IActionResult TriggerWpfAction()
    {
        // Perform the action here, e.g., send a signal to your WPF application.
        // You can use inter-process communication techniques to communicate with your WPF app.

        // Return a response to indicate the action was triggered successfully.
        return Ok("Action triggered successfully");
    }
}
